import json
import re
import unicodedata
from html import unescape
from pathlib import Path
from urllib.parse import parse_qs, unquote, urljoin, urlparse

import requests
from bs4 import BeautifulSoup

from db import replace_products


BASE_URL = "https://rahvaraamat.ee"
LIST_URL = (
    "https://rahvaraamat.ee/et/raamatud"
    "?productAvailabilityCodes=WEB"
    "&page={page}"
    "&sort=-top"
    "&productType=BOOK"
    "&language=et"
    "&openSearchResults=true"
)
MAX_PAGES = 8
JSON_PATH = Path("products.json")

HEADERS = {
    "User-Agent": (
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) "
        "AppleWebKit/537.36 (KHTML, like Gecko) "
        "Chrome/124.0 Safari/537.36"
    )
}


def fetch_html(page):
    response = requests.get(LIST_URL.format(page=page), headers=HEADERS, timeout=20)
    response.raise_for_status()
    return response.text


def class_contains(tag, text):
    classes = tag.get("class") or []
    return any(text in class_name for class_name in classes)


def parse_price(value):
    match = re.search(r"(\d+(?:[ ,]\d{3})*(?:[,.]\d{1,2})?)\s*(?:€|EUR)", value)
    if not match:
        return None
    normalized = match.group(1).replace(" ", "").replace(",", ".")
    return float(normalized)


def normalize_picture_url(url):
    if not url:
        return ""
    url = unescape(url)
    parsed = urlparse(url)
    query_url = parse_qs(parsed.query).get("url", [None])[0]
    if query_url:
        return unquote(query_url)
    return urljoin(BASE_URL, url)


def image_from_card(card):
    image = card.find("img", alt=True)
    if image is None:
        return ""
    return normalize_picture_url(image.get("src") or "")


def decode_json_string(value):
    try:
        return json.loads(f'"{value}"')
    except json.JSONDecodeError:
        return value


def slugify_path_part(value):
    value = decode_json_string(value).strip().lower()
    value = unicodedata.normalize("NFKD", value)
    value = "".join(character for character in value if not unicodedata.combining(character))
    value = value.replace("õ", "o").replace("ä", "a").replace("ö", "o").replace("ü", "u")
    value = re.sub(r"[^a-z0-9]+", "-", value)
    return value.strip("-")


def product_url_from_category_path(category_path, product_slug, product_id):
    parts = [slugify_path_part(part) for part in decode_json_string(category_path).split("/")]
    parts = [part for part in parts if part]
    return f"{BASE_URL}/et/{'/'.join(parts)}/{decode_json_string(product_slug)}/{product_id}"


def parse_embedded_products(html):
    pattern = re.compile(
        r'\{\\"id\\":(\d+),\\"name\\":\\"((?:\\\\.|[^\\"])*)\\"'
        r'.*?\\"thumb_file_url\\":\\"([^\\"]*)\\"'
        r'.*?\\"price\\":\{\\"price\\":([0-9.]+)'
        r'.*?\\"slug\\":\\"([^\\"]*)\\"'
        r'.*?\\"categoryPath\\":\\"([^\\"]*)\\"',
        re.S,
    )
    products = []
    seen_urls = set()

    for product_id, title, image_url, price, slug, category_path in pattern.findall(html):
        product_url = product_url_from_category_path(category_path, slug, product_id)
        if product_url in seen_urls:
            continue
        seen_urls.add(product_url)

        products.append(
            {
                "title": decode_json_string(title),
                "price": float(price),
                "picture_href": decode_json_string(image_url),
                "product_url": product_url,
            }
        )

    return products


def parse_products(html):
    soup = BeautifulSoup(html, "html.parser")
    products = []
    seen_urls = set()

    for card in soup.find_all("div"):
        if not class_contains(card, "styles_productCard__"):
            continue

        title_holder = card.find(class_=lambda value: value and "styles_productTitle__" in value)
        if title_holder is None:
            continue

        title_link = title_holder.find("a", href=True)
        price_holder = card.find(class_=lambda value: value and "styles_productPrice__" in value)
        if title_link is None or price_holder is None:
            continue

        price = parse_price(price_holder.get_text(" ", strip=True))
        if price is None:
            continue

        product_url = urljoin(BASE_URL, title_link["href"])
        if product_url in seen_urls:
            continue
        seen_urls.add(product_url)

        products.append(
            {
                "title": title_link.get_text(" ", strip=True),
                "price": price,
                "picture_href": image_from_card(card),
                "product_url": product_url,
            }
        )

    for product in parse_embedded_products(html):
        if product["product_url"] in seen_urls:
            continue
        seen_urls.add(product["product_url"])
        products.append(product)

    return products


def scrape_pages(page=1, max_pages=MAX_PAGES, collected=None):
    """Recursive scraper: page 1 calls page 2, and so on until max_pages."""
    if collected is None:
        collected = []
    if page > max_pages:
        return collected

    html = fetch_html(page)
    collected.extend(parse_products(html))
    return scrape_pages(page + 1, max_pages, collected)


def deduplicate(products):
    unique = {}
    for product in products:
        unique[product["product_url"]] = product
    return list(unique.values())


def main():
    products = deduplicate(scrape_pages())
    JSON_PATH.write_text(
        json.dumps(products, ensure_ascii=False, indent=2),
        encoding="utf-8",
    )
    replace_products(products)
    return len(products)


if __name__ == "__main__":
    main()
