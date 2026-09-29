# WhereTo.Api (ASP.NET Core 8 + SQLite + Swagger)

Pakeičia `server/` (Node). Duomenų bazė – SQLite failas `whereto.db`: jokio MySQL / XAMPP nereikia.

## Paleidimas

```
cd WhereTo.Api
dotnet run
```

Pirmo paleidimo metu automatiškai sukuriamas `whereto.db`, lentelės ir pavyzdiniai duomenys
(3 organizacijos, 3 artėjantys renginiai).

- API: http://localhost:4000
- Swagger UI: http://localhost:4000/swagger

## DB peržiūra VS Code

Įdiek plėtinį **SQLite Viewer** (`qwtel.sqlite-viewer`) ir tiesiog spustelėk `whereto.db` Explorer'yje.
(Alternatyva: **SQLite** by alexcvzz – `Ctrl+Shift+P` → `SQLite: Open Database`, kad galėtum vykdyti SQL užklausas.)

## Duomenų atstatymas

Sustabdyk API ir ištrink `whereto.db` (+ `whereto.db-shm`, `whereto.db-wal`, jei yra) – kitą kartą jis bus
sukurtas iš naujo. Tai reikalinga ir tada, kai pakeiti modelius (`Models/`), nes `EnsureCreated`
senos DB schemos neatnaujina.

## Endpoint'ai

| Metodas | Kelias | Aprašymas |
|---|---|---|
| GET | /api/health | Ar API veikia |
| GET | /api/organizations | Visos organizacijos |
| GET | /api/organizations/{id} | Viena organizacija |
| POST | /api/organizations | Sukurti |
| PUT | /api/organizations/{id} | Atnaujinti |
| DELETE | /api/organizations/{id} | Ištrinti |
| GET | /api/events | Artėjantys renginiai |

## Struktūra

```
Controllers/  HTTP endpoint'ai (su /// komentarais -> Swagger aprašymai)
Dtos/         Request/Response modeliai (validacija čia)
Models/       EF entity'ai
Data/         AppDbContext + seeder'iai
Program.cs    DI, CORS, Swagger, DB sukūrimas
```
