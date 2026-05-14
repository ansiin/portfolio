import tkinter as tk
from tkinter import messagebox

try:
    from andres import reverse_string
except ImportError as exc:
    raise SystemExit(
        "Moodul andres ei ole installeeritud. "
        "Paigalda see enne GUI kaivitamist TestPyPI kaudu."
    ) from exc


def reverse_input():
    output_var.set(reverse_string(input_var.get()))


root = tk.Tk()
root.title("Stringi pooraja")
root.geometry("420x180")
root.resizable(False, False)

input_var = tk.StringVar()
output_var = tk.StringVar()

frame = tk.Frame(root, padx=16, pady=16)
frame.pack(fill="both", expand=True)

tk.Label(frame, text="Sisend:").grid(row=0, column=0, sticky="w", pady=(0, 8))
tk.Entry(frame, textvariable=input_var, width=42).grid(row=0, column=1, pady=(0, 8))

tk.Button(frame, text="Poora string", command=reverse_input).grid(
    row=1, column=1, sticky="ew", pady=(0, 8)
)

tk.Label(frame, text="Valjund:").grid(row=2, column=0, sticky="w")
tk.Entry(frame, textvariable=output_var, width=42, state="readonly").grid(row=2, column=1)

try:
    root.mainloop()
except tk.TclError as exc:
    messagebox.showerror("Viga", str(exc))
