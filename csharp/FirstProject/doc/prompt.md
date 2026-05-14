Kirjuta ja loo taielik ASP.NET Core MVC projekt nimega "Multi-Tenant Dental Practice SaaS", kasutades .NET 8 (voi uusim LTS).

Tee lahendus CLEAN CODE pohimotetel:
- loetav ja uhtlane koodistiil
- SOLID, DRY
- selge vastutuse jaotus kihtide vahel
- vaikesed meetodid
- selged nimed
- ariloogika ei tohi olla Controllerites
- ara loo spagetikoodi

## 1. Eesmark
Soovin tootmiskolbliku arhitektuuriga SaaS rakendust hambaravikabinettidele, kus iga tenant (Company) on rangelt isoleeritud, rollipohine turve tootab tenantipohiselt ning subscription-tierid mojutavad funktsionaalsust nii UI kui serveri tasemel.

## 2. Tehnoloogiline raamistik
Kohustuslik tehniline stack:
- ASP.NET Core MVC
- EF Core + migrations
- ASP.NET Core Identity
- xUnit (vahemalt Application layer unit-testid)

## 3. Arhitektuuri nouded (kihiline)
Rakenda selge kihiline arhitektuur ning kirjelda see README-s.

Kihid:
- UI layer: Controllers, Views, ViewModels, Filters
- Application/BLL layer: ariloogika, teenused, use-case'id, validatsioon
- Infrastructure/DAL layer: EF Core, repositories (voi Unit of Work), migrations, tenant filterid
- Domain layer: entity'd, value object'id, enumid, domainireeglid
- Cross-cutting: logging, audit trail, soft delete, feature flags, authorization policies

Soovituslik solution struktuur (void paremaks muuta, kuid kihid peavad jaama selgeks):
- DentalSaaS.Web
- DentalSaaS.Application
- DentalSaaS.Domain
- DentalSaaS.Infrastructure
- DentalSaaS.Shared

## 4. Multi-tenancy ja andmete isolatsioon (kohustuslik)
Rakenda path-based tenant routing:
- dentalsaas.io/{tenantSlug}
- naited: /{tenantSlug}/patients, /{tenantSlug}/appointments

Andmete isolatsioon:
- koigil tenant-andmetel on CompanyId
- koik EF Core paringud peavad automaatselt filtreerima CompanyId jargi (Global Query Filters voi interceptorid)
- cross-tenant lekked on keelatud
- admin/support erandid ainult selgelt lubatud teedel ja kontrollidega

Onboarding/signup:
- self-service signup loob uue Company + CompanyOwner kasutaja
- slug peab olema unikaalne (nt acme-dental)
- loo algandmed: CompanySettings + vaikimisi Subscription = Free

Soft delete:
- business entity'dele hard delete keelatud
- kasuta valju: IsDeleted, DeletedAt, DeletedBy
- query filter peidab soft-deleted read

Audit trail:
- logi "kes muutis mida ja millal" tenantipohiselt
- loo AuditLog tabel valjadega:
  - EntityName
  - EntityId
  - Action
  - OldValues
  - NewValues
  - ChangedAt
  - ChangedByUserId
  - CompanyId

## 5. Identity ja autoriseerimine (kohustuslik)
Kasuta ASP.NET Core Identity.

Nouded:
- kasutaja voib kuuluda mitmesse company'sse
- rollipohine autoriseerimine ([Authorize(Roles="...")]) voi policy'd
- rollid peavad eksisteerima:
  - System-level: SystemAdmin, SystemSupport, SystemBilling
  - Company-level: CompanyOwner, CompanyAdmin, CompanyManager, CompanyEmployee

Selgita ja implementeeri:
- kuidas user valib aktiivse tenant'i (URL slug + membership check)
- kuidas oiguseid kontrollitakse tenant'i sees
- SystemAdmin impersonation tugi (toeks), koos audit log kirjega

Lisa standard auth vood:
- login
- register
- password reset

## 6. Subscription tiers ja feature gating (kohustuslik)
Subscription tasemed:
- Free
- Standard
- Premium

Rakenda feature flags voi limiidid, nt:
- max Patients / Users / Rooms
- Insurance moodul ainult Standard/Premium
- Payment Plan ainult Premium

Kohustuslik:
- UI peidab keelatud funktsioonid
- server kontrollib ligipaasu pariselt (ainult UI peitmisest ei piisa)

## 7. Domeeni reeglid (Dental SaaS)
Iga Company haldab:
- patsiendid
- raviprotseduurid
- ruumid
- kindlustus/billing

Practice seadistab:
- treatment rooms
- appointment types (kestus, hind)

Patsiendi kaart:
- universal tooth numbering 1..32 (adult)
- per-tooth condition status
- treatment history
- X-ray tracking + konfigureeritav intervall

Treatment plans:
- mitme-appointment plaanid
- itemized costs
- sequencing
- urgency levels

Patient approval workflow:
- patsient saab plaani itemeid Accept voi Defer
- acceptance tracking mojutab schedulingut ja revenue forecastingut

Insurance:
- per-country seadistatavad nouded
- statutory vs private coverage
- legal format cost estimate (nt Saksamaa "Kostenvoranschlag")
- claim submission + tracking

Payment plans:
- kallite protseduuride osamaksete plaanid
- tingimused peavad olema konfigureeritavad

Rollide too:
- CompanyEmployee: appointments + basic records
- CompanyManager: clinical decisions + treatment plans
- CompanyAdmin: practice config + insurance relationships

## 8. Noutud entity'd ja seosed (kohustuslik)
Kasuta vahemalt jargnevaid entity'sid (void lisada juurde):
- Company
- CompanySettings
- Patient
- ToothRecord
- Treatment
- TreatmentType
- Appointment
- TreatmentPlan
- PlanItem
- Xray
- InsurancePlan
- CostEstimate
- Invoice
- PaymentPlan
- Dentist
- TreatmentRoom
- Subscription
- AppUser
- AppUserRole

Igal business entity'l peavad olema:
- Id
- CompanyId
- audit valjad
- soft delete valjad

Defineeri seosed (1:N, N:M) selgelt ning dokumenteeri.

## 9. UI moodulid (MVC)
Rakenda vahemalt need moodulid:
- Company onboarding: tenant + owner registreerimine
- Patients: CRUD + tooth chart vaade (lihtne tabel 1..32)
- Appointments: calendar-like list (nadal/kuu list sobib)
- Treatment Types & Rooms: CRUD (CompanyAdmin)
- Treatment Plans: plaani loomine, plan item'id, urgency, sequencing, totals
- Patient approval: Accept/Defer per plan item
- Insurance: insurance plans CRUD, cost estimate (country template), basic claim flow
- Invoices + Payment Plans (subscription-gated)

Kohustuslik arhitektuurireegel:
- iga controller kasutab Application/BLL teenuseid
- controller ei tohi kasutada DbContexti otse

## 10. Tehnilised kvaliteedinouded
- EF Core migrations
- rollide ja algandmete seedimine
- DataAnnotations + server-side validatsioon BLL-is
- uhtne error page + TempData/toast tagasiside
- struktureeritud logging
- vahemalt Application layer unit-testid (xUnit)
- README peab katma:
  - setup
  - arhitektuur
  - tenant routing
  - auth flow
  - subscription gating

## 11. CLEAN CODE standard (kohustuslik)
- ara kirjuta pikki meetodeid
- ara dubleeri loogikat
- kasuta ViewModel-e, mitte entity'sid otse Viewdes
- ara pane ariloogikat controllerisse
- kasuta selgeid nimesid, valdi magic stringe
- authorization kontrollid peavad olema tsentraliseeritud (policy/service)
- tenant resolution peab olema tsentraalne (middleware/filter/service)

## 12. Kohustuslikud naidisdeliverable'id, mis peavad vastuses olemas olema
Tagasta vahemalt:
1. solution struktuur (kaustad/projektid)
2. peamised klassid (TenantResolver, CurrentTenant, authorization policies, services)
3. DbContext + Global Query Filters naide (CompanyId + soft delete)
4. naidis: Patients CRUD (Controller + Service + Repository + ViewModels)
5. naidis: TreatmentPlan approval workflow loogika (BLL)
6. seedimine rollide ja subscription tieride jaoks
7. README mustand

## 13. Vastuse formaat
Esita valjund struktureeritult:
- koigepealt arhitektuuri ulevaade
- siis projekti struktuur
- siis votmeklasside ja flow'de koodinaited
- siis seeding ja konfiguratsioon
- lopus README mustand

## 14. Keelatud lihtsustused / kriitilised piirangud
- ara jata kriitilist funktsionaalsust TODO-na
- kui lihtsustad, selgita tapselt kuidas ja miks
- lihtsustused ei tohi rikkuda:
  - multi-tenancy isolatsiooni
  - role-based authorization'i
  - server-side subscription gatingut
