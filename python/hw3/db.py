import sqlite3
from pathlib import Path


DB_PATH = Path("products.db")


def get_connection():
    connection = sqlite3.connect(DB_PATH)
    connection.row_factory = sqlite3.Row
    return connection


def init_db():
    with get_connection() as connection:
        connection.execute(
            """
            CREATE TABLE IF NOT EXISTS products (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                title TEXT NOT NULL,
                price REAL NOT NULL,
                picture_href TEXT NOT NULL,
                product_url TEXT NOT NULL UNIQUE
            )
            """
        )


def replace_products(products):
    init_db()
    with get_connection() as connection:
        connection.execute("DELETE FROM products")
        connection.executemany(
            """
            INSERT OR IGNORE INTO products
                (title, price, picture_href, product_url)
            VALUES
                (:title, :price, :picture_href, :product_url)
            """,
            products,
        )


def find_products_more_expensive_than(min_price):
    init_db()
    with get_connection() as connection:
        return connection.execute(
            """
            SELECT title, price, picture_href, product_url
            FROM products
            WHERE price > ?
            ORDER BY price ASC, title ASC
            """,
            (min_price,),
        ).fetchall()


def count_products():
    init_db()
    with get_connection() as connection:
        row = connection.execute("SELECT COUNT(*) AS total FROM products").fetchone()
        return row["total"]
