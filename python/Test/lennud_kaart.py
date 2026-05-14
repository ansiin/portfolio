from __future__ import annotations

import argparse
import os
from pathlib import Path

PROJECT_DIR = Path(__file__).resolve().parent
MPLCONFIGDIR = PROJECT_DIR / ".mplconfig"
XDG_DATA_HOME = PROJECT_DIR / ".local" / "share"
MPLCONFIGDIR.mkdir(exist_ok=True)
XDG_DATA_HOME.mkdir(parents=True, exist_ok=True)
os.environ.setdefault("MPLCONFIGDIR", str(MPLCONFIGDIR))
os.environ.setdefault("XDG_DATA_HOME", str(XDG_DATA_HOME))
os.environ.setdefault("CARTOPY_DATA_DIR", str(XDG_DATA_HOME / "cartopy"))

import cartopy.crs as ccrs
import cartopy.feature as cfeature
import matplotlib
import pandas as pd

import matplotlib.pyplot as plt
from matplotlib.lines import Line2D


AUTHOR_NAME = "Andres"
TALLINN_IATA = "TLL"


def load_airports() -> pd.DataFrame:
    airports = pd.read_csv(PROJECT_DIR / "airports.csv", usecols=["IATA", "Name", "City", "Country", "Latitude", "Longitude"])
    airports = airports.dropna(subset=["IATA", "Latitude", "Longitude"]).drop_duplicates(subset="IATA")
    return airports


def load_routes(filename: str) -> pd.DataFrame:
    routes = pd.read_csv(PROJECT_DIR / filename, sep=";")
    routes["IATA"] = routes["IATA"].str.strip()
    routes = routes.drop_duplicates(subset="IATA").reset_index(drop=True)
    return routes


def merge_routes(routes: pd.DataFrame, airports: pd.DataFrame, period: str) -> pd.DataFrame:
    merged = routes.merge(airports, on="IATA", how="left", validate="many_to_one")
    missing = merged.loc[merged["Latitude"].isna(), "IATA"].tolist()
    if missing:
        missing_codes = ", ".join(sorted(missing))
        raise ValueError(f"Koordinaadid puuduvad lennujaamadele: {missing_codes}")

    merged["Period"] = period
    return merged


def draw_routes(ax: plt.Axes, tallinn: pd.Series, routes: pd.DataFrame, color: str, label: str) -> None:
    ax.plot(
        routes["Longitude"],
        routes["Latitude"],
        linestyle="",
        marker="o",
        markersize=5,
        markerfacecolor=color,
        markeredgecolor="white",
        markeredgewidth=0.6,
        transform=ccrs.PlateCarree(),
        zorder=4,
    )

    for row in routes.itertuples(index=False):
        ax.plot(
            [tallinn["Longitude"], row.Longitude],
            [tallinn["Latitude"], row.Latitude],
            color=color,
            linewidth=1.4,
            alpha=0.8,
            transform=ccrs.Geodetic(),
            zorder=3,
        )

    ax.scatter(
        [tallinn["Longitude"]],
        [tallinn["Latitude"]],
        s=40,
        color="#111111",
        edgecolor="white",
        linewidth=0.7,
        transform=ccrs.PlateCarree(),
        zorder=5,
    )

    ax.text(
        tallinn["Longitude"] + 1.0,
        tallinn["Latitude"] + 0.25,
        f"Tallinn ({TALLINN_IATA})",
        fontsize=9,
        color="#111111",
        transform=ccrs.PlateCarree(),
        zorder=6,
    )


def build_map(output_file: Path) -> tuple[plt.Figure, Path]:
    airports = load_airports()
    routes_2020 = merge_routes(load_routes("otselennud20.csv"), airports, "Koroonaeelne aeg")
    routes_2026 = merge_routes(load_routes("otselennud26.csv"), airports, "Marts 2026")

    tallinn = airports.loc[airports["IATA"] == TALLINN_IATA].iloc[0]
    routes_2020 = routes_2020.loc[routes_2020["IATA"] != TALLINN_IATA].copy()
    routes_2026 = routes_2026.loc[routes_2026["IATA"] != TALLINN_IATA].copy()

    fig = plt.figure(figsize=(14, 10))
    ax = plt.axes(projection=ccrs.PlateCarree())
    ax.set_extent([-15, 45, 34, 72], crs=ccrs.PlateCarree())
    ax.set_facecolor("#a9c8eb")
    ax.add_feature(cfeature.OCEAN.with_scale("50m"), facecolor="#a9c8eb", zorder=0)
    ax.add_feature(cfeature.LAND.with_scale("50m"), facecolor="#f3f1df", edgecolor="none", zorder=1)
    ax.add_feature(cfeature.LAKES.with_scale("50m"), facecolor="#a9c8eb", edgecolor="#9bbcf0", linewidth=0.5, zorder=2)
    ax.add_feature(cfeature.RIVERS.with_scale("50m"), edgecolor="#9bbcf0", linewidth=0.5, zorder=2)
    ax.add_feature(cfeature.COASTLINE.with_scale("50m"), edgecolor="#222222", linewidth=0.8, zorder=2)
    ax.add_feature(cfeature.BORDERS.with_scale("50m"), edgecolor="#333333", linewidth=0.6, zorder=2)

    gridliner = ax.gridlines(
        draw_labels=True,
        linewidth=0.5,
        color="#ffffff",
        alpha=0.45,
        linestyle="--",
    )
    gridliner.top_labels = False
    gridliner.right_labels = False

    draw_routes(ax, tallinn, routes_2020, "#0b6efd", "Koroonaeelne aeg")
    draw_routes(ax, tallinn, routes_2026, "#ff6b35", "Marts 2026")

    legend_handles = [
        Line2D([0], [0], color="#0b6efd", marker="o", markersize=6, linewidth=1.4, label="Otselennud enne koroonat"),
        Line2D([0], [0], color="#ff6b35", marker="o", markersize=6, linewidth=1.4, label="Otselennud martsis 2026"),
        Line2D([0], [0], color="#111111", marker="o", markersize=7, linewidth=0, label="Tallinn"),
    ]
    ax.legend(handles=legend_handles, loc="lower left", frameon=True, framealpha=0.9)

    title = "Tallinna Lennujaamast valjuvad otselennud Euroopas"
    subtitle = f"Autor: {AUTHOR_NAME}"
    fig.suptitle(title, fontsize=18, fontweight="bold", y=0.96)
    fig.text(0.5, 0.925, subtitle, ha="center", fontsize=11)

    output_file.parent.mkdir(parents=True, exist_ok=True)
    fig.savefig(output_file, dpi=200, bbox_inches="tight")
    return fig, output_file


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Joonistab Tallinna otselennud Euroopa kaardile.")
    parser.add_argument("--output", default="lennud_uus.png", help="Väljundfaili nimi.")
    parser.add_argument("--no-show", action="store_true", help="Ei ava joonist aknas.")
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    if args.no_show:
        plt.switch_backend("Agg")

    output_path: Path
    fig: plt.Figure
    fig, output_path = build_map(PROJECT_DIR / args.output)
    print(f"Kaart salvestatud: {output_path}")
    if args.no_show:
        plt.close(fig)
    else:
        plt.show()


if __name__ == "__main__":
    main()
