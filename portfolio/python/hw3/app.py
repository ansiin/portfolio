from flask import Flask, render_template, request

from db import count_products, find_products_more_expensive_than, init_db


app = Flask(__name__)


@app.route("/")
def index():
    raw_min_price = request.args.get("min_price", "20")
    try:
        min_price = float(raw_min_price.replace(",", "."))
    except ValueError:
        min_price = 20.0

    products = find_products_more_expensive_than(min_price)
    return render_template(
        "index.html",
        products=products,
        min_price=min_price,
        product_count=count_products(),
    )


if __name__ == "__main__":
    init_db()
    app.run(debug=True)
