"""Track Steam Market case prices and log profit for a fixed inventory."""

import os
import re
from datetime import datetime
from typing import Optional, Tuple

import requests
import tkinter as tk
from tkinter import messagebox, ttk

GLOVES_QUANTITY = 43
SPECTRUM_QUANTITY = 83
FILENAME = "case_prices.txt"
LOGFILE = "profit_log.txt"
INITIAL_PRICES_FILE = "initial_prices.txt"


def parse_price_value(raw_price: str) -> Optional[float]:
    """Convert Steam price text such as ``15,61€`` into a float."""

    normalized = raw_price.replace(",", ".").replace(" ", "").strip()
    match = re.search(r"\d+(?:\.\d+)?", normalized)
    if not match:
        return None
    return float(match.group(0))


def get_case_price(case_name: str) -> Optional[float]:
    """Return the current lowest Steam Market price for the given case."""

    url = (
        "https://steamcommunity.com/market/priceoverview/"
        f"?appid=730&currency=3&market_hash_name={case_name}"
    )
    try:
        response = requests.get(url, timeout=15)
        data = response.json()
        if data.get("success") and "lowest_price" in data:
            return parse_price_value(data["lowest_price"])
        return None
    except Exception:
        return None


def load_initial_prices() -> Tuple[Optional[float], Optional[float]]:
    """Load the baseline prices used for later profit calculations."""

    if not os.path.exists(INITIAL_PRICES_FILE):
        return None, None

    with open(INITIAL_PRICES_FILE, "r", encoding="utf-8") as file:
        lines = file.readlines()

    initial_gloves_price = None
    initial_spectrum_price = None

    for line in lines:
        if "Gloves" in line:
            initial_gloves_price = float(line.split(":")[1].strip())
        elif "Spectrum" in line:
            initial_spectrum_price = float(line.split(":")[1].strip())

    return initial_gloves_price, initial_spectrum_price


def save_initial_prices(gloves_price: Optional[float], spectrum_price: Optional[float]) -> None:
    """Persist the first successfully fetched prices as the baseline."""

    with open(INITIAL_PRICES_FILE, "w", encoding="utf-8") as file:
        if gloves_price is not None:
            file.write(f"Gloves: {gloves_price}\n")
        if spectrum_price is not None:
            file.write(f"Spectrum: {spectrum_price}\n")


def calculate_profit(
    initial_price: Optional[float], current_price: Optional[float], quantity: int
) -> Optional[float]:
    """Calculate profit or loss for the tracked quantity."""

    if initial_price is None or current_price is None:
        return None
    return (quantity * current_price) - (quantity * initial_price)


def save_results(gloves_profit: float, spectrum_profit: float) -> None:
    """Append the current profit snapshot to the history log."""

    current_date = datetime.now().strftime("%d/%m/%Y")
    with open(LOGFILE, "a", encoding="utf-8") as file:
        file.write(
            f"{current_date} - Gloves Profit: {gloves_profit:.2f}€, "
            f"Spectrum Profit: {spectrum_profit:.2f}€\n"
        )


class CaseTrackerApp:
    """Render the tracker UI and coordinate price refreshes."""

    def __init__(self, root: tk.Tk):
        """Create the widgets and load the first price snapshot."""

        self.root = root
        self.root.title("CSGO Case Tracker")
        self.root.configure(bg="#1E1E1E")
        self.root.geometry("400x300")

        style = ttk.Style()
        style.configure("TLabel", foreground="white", background="#1E1E1E", font=("Arial", 12))
        style.configure("TButton", foreground="black", background="white", font=("Arial", 10))
        style.configure("Title.TLabel", font=("Arial", 14, "bold"))

        ttk.Label(root, text="Gloves Case", style="Title.TLabel").grid(row=0, column=0, padx=10, pady=5)
        self.gloves_label = ttk.Label(root, text="Laeb...", style="TLabel")
        self.gloves_label.grid(row=1, column=0, padx=10, pady=5)

        ttk.Label(root, text="Spectrum 2 Case", style="Title.TLabel").grid(
            row=0, column=1, padx=10, pady=5
        )
        self.spectrum_label = ttk.Label(root, text="Laeb...", style="TLabel")
        self.spectrum_label.grid(row=1, column=1, padx=10, pady=5)

        self.refresh_button = ttk.Button(root, text="Uuenda andmeid", command=self.update_prices)
        self.refresh_button.grid(row=2, column=0, columnspan=2, pady=10)

        self.log_button = ttk.Button(root, text="Vaata logi", command=self.show_log)
        self.log_button.grid(row=3, column=0, columnspan=2, pady=5)

        self.update_prices()

    def update_prices(self) -> None:
        """Fetch current prices, refresh the UI, and write a log entry."""

        initial_gloves_price, initial_spectrum_price = load_initial_prices()
        gloves_price = get_case_price("Glove%20Case")
        spectrum_price = get_case_price("Spectrum%202%20Case")

        if initial_gloves_price is None and gloves_price is not None:
            initial_gloves_price = gloves_price
        if initial_spectrum_price is None and spectrum_price is not None:
            initial_spectrum_price = spectrum_price
        save_initial_prices(initial_gloves_price, initial_spectrum_price)

        gloves_profit = calculate_profit(initial_gloves_price, gloves_price, GLOVES_QUANTITY) or 0.00
        spectrum_profit = (
            calculate_profit(initial_spectrum_price, spectrum_price, SPECTRUM_QUANTITY) or 0.00
        )

        gloves_text = (
            "Hinnad puuduvad"
            if initial_gloves_price is None or gloves_price is None
            else (
                f"Kogus: {GLOVES_QUANTITY}\n"
                f"Esialgne hind: {initial_gloves_price:.2f}€\n"
                f"Uus hind: {gloves_price:.2f}€\n"
                f"Kasum: {gloves_profit:.2f}€"
            )
        )

        spectrum_text = (
            "Hinnad puuduvad"
            if initial_spectrum_price is None or spectrum_price is None
            else (
                f"Kogus: {SPECTRUM_QUANTITY}\n"
                f"Esialgne hind: {initial_spectrum_price:.2f}€\n"
                f"Uus hind: {spectrum_price:.2f}€\n"
                f"Kasum: {spectrum_profit:.2f}€"
            )
        )

        self.gloves_label.config(text=gloves_text)
        self.spectrum_label.config(text=spectrum_text)

        save_results(gloves_profit, spectrum_profit)

    def show_log(self) -> None:
        """Open a read-only window with the full profit history."""

        if not os.path.exists(LOGFILE):
            messagebox.showinfo("Logi", "Logifaili pole veel loodud.")
            return

        with open(LOGFILE, "r", encoding="utf-8") as file:
            log_data = file.read()

        log_window = tk.Toplevel(self.root)
        log_window.title("Profit Log")
        log_window.configure(bg="#1E1E1E")

        log_text = tk.Text(log_window, width=50, height=20, fg="white", bg="#1E1E1E", font=("Arial", 10))
        log_text.insert("1.0", log_data)
        log_text.pack()
        log_text.config(state="disabled")


if __name__ == "__main__":
    root = tk.Tk()
    app = CaseTrackerApp(root)
    root.mainloop()
