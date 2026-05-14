# Python HW4: stringi pooramise pakett ja kasutajaliides

## Osa 1: moodul

Paketi lähtekood asub kaustas `package`.

Mooduli funktsioon:

```python
from andres import reverse_string

reverse_string("arvuti")  # "ituvra"
```

TestPyPI projekti aadress:

```text
https://test.pypi.org/project/andres/
```

Paketi ehitamine:

```powershell
cd package
python -m pip install --upgrade build twine
python -m build
```

Paketi üleslaadimine TestPyPI keskkonda:

```powershell
python -m twine upload --repository testpypi dist/*
```

Kui `twine` küsib kasutajat, kasuta TestPyPI API tokenit nii:

```powershell
python -m twine upload --repository-url https://test.pypi.org/legacy/ -u __token__ -p <SINU_TESTPYPI_TOKEN> dist/*
```

Tagasi installeerimine TestPyPI keskkonnast:

```powershell
python -m pip install --index-url https://test.pypi.org/simple/ --no-deps andres
```

Alternatiivne installikäsk:

```powershell
python -m pip install -i https://test.pypi.org/simple/ --no-deps andres
```

## Osa 2: kasutajaliides

Kasutajaliidese kood asub kaustas `gui`.

Oluline: GUI ei sisalda stringi pööramise funktsioonikoodi otse. Fail `gui/app.py` impordib funktsiooni `reverse_string` moodulist `andres`, mis peab olema arvutisse installeeritud.

Käivitamine pärast paketi installeerimist:

```powershell
python gui/app.py
```

## Failid Moodle jaoks

Laadi üles:

- `package/src/andres/__init__.py`
- `package/pyproject.toml`
- `gui/app.py`
- `README.md`
