# Lõpuplaan

## Eesmärk

Core Assignment 3 töö on tehtud. Selle faili eesmärk ei ole enam kirjeldada nullist arendusjärjekorda, vaid lukustada allesjäänud viimased tööd, mis viivad projekti esitluse ja päris deploy seisukohast täiesti valmis kujule.

## Prioriteet 1: deploy päriselt üles

Teha VPS poolel ära järgmine:

1. klooni repo serverisse
2. loo serverisse `.env.docker`
3. kontrolli reverse proxy / domeenid või vähemalt eraldi pordid
4. käivita `docker compose --env-file .env.docker up -d --build`
5. testi backend URL ja eraldi client URL läbi
6. lülita GitHub `deploy.yml` tööle päris secretitega

Valmiskriteerium:

- backend töötab VPS-is
- client töötab VPS-is eraldi URL/porti taga
- client saab backendiga läbi CORS-i suhelda

## Prioriteet 2: testide laiendamine

Praegu on olemas unit-testid finantsarvutustele ja snapshoti loomisele. Järgmisena tasub lisada:

- integration test auth flow’le
- integration test ownership/IDOR kontrollidele
- vähemalt üks end-to-end test login + CRUD voo peale

Valmiskriteerium:

- kõige riskantsemad vood on automatiseeritult kaetud

## Prioriteet 3: järgmise etapi laiendus

Need ei ole enam Assignment 3 core plokid, vaid järgmine faas:

- `CorporateAction`
- market data automaatika
- import/export
- põhjalikumad raportid
- tugevam admini tõlkehaldus

## Soovitatud esitlusjärjekord

Kui projektitööd tuleb kaitsta või demo teha, näita seda järjekorras:

1. login ja language switch
2. portfolios
3. assets
4. transactions
5. dashboard valuation
6. watchlists ja notes
7. admin lookup management
8. Swagger
9. separate JS client
10. Docker / CI-CD / deploy workflow

## Hetkehinnang

Kui eesmärk on Assignment 3 Phase 1 läbimine ja repo esitlusvalmidus, siis projekt on valmis.
Allesjäänud töö ei ole enam core funktsionaalsuse ehitamine, vaid:

- päris VPS deploy live secretite ja serveri bootstrapiga
- lisatestid bonus-track tasemel
- järgmise etapi laiendused
