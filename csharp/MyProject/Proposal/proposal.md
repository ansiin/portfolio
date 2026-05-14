# Investing Tracker

## Eesmärk

Selle projekti eesmärk on teha isiklik investeeringute jälgimise süsteem, mis koondab portfellid, varad, tehingud, hinnad, märkmed ja jälgimisnimekirjad ühte kohta. Projekt vastab Assignment 3 Phase 1 nõuetele: ASP.NET Core backend, versioneeritud REST API, JWT autentimine, MVC kliendi-UX, MVC admin-UX, eraldi JS klient, i18n nii UI-s kui andmebaasis ning deploy-valmidus.

## Phase 1 ulatus

Phase 1 fookus ei ole täisfunktsionaalne finantsplatvorm, vaid korralik töötav MVP, mille järgi on võimalik tõestada:

- päris domeen vähemalt 10 sisulise andmebaasiüksusega
- oma REST API kasutamine nii Swaggerist kui eraldi kliendist
- kasutajapõhine andmeomand ja IDOR kaitse
- admini kaudu hallatavad lookup-andmed
- eraldi deployitav backend ja eraldi deployitav veebiklient

Phase 1 teadlikult välja jäetud või järgmisse etappi lükatud teemad:

- automaatne market data import
- `CorporateAction` täisfunktsionaalsus
- täiscoverage integration/e2e testide komplekt
- keerukad background job’id

## Põhifunktsioonid

Lõppkasutaja saab:

- sisse logida ja välja logida
- kasutada JWT + refresh token voogu
- luua ja hallata portfelle
- luua ja hallata varasid
- sisestada tehinguid
- vaadata dashboardi koondandmeid
- pidada watchlist’e
- lisada märkmeid
- hallata hinnasnapshote ja positsioonisnapshote

Admin saab:

- hallata valuutasid
- hallata varatüüpe
- hallata börse
- hallata market data providereid
- vaadata kasutajate ja rollide ülevaadet

## Domeen

Phase 1 domeen koosneb järgmistest peamistest üksustest:

- `AppUser`
- `Portfolio`
- `Asset`
- `Transaction`
- `TransactionFee`
- `PriceSnapshot`
- `PositionSnapshot`
- `Watchlist`
- `WatchlistItem`
- `Tag`
- `AssetTag`
- `Note`
- `Currency`
- `Exchange`
- `AssetType`
- `MarketDataProvider`

See ületab Assignment 3 miinimumnõude selgelt.

## Arhitektuur

Projekt on tehtud praktilise kihilise lahendusena:

- `App.Domain` hoiab domeenimudelit
- `App.DAL.EF` hoiab EF Core andmekihti ja migratsioone
- `App.BLL` hoiab äriloogikat ja ownership-kontrolle
- `App.DTO` hoiab public DTO-sid
- `WebApp` hostib MVC kliendi, MVC admini ja REST API
- `App.Tests` hoiab unit-teste
- `MyProjectUI` on eraldi staatiline JavaScript klient

Täielikku Clean/Onion arhitektuuri ei rakendata, sest Assignment 3 seda ei nõua.

## API

REST API kasutab versioneeritud route’i kujul `/api/v1/...`.

Põhiotspunktid:

- `identity/account`
- `Portfolios`
- `Assets`
- `Transactions`
- `Dashboard`
- `Watchlists`
- `Notes`
- `PriceSnapshots`
- `PositionSnapshots`
- lookup-otspunktid admini ja kliendi jaoks

API tagastab DTO-sid, mitte EF entiteete.

## Turve

Projektis on kaks kriitilist turvenõuet:

- JWT + refresh token autentimine
- IDOR kaitse, kus kasutaja saab REST API kaudu ligi ainult enda andmetele

Ownership otsused tehakse serveris autentitud kasutaja põhjal.

## Lokaliseerimine

Projektis on kaks eraldi i18n kihti:

- UI tekstid `.resx` failidest
- andmebaasi mitmekeelsed väljad `LangStr` kaudu

Toetatud keeled on vähemalt:

- `en`
- `et`

## Eraldi klient

Kuna projekt peab sisaldama oma backendiga töötavat eraldi klienti, on Phase 1 sees ka eraldi JS rakendus, mis kasutab:

- login/logout
- refresh token flow
- CRUD tehteid vähemalt 3 üksuse vastu

Praeguses scope’is katab see vähemalt:

- portfellid
- varad
- tehingud

## Deploy

Deploy siht on VPS, kus:

- backend töötab eraldi konteineris
- andmebaas töötab eraldi konteineris
- JS klient töötab eraldi veebikonteineris ja eraldi URL/porti taga
- CORS on backendis seadistatud

CI/CD jaoks on projektis build/test workflow ja eraldi deploy workflow.

## Kokkuvõte

See proposal on nüüd joondatud Assignment 3 Phase 1 reaalse scope’iga. Eesmärk ei ole ehitada korraga kogu tulevane investing platvorm, vaid teha korrektne, turvaline ja deployitav esimene versioon, mille peal saab järgmistes etappides edasi ehitada.
