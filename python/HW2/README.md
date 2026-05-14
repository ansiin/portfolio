# TalTechi kohvikute API ja veebileht

Projekt on tehtud Flaskiga ja sisaldab:

- REST API-t kohvikute pärimiseks, lisamiseks, muutmiseks ja kustutamiseks
- veebilehte, mis kasutab sama API-t brauserist `fetch()` kaudu
- algandmeid UTF-8 kodeeringus CSV failis `cafes.csv`
- eraldi faile HTML-i, CSS-i ja JavaScripti jaoks

## Käivitamine

1. Loo virtuaalkeskkond:

```powershell
python -m venv .venv
```

2. Aktiveeri see:

```powershell
.venv\Scripts\Activate.ps1
```

3. Paigalda sõltuvused:

```powershell
pip install -r requirements.txt
```

4. Käivita rakendus:

```powershell
python app.py
```

5. Ava brauseris:

```text
http://127.0.0.1:5000
```

## API näited

- `GET /api/cafes` tagastab kõik kohvikud
- `GET /api/cafes?start=18:30&end=21:00` tagastab kohvikud, mis on kogu selle ajavahemiku jooksul avatud
- `POST /api/cafes` lisab uue kohviku
- `PUT /api/cafes/3` muudab kohviku andmeid
- `DELETE /api/cafes/3` kustutab kohviku

## Märkused

- Täpitähed säilivad, sest CSV faili loetakse ja kirjutatakse `utf-8` kodeeringuga.
- Muudatused salvestatakse tagasi `cafes.csv` faili.
- Moodle jaoks pakitavas `.zip` failis ära lisa `.venv`, `__pycache__`, `.idea` ega muid genereeritud faile.
