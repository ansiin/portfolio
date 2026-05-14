# VAATA-KUI-JULGED

See repo sisaldab vaikest CS2/CS:GO case tracker'it, mis vordleb Steam Marketi hindu sinu algsete ostuhindadega ja arvutab kahe case'i kasumi voi kahjumi.

Praegu jalgitavad itemid:

- `Glove Case`
- `Spectrum 2 Case`

Projektis on kaks eri osa:

- [`CASE.txt`](/C:/VAATA-KUI-JULGED/CASE.txt) sisaldab kasitsi kogutud linke ja markmeid.
- [`Calculator/brain.py`](/C:/VAATA-KUI-JULGED/Calculator/brain.py) on GUI-rakendus, mis loeb hinnad Steamist ja kirjutab tulemused logisse.

## Kuidas see tootab

Rakendus teeb Steam Marketi `priceoverview` paringu, loeb kahe case'i hetkehinnad ja vordleb neid failis talletatud algsete hindadega.

Kasum arvutatakse valemiga:

```text
(kogus * hetkehind) - (kogus * algne hind)
```

Kogused on koodis hetkel fikseeritud:

- Glove Case: `43`
- Spectrum 2 Case: `83`

## Failid

- [`Calculator/brain.py`](/C:/VAATA-KUI-JULGED/Calculator/brain.py) sisaldab kogu rakenduse loogikat ja Tkinter GUI-d.
- [`Calculator/initial_prices.txt`](/C:/VAATA-KUI-JULGED/Calculator/initial_prices.txt) salvestab esmakordsel kaivitamisel baas-hinnad.
- [`Calculator/profit_log.txt`](/C:/VAATA-KUI-JULGED/Calculator/profit_log.txt) kogub kuupaevapohist kasumi/kahjumi ajalugu.
- [`Calculator/dist/brain.exe`](/C:/VAATA-KUI-JULGED/Calculator/dist/brain.exe) on kompileeritud Windowsi build.
- [`Calculator/build`](/C:/VAATA-KUI-JULGED/Calculator/build) sisaldab build-artifakte.

## Kaivitamine

Noued:

- Python 3
- `requests`
- Windowsi Pythonis olemasolev `tkinter`

Kaivita:

```powershell
cd C:\VAATA-KUI-JULGED\Calculator
python brain.py
```

Kui `requests` puudub:

```powershell
pip install requests
```

Alternatiivina saad kasutada valmis faili [`Calculator/dist/brain.exe`](/C:/VAATA-KUI-JULGED/Calculator/dist/brain.exe).

## Rakenduse kaitumine

Rakendus teeb kaivitamisel kohe esimese hinnauuenduse.

Nupud:

- `Uuenda andmeid` kusib uued hinnad ja lisab uue rea logisse.
- `Vaata logi` avab eraldi aknas kogu `profit_log.txt` sisu.

Oluline detail:

- kui `initial_prices.txt` puudub, salvestatakse esimene edukalt loetud hind algseks vordlushinnaks;
- iga uuendus kirjutab logisse uue kirje, isegi siis kui rakendus kaivitati samal paeval mitu korda;
- kui Steamist hinda katte ei saada, kuvatakse tekst `Hinnad puuduvad`.

## Tuupiline workflow

1. Kaivita rakendus.
2. Lase esimesel edukal paringul algsed hinnad salvestada.
3. Vajuta hiljem `Uuenda andmeid`, et vorrelda praeguseid hindu baasiga.
4. Ava logi, et naha ajalugu kuupaevade kaupa.

## Markused

- Rakendus eeldab internetiuhendust.
- Hindade formaat on euro-pohine ja kood eeldab vastavat stringi puhastamist.
- Mones olemasolevas failis on margikodeering katki lainud ja euro symbol voib kuvada valesti.
- `FILENAME = "case_prices.txt"` on koodis defineeritud, kuid seda faili hetkel tegelikult ei kasutata.

## Kiire parendusnimekiri

- tosta kogused ja jalgitavad case'id seadistusfaili;
- lisa veateated Steam API timeout'ide jaoks;
- paranda UTF-8 margikodeering kogu projektis;
- lisa `requirements.txt`;
- lisa voimalus valida case'e GUI-st, mitte ainult koodis.
