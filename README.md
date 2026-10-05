# WhereTo.Api (ASP.NET Core 8 + SQLite + Swagger)

Pakeičia `server/` (Node). Duomenų bazė – SQLite failas `whereto.db`

## Paleidimas

```
cd WhereTo
dotnet build
dotnet run
```

Pirmo paleidimo metu automatiškai sukuriamas `whereto.db`, lentelės ir pavyzdiniai duomenys

- API: http://localhost:4000

## REIKALAVIMAI

Plėtinys **SQLite Viewer**

## Duomenų atstatymas

Ištrinti db failą ir vėl paleisti programą iš naujo su dotnet build, tada run.