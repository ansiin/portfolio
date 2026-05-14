# Veebiämblik: Rahva Raamatu tooted

See on TalTechi kodutöö lahendus lihtsa veebiämbliku, SQLite andmebaasi ja Flaski veebilehega.

## Mida esitada

Esita kogu projekti kaust või ZIP-fail nende failidega:

- `scraper.py` - Beautiful Soupiga tehtud rekursiivne kaabits
- `db.py` - SQLite andmebaasi loomine ja päringud
- `app.py` - Flaski veebirakendus
- `templates/index.html` - veebilehe HTML
- `requirements.txt` - vajalikud Python teegid
- `README.md` - käivitamise juhend
- `products.db` - kaabitud andmetega SQLite andmebaas
- `products.json` - kaabitud andmete JSON varukoopia

`__pycache__` kausta ei pea esitama.

## Valitud veebipood

Veebipood: Rahva Raamat  
Kategooria/nimekiri: Rahva Raamatu raamatute nimekiri  
1a.ee veebipoodi ei kasutata.

Kaabits käib rekursiivselt läbi 8 alamlehte. Andmebaasi salvestatakse toote:

- pealkiri
- hind
- pildi aadress
- toote link

Praeguse kaabitsemise tulemusena on andmebaasis 172 toodet, mis täidab nõude "vähemalt 150 objekti".

## Käivitamine

1. Paigalda vajalikud teegid:

```powershell
pip install -r requirements.txt
```

2. Käivita kaabits:

```powershell
python scraper.py
```

See loob või uuendab failid:

- `products.json`
- `products.db`

3. Käivita veebirakendus:

```powershell
python app.py
```

4. Ava brauseris:

```text
http://127.0.0.1:5000
```

## Veebilehe kasutamine

Veebilehel saab sisestada hinna ja otsida kõiki tooteid, mis on sellest hinnast kallimad.

Näiteks kõik tooted hinnaga üle 20 euro:

```text
http://127.0.0.1:5000/?min_price=20
```

Kõik andmebaasis olevad tooted:

```text
http://127.0.0.1:5000/?min_price=0
```

## Märkused

Kaabits ei prindi kõiki tooteid konsooli. Andmed salvestatakse automaatselt andmebaasi ja JSON faili.

Toodete lingid moodustatakse Rahva Raamatu kategooriaraja järgi, näiteks:

```text
https://rahvaraamat.ee/et/raamatud/ilukirjandus/kaasaegne-ilukirjandus/ellujaamiskursus/2851067
```
