from __future__ import annotations

import csv
from pathlib import Path
from typing import Any

from flask import Flask, jsonify, render_template, request

BASE_DIR = Path(__file__).resolve().parent
CSV_FILE = BASE_DIR / "cafes.csv"
FIELDNAMES = ["id", "name", "location", "time_open", "time_closed", "provider"]

app = Flask(__name__)


def normalize_time(value: str) -> str:
    """Convert user input into a strict HH:MM time string."""
    cleaned = value.strip().replace(".", ":")
    pieces = cleaned.split(":")
    if len(pieces) != 2:
        raise ValueError("Time must be in HH:MM format.")

    hours, minutes = pieces
    if not hours.isdigit() or not minutes.isdigit():
        raise ValueError("Time must contain only digits.")

    hour_value = int(hours)
    minute_value = int(minutes)
    if hour_value not in range(24) or minute_value not in range(60):
        raise ValueError("Time is out of range.")

    return f"{hour_value:02d}:{minute_value:02d}"


def time_to_minutes(value: str) -> int:
    """Convert a normalized time string into minutes from midnight."""
    normalized = normalize_time(value)
    hours, minutes = normalized.split(":")
    return int(hours) * 60 + int(minutes)


def read_cafes() -> list[dict[str, Any]]:
    """Load cafes from CSV and convert numeric fields to Python types."""
    with CSV_FILE.open("r", encoding="utf-8", newline="") as file:
        reader = csv.DictReader(file)
        cafes = []
        for row in reader:
            row["id"] = int(row["id"])
            cafes.append(row)
        return cafes


def write_cafes(cafes: list[dict[str, Any]]) -> None:
    """Persist the current cafe list back into the CSV file."""
    with CSV_FILE.open("w", encoding="utf-8", newline="") as file:
        writer = csv.DictWriter(file, fieldnames=FIELDNAMES)
        writer.writeheader()
        for cafe in cafes:
            writer.writerow(cafe)


def next_id(cafes: list[dict[str, Any]]) -> int:
    """Generate the next unique numeric identifier."""
    if not cafes:
        return 1
    return max(cafe["id"] for cafe in cafes) + 1


def validate_payload(payload: dict[str, Any], partial: bool = False) -> dict[str, Any]:
    """Validate incoming JSON and normalize fields before saving."""
    allowed_fields = {"name", "location", "time_open", "time_closed", "provider"}
    required_fields = {"name", "location", "time_open", "time_closed"}

    if not isinstance(payload, dict):
        raise ValueError("JSON body must be an object.")

    unknown_fields = set(payload) - allowed_fields
    if unknown_fields:
        raise ValueError(f"Unknown fields: {', '.join(sorted(unknown_fields))}")

    if not partial:
        missing = required_fields - set(payload)
        if missing:
            raise ValueError(f"Missing fields: {', '.join(sorted(missing))}")

    validated: dict[str, Any] = {}
    for field in allowed_fields:
        if field in payload:
            value = str(payload[field]).strip()
            if not value and field in required_fields:
                raise ValueError(f"Field '{field}' cannot be empty.")
            if field in {"time_open", "time_closed"} and value:
                value = normalize_time(value)
            validated[field] = value

    open_value = validated.get("time_open")
    closed_value = validated.get("time_closed")
    if open_value and closed_value and time_to_minutes(open_value) > time_to_minutes(closed_value):
        raise ValueError("Opening time cannot be later than closing time.")

    return validated


def filter_cafes_by_time(cafes: list[dict[str, Any]], start: str, end: str) -> list[dict[str, Any]]:
    """Return cafes that stay open for the whole requested time range."""
    start_minutes = time_to_minutes(start)
    end_minutes = time_to_minutes(end)
    if start_minutes > end_minutes:
        raise ValueError("Filter start time cannot be later than end time.")

    matches = []
    for cafe in cafes:
        cafe_open = time_to_minutes(cafe["time_open"])
        cafe_closed = time_to_minutes(cafe["time_closed"])
        if cafe_open <= start_minutes and cafe_closed >= end_minutes:
            matches.append(cafe)
    return matches


def find_cafe_by_id(cafes: list[dict[str, Any]], cafe_id: int) -> dict[str, Any] | None:
    """Find a single cafe by its identifier."""
    return next((item for item in cafes if item["id"] == cafe_id), None)


def create_cafe_record(cafes: list[dict[str, Any]], validated: dict[str, Any]) -> dict[str, Any]:
    """Build a new cafe record from validated request data."""
    return {
        "id": next_id(cafes),
        "name": validated["name"],
        "location": validated["location"],
        "time_open": validated["time_open"],
        "time_closed": validated["time_closed"],
        "provider": validated.get("provider", ""),
    }


def error_response(message: str, status_code: int):
    """Create a consistent JSON error response for the API."""
    return jsonify({"error": message}), status_code


@app.get("/")
def index():
    """Render the main web interface."""
    return render_template("index.html")


@app.get("/api/cafes")
def get_cafes():
    """Return all cafes or filter them by a requested time range."""
    cafes = read_cafes()
    start = request.args.get("start")
    end = request.args.get("end")

    if start or end:
        if not start or not end:
            return error_response("Both start and end query parameters are required.", 400)
        try:
            cafes = filter_cafes_by_time(cafes, start, end)
        except ValueError as error:
            return error_response(str(error), 400)

    return jsonify(cafes)


@app.get("/api/cafes/<int:cafe_id>")
def get_cafe(cafe_id: int):
    """Return one cafe by its ID."""
    cafes = read_cafes()
    cafe = find_cafe_by_id(cafes, cafe_id)
    if cafe is None:
        return error_response("Cafe not found.", 404)
    return jsonify(cafe)


@app.post("/api/cafes")
def create_cafe():
    """Create a new cafe and save it into the CSV dataset."""
    cafes = read_cafes()
    payload = request.get_json(silent=True) or {}

    try:
        validated = validate_payload(payload)
    except ValueError as error:
        return error_response(str(error), 400)

    cafe = create_cafe_record(cafes, validated)
    cafes.append(cafe)
    write_cafes(cafes)
    return jsonify(cafe), 201


@app.put("/api/cafes/<int:cafe_id>")
def update_cafe(cafe_id: int):
    """Update an existing cafe with the provided fields."""
    cafes = read_cafes()
    cafe = find_cafe_by_id(cafes, cafe_id)
    if cafe is None:
        return error_response("Cafe not found.", 404)

    payload = request.get_json(silent=True) or {}
    try:
        validated = validate_payload(payload, partial=True)
    except ValueError as error:
        return error_response(str(error), 400)

    updated = {**cafe, **validated}
    if time_to_minutes(updated["time_open"]) > time_to_minutes(updated["time_closed"]):
        return error_response("Opening time cannot be later than closing time.", 400)

    cafe.update(validated)
    write_cafes(cafes)
    return jsonify(cafe)


@app.delete("/api/cafes/<int:cafe_id>")
def delete_cafe(cafe_id: int):
    """Delete a cafe from the dataset by its ID."""
    cafes = read_cafes()
    remaining = [item for item in cafes if item["id"] != cafe_id]
    if len(remaining) == len(cafes):
        return error_response("Cafe not found.", 404)

    write_cafes(remaining)
    return jsonify({"message": f"Cafe {cafe_id} deleted."})


if __name__ == "__main__":
    app.run(debug=True)
