# Projekti hetkeseis

## Kokkuvõte

Projekt on praegu töötav Investing Tracker MVP, mis vastab Assignment 3 Phase 1 põhituumale. Core backend, MVC kliendi- ja admini vaated, eraldi JS klient, JWT autentimine, DTO-põhine versioneeritud API, `LangStr`-põhine DB i18n, `.resx`-põhine UI i18n ning Docker/CI/CD baas on olemas.

13.04.2026 seisuga kontrollitud:

- `dotnet build HWDemo.sln -v:minimal` -> roheline
- `dotnet test App.Tests/App.Tests.csproj -v:minimal --no-build` -> 3 testi, kõik läbisid
- `node --check ../MyProjectUI/app.js` -> roheline

## Tehniline stack

- `.NET 8`
- ASP.NET Core MVC
- ASP.NET Core Identity
- JWT Bearer autentimine
- refresh tokenid
- EF Core
- PostgreSQL
- Swagger / OpenAPI
- Razor Views
- eraldi staatiline JavaScript klient
- Docker / Docker Compose
- GitHub Actions

## Solution struktuur

- `WebApp`
  Host-projekt. Sisaldab MVC kliendi, MVC admini, REST API kontrollerid, authi, Swaggeri ja runtime konfiguratsiooni.

- `App.Domain`
  Domeenientiteedid ja enumid.

- `App.DAL.EF`
  `AppDbContext`, migratsioonid, seedimine.

- `App.BLL`
  Teenused ja äriloogika.

- `App.DTO`
  Public DTO-d.

- `App.Resources`
  `.resx` UI ressursid.

- `App.Tests`
  Unit-testid finantsarvutuste ja snapshot loogika jaoks.

- `MyProjectUI`
  Eraldi frontend, mis tarbib backendi REST API-t.

## Domeen

Praegune investing tracker domeen sisaldab vähemalt järgmisi sisulisi üksusi:

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

Lisaks on olemas `AppUser`, `AppRole` ja `AppRefreshToken`.

## Auth ja turve

Olemas:

- JWT access token
- refresh token roteerimine
- logout/revoke flow
- `UseAuthentication()` request pipeline’is
- API ownership-kontrollid portfellide, varade, tehingute, watchlistide, märkmete, hinnasnapshotide ja positsioonisnapshotide vastu

Oluline tulemus:

- kasutaja ei saa REST API kaudu teise kasutaja andmeid lugeda ega muuta

Legacy `ListItem` domeeniüksus ja tabel on jäänud andmemudelisse ainult vana algse migratsiooni
järjepidevuse hoidmiseks. Aktiivne MVC ja REST API pind selle ümber on eemaldatud ning see ei kuulu
enam Assignment 3 scope’i.

## REST API

Olemasolevad versioneeritud API otspunktid:

- `/api/v1/identity/account/*`
- `/api/v1/Portfolios`
- `/api/v1/Assets`
- `/api/v1/Transactions`
- `/api/v1/Dashboard`
- `/api/v1/Watchlists`
- `/api/v1/Notes`
- `/api/v1/PriceSnapshots`
- `/api/v1/PositionSnapshots`
- `/api/v1/Currencies`
- `/api/v1/AssetTypes`
- `/api/v1/Exchanges`
- `/api/v1/MarketDataProviders`

API kasutab public DTO-sid ja on Swaggeris dokumenteeritud.

## MVC kliendi-UX

Olemasolevad vood:

- home + language switcher
- dashboard
- portfolios
- assets
- transactions
- watchlists
- notes
- price snapshots
- position snapshots

Dashboard arvutab:

- portfellide arvu
- aktiivsete varade arvu
- tehingute arvu
- net cash flow
- buy/sell volume
- market value
- unrealized profit
- allocation snapshot
- monthly timeline

Valuation loogika kasutab viimast `PriceSnapshot` kirjet.

## MVC admin-UX

Admin ala on eraldi `Area` all ja kaitstud rolliga `admin`.

Olemas:

- admin home
- users overview
- currencies
- asset types
- exchanges
- market data providers

Admin ala on Assignment 3 nõude järgi viidud viewmodel-põhiseks. `ViewBag` / `ViewData` ei kasutata admini lehtede andmete edastamiseks.

## Eraldi JS klient

`MyProjectUI` on eraldi staatiline frontend.

Olemas:

- API base URL seadistamine
- login
- refresh token
- logout
- dashboard summary / timeline / allocation
- CRUD portfellidele
- CRUD varadele
- CRUD tehingutele

See katab Assignment 3 nõude, et eraldi klient kasutaks oma backendi REST API-t ja teeks CRUD-i vähemalt 3 üksuse vastu.

## Lokaliseerimine

### UI i18n

Olemas:

- `.resx` failid `en` ja `et` jaoks
- keelevalik UI-s
- tõlgitud ühised menüütekstid ja peamised investing tracker index/dashboard vaated

### DB i18n

Olemas:

- `LangStr`
- `jsonb` salvestus PostgreSQL-is
- mitmekeelsed lookup-väljad nagu `Currency.DisplayName`, `Exchange.DisplayName`, `AssetType.DisplayName`, `MarketDataProvider.DisplayName`

## Seedimine

Identity seedimine loob vaikimisi:

- `admin@taltech.ee` / `Kala.12345`
- `user@taltech.ee` / `Kala.12345`

Lookup-seemendus lisab baastaseme valuutad, asset type’id, börsid ja market data providerid.

## CI/CD ja deploy

Olemas:

- `.github/workflows/ci.yml`
  Build, test, JS süntaksikontroll, backend Docker image build, client Docker image build

- `.github/workflows/deploy.yml`
  SSH-põhine deploy workflow VPS-i jaoks

- `docker-compose.yml`
  backend + db + separate client container

- `.env.docker.example`
  runtime seadete näidis

Praegune deploy workflow eeldab, et:

- repo on serveris juba kloonitud
- `.env.docker` on serveris olemas
- Docker ja Docker Compose on VPS-is olemas

## Assignment 3 vastavus

Praegune hinnang:

- domain design, min 10 entityt: tehtud
- REST API + versioning + DTO-d: tehtud
- Swagger: tehtud
- JWT auth: tehtud
- MVC client UX: tehtud
- MVC admin UX area + protected + viewmodels: tehtud
- translations in UI (`.resx`): tehtud
- translations in DB (`LangStr`): tehtud
- IDOR kaitse REST API-s: tehtud
- separate client app: tehtud
- client kasutab JWT + refresh token flow’d: tehtud
- CRUD vähemalt 3 entity vastu eraldi kliendis: tehtud
- backend/client deploy baas: tehtud

## Teadlikud piirangud

Need ei bloki Assignment 3 core nõuet, aga tasub välja öelda:

- `CorporateAction` ei ole Phase 1-s realiseeritud
- osa frameworki vaikimisi valideerimistekste võib jääda ingliskeelseks
- deploy workflow on olemas, aga vajab päris VPS keskkonnas bootstrap’i ja secretite seadistamist
- bonus track integration/e2e testid puuduvad

## Järeldus

Projekt ei ole enam skeleton ega demo-CRUD. See on töötav Assignment 3 Phase 1 investing tracker, millel on olemas backend, kaks MVC UX kihti, eraldi JS klient, i18n, ownership-kaitse ja deploy baas. Alles jäänud töö on peamiselt polish ja järgmise etapi laiendused, mitte core nõuete nullist ehitamine.
