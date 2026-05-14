"""Serve a small web dashboard for tracking CS case prices and profit history."""

from __future__ import annotations

import json
import re
from datetime import datetime
from functools import partial
from http import HTTPStatus
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from typing import Optional
from urllib.parse import parse_qs, urlparse

import requests

GLOVES_QUANTITY = 43
SPECTRUM_QUANTITY = 83
INITIAL_PRICES_FILE = Path(__file__).with_name("initial_prices.txt")
LOGFILE = Path(__file__).with_name("profit_log.txt")
WEB_DIR = Path(__file__).with_name("web")
HOST = "127.0.0.1"
PORT = 8080

PRICE_ENDPOINTS = {
    "gloves": {
        "label": "Glove Case",
        "market_hash_name": "Glove%20Case",
        "quantity": GLOVES_QUANTITY,
        "baseline_key": "Gloves",
    },
    "spectrum": {
        "label": "Spectrum 2 Case",
        "market_hash_name": "Spectrum%202%20Case",
        "quantity": SPECTRUM_QUANTITY,
        "baseline_key": "Spectrum",
    },
}

LOG_PATTERN = re.compile(
    r"(?P<date>\d{2}/\d{2}/\d{4})\s*-\s*Gloves Profit:\s*"
    r"(?P<gloves>-?\d+(?:[.,]\d+)?)"
    r".*?Spectrum Profit:\s*(?P<spectrum>-?\d+(?:[.,]\d+)?)",
    re.IGNORECASE,
)


def parse_number(raw_value: str) -> Optional[float]:
    """Extract the first decimal number from a Steam/API string."""

    normalized = raw_value.replace(" ", "").replace(",", ".").strip()
    match = re.search(r"-?\d+(?:\.\d+)?", normalized)
    if not match:
        return None
    return float(match.group(0))


def fetch_case_price(case_name: str) -> Optional[float]:
    """Return the current lowest Steam Market price for one case."""

    url = (
        "https://steamcommunity.com/market/priceoverview/"
        f"?appid=730&currency=3&market_hash_name={case_name}"
    )
    try:
        response = requests.get(url, timeout=15)
        response.raise_for_status()
        data = response.json()
        lowest_price = data.get("lowest_price")
        if not data.get("success") or lowest_price is None:
            return None
        return parse_number(lowest_price)
    except Exception:
        return None


def load_initial_prices() -> dict[str, Optional[float]]:
    """Load the baseline purchase prices from disk."""

    prices: dict[str, Optional[float]] = {"Gloves": None, "Spectrum": None}
    if not INITIAL_PRICES_FILE.exists():
        return prices

    for line in INITIAL_PRICES_FILE.read_text(encoding="utf-8").splitlines():
        if ":" not in line:
            continue
        key, raw_value = line.split(":", 1)
        value = parse_number(raw_value)
        if key.strip() in prices:
            prices[key.strip()] = value
    return prices


def compute_profit(initial_price: Optional[float], current_price: Optional[float], quantity: int) -> Optional[float]:
    """Calculate the delta from the baseline position value."""

    if initial_price is None or current_price is None:
        return None
    return (current_price - initial_price) * quantity


def parse_profit_history() -> list[dict[str, object]]:
    """Parse the historical profit log and reconstruct price points."""

    baselines = load_initial_prices()
    entries: list[dict[str, object]] = []
    if not LOGFILE.exists():
        return entries

    for index, line in enumerate(LOGFILE.read_text(encoding="utf-8", errors="replace").splitlines()):
        match = LOG_PATTERN.search(line)
        if not match:
            continue

        date = datetime.strptime(match.group("date"), "%d/%m/%Y")
        gloves_profit = parse_number(match.group("gloves"))
        spectrum_profit = parse_number(match.group("spectrum"))
        gloves_base = baselines.get("Gloves")
        spectrum_base = baselines.get("Spectrum")

        gloves_price = None
        if gloves_profit is not None and gloves_base is not None:
            gloves_price = gloves_base + (gloves_profit / GLOVES_QUANTITY)

        spectrum_price = None
        if spectrum_profit is not None and spectrum_base is not None:
            spectrum_price = spectrum_base + (spectrum_profit / SPECTRUM_QUANTITY)

        entries.append(
            {
                "id": index,
                "date": date.strftime("%Y-%m-%d"),
                "label": date.strftime("%d.%m.%Y"),
                "glovesProfit": gloves_profit,
                "spectrumProfit": spectrum_profit,
                "glovesPrice": round(gloves_price, 2) if gloves_price is not None else None,
                "spectrumPrice": round(spectrum_price, 2) if spectrum_price is not None else None,
            }
        )

    return entries


def build_dashboard_payload() -> dict[str, object]:
    """Collect live market data plus historical entries for the web UI."""

    baselines = load_initial_prices()
    current = {}
    for key, meta in PRICE_ENDPOINTS.items():
        baseline = baselines.get(meta["baseline_key"])
        current_price = fetch_case_price(meta["market_hash_name"])
        profit = compute_profit(baseline, current_price, meta["quantity"])
        current[key] = {
            "label": meta["label"],
            "quantity": meta["quantity"],
            "baselinePrice": baseline,
            "currentPrice": round(current_price, 2) if current_price is not None else None,
            "profit": round(profit, 2) if profit is not None else None,
        }

    history = parse_profit_history()
    last_updated = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    return {
        "lastUpdated": last_updated,
        "current": current,
        "history": history,
    }


class DashboardHandler(SimpleHTTPRequestHandler):
    """Serve static dashboard files and a JSON API."""

    def __init__(self, *args, directory: str | None = None, **kwargs):
        super().__init__(*args, directory=directory, **kwargs)

    def do_GET(self) -> None:  # noqa: N802
        parsed = urlparse(self.path)
        if parsed.path == "/api/dashboard":
            self.handle_dashboard_api(parsed.query)
            return
        if parsed.path == "/":
            self.path = "/index.html"
        super().do_GET()

    def handle_dashboard_api(self, query: str) -> None:
        _ = parse_qs(query)
        payload = json.dumps(build_dashboard_payload()).encode("utf-8")
        self.send_response(HTTPStatus.OK)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(payload)))
        self.send_header("Cache-Control", "no-store")
        self.end_headers()
        self.wfile.write(payload)


def main() -> None:
    """Start the local web dashboard server."""

    handler = partial(DashboardHandler, directory=str(WEB_DIR))
    server = ThreadingHTTPServer((HOST, PORT), handler)
    print(f"Dashboard running at http://{HOST}:{PORT}")
    server.serve_forever()


if __name__ == "__main__":
    main()
