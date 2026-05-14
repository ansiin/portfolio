Kirjuta ja loo mulle täielik ASP.NET Core MVC (NET 8 või uusim LTS) projekt “Multi-Tenant Dental Practice SaaS”, mis järgib allolevaid nõudeid. Tee lahendus CLEAN CODE põhimõtetel: lihtne loetav kood, selged klassi- ja meetodinimed, SOLID, DRY, selge vastutuste jaotus, väikesed meetodid, ühtlane stiil. Ära tee “spagetti” ega koonda äriloogikat Controlleritesse.

1) Arhitektuur (kohustuslik)

Lisa projektile selge kihiline arhitektuur ja kirjuta see ka välja README-s:

UI layer: ASP.NET Core MVC (Controllers, Views, ViewModels, Filters)

BLL / Application layer: äriloogika, teenused (Services), use-case’id, validatsioon

DAL / Infrastructure layer: EF Core, Repositories (või Unit of Work), migrations, tenant query filterid

Domain layer: Entity-d, Value Object-id, enumid, domain reeglid

Cross-cutting: Logging, audit trail, soft delete, feature flags, authorization policies

Tee solution näiteks nii (võid paremaks muuta, aga kihid peavad olema selged):

DentalSaaS.Web (UI)

DentalSaaS.Application (BLL)

DentalSaaS.Domain

DentalSaaS.Infrastructure (DAL + Integrations)

DentalSaaS.Shared (util, constants, common models)

2) Multi-Tenancy (kohustuslik)

Rakenda path-based routing stiilis:

bikerental.io/acme analoog -> dentalsaas.io/{tenantSlug}
Näiteks: /acme/patients, /acme/appointments, jne.

Data isolation:

Kõigil tenant-andmetel on CompanyId

Kõik EF Core query’d peavad automaatselt filtreerima CompanyId järgi (Global Query Filters või interceptors)

Keelatud on cross-tenant lekked (sh admin/suppport režiimis ainult lubatud teed)

Onboarding / signup:

Self-service signup loob uue Company (tenant) + CompanyOwner kasutaja

Slug peab olema unikaalne (nt “acme-dental”)

Loo algandmed (CompanySettings, default Subscription Free)

Soft delete:

Ära tee hard delete’i business entity’dele

Kasuta IsDeleted, DeletedAt, DeletedBy

Query filter peidab kustutatud read

Audit trail:

Logi “who changed what and when” per tenant

Tee AuditLog tabel: EntityName, EntityId, Action, OldValues, NewValues, ChangedAt, ChangedByUserId, CompanyId

3) Identity & Authorization (kohustuslik)

Kasuta ASP.NET Core Identity.
Nõuded:

Users võivad kuuluda mitmesse company’sse (nt sama inimene töötab mitmes kliinikus)

Role-based authorization [Authorize(Roles="...")] või policy’d

Rollid peavad eksisteerima:

System-level: SystemAdmin, SystemSupport, SystemBilling

Company-level: CompanyOwner, CompanyAdmin, CompanyManager, CompanyEmployee

Selgita ja implementeeri:

Kuidas user valib aktiivse tenant’i (URL slug + membership check)

Kuidas kontrollitakse õiguseid tenant’i sees

SystemAdmin impersonation: SystemAdmin saab “impersonate” company userit (toeks), jälgi audit logis

Lisa standard auth vood:

Login, register, password reset

4) Subscription tiers & feature access (kohustuslik)

Tee subscription mudel:

Free, Standard, Premium
Mõju:

Feature flags või limiidid (näiteks: max Patients/Users/Rooms; Insurance moodul ainult Standard/Premium; PaymentPlan ainult Premium)

Rakenda nii, et UI peidab funktsiooni, aga server pool päriselt kontrollib.

5) Domeeni kirjeldus (Dental SaaS)

Platvorm teenindab hambaravikabinette üle maailma. Iga practice (Company) haldab:

patsiendid, raviprotseduurid, ruumid, kindlustus/billing

Practice seadistab:

Treatment rooms

Appointment types: kestus, hind

Patsiendi kaart:

Universal tooth numbering 1–32 (adult)

per-tooth condition status ja treatment history

X-ray tracking + configurable interval requirements

Treatment plans:

Mitme-appointment plaanid, itemized costs, sequencing, urgency levels

Patient approval workflow: patsiendid võivad nõustuda urgent töödega ja lükata elective kallid tööd edasi

Plan acceptance tracking -> scheduling ja revenue forecasting

Insurance integration:

per-country configurable requirements

statutory vs private coverage

cost estimate generation legally required formats (nt “Kostenvoranschlag” Germany)

claim submission & tracking

payment plans kallitele protseduuridele (installments, configurable terms)

Rollide töö:

CompanyEmployees (assistendid/hügienistid): manage appointments, basic records

CompanyManagers (dentists): clinical decisions, treatment plans

CompanyAdmins: practice config + insurance relationships

6) Entity-d (kohustuslik)

Kasuta järgnevaid entity’sid (võid lisada juurde):
Company, CompanySettings, Patient, ToothRecord, Treatment, TreatmentType, Appointment, TreatmentPlan, PlanItem, Xray, InsurancePlan, CostEstimate, Invoice, PaymentPlan, Dentist, TreatmentRoom, Subscription, AppUser, AppUserRole

Igal business entity’l: Id, CompanyId, audit väljad, soft delete väljad.
Defineeri vajalikud seosed (1:N, N:M) selgelt ja dokumenteeri.

7) UI funktsionaalsus (MVC)

Tee vähemalt need moodulid:

Company onboarding: register tenant + owner

Patients: CRUD + tooth chart vaade (lihtne tabel 1..32)

Appointments: calendar-like list (lihtne nädal/kuu list sobib)

Treatment Types & Rooms: CRUD (CompanyAdmin)

Treatment Plans: create plan, add plan items, set urgency, sequencing, estimate totals

Patient approval: Accept/Defer per plan item

Insurance: Insurance plans CRUD, create CostEstimate (country template), create claim (basic)

Invoices + Payment Plans (subscription gated)

Iga controller peab kasutama teenuseid BLL kihist, mitte otse DbContexti.

8) Tehnilised nõuded

EF Core + migrations

Seed rollid ja vajalikud algandmed

Validation: DataAnnotations + server-side checks BLL-is

Error handling: ühtne error page + tempdata/toast message’d

Logging: struktureeritud logid

Tests: vähemalt Application layer unit-testid (xUnit)

README: setup, arhitektuur, tenant routing, auth flow, subscription gating

9) CLEAN CODE nõuded (kohustuslik)

Ära kirjuta pikki meetodeid

Ära dubleeri loogikat

Kasuta ViewModel-e, mitte entity’sid otse view’s

Ära pane äriloogikat controllerisse

Selged nimetused, vähe “magic stringe”

Authorization kontrollid peavad olema keskseks tehtud (policy/service)

Tenant resolution peab olema keskne (middleware/filter/service)

10) Väljund, mida ma tahan sinult

Anna mulle:

Solution struktuur (kaustad/projektid)

Peamised klassid (TenantResolver, CurrentTenant, Authorization policies, Services)

DbContext + Global Query Filters näide CompanyId ja SoftDelete jaoks

Näidis: Patients CRUD (Controller + Service + Repository + ViewModels)

Näidis: TreatmentPlan approval workflow loogika (BLL)

Seeding rollide ja subscription tieride jaoks

README mustand

Ära jäta “TODO” kohtadesse kriitilist funktsionaalsust. Kui midagi lihtsustad, selgita täpselt kuidas ja miks, aga ära riku multi-tenancy isolation’i ega role-based authorization’it.