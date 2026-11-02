# WhereTo


## Paleidimas

```
cd WhereTo
dotnet build
dotnet run
```

Pirmo paleidimo metu automatiškai sukuriamas `whereto.db`, lentelės ir pavyzdiniai duomenys

- API: http://localhost:4000

## Autentifikacija

`POST /api/accounts/login` grąžina trumpai galiojantį access tokeną ir 30 dienų
refresh tokeną. Pasibaigus access tokeno galiojimui, siųskite refresh tokeną į
`POST /api/accounts/refresh`; senas refresh tokenas bus rotuojamas ir nebegalės
būti panaudotas dar kartą. Atsijungimui naudokite `POST /api/accounts/logout`.

## Reikalavimai

Plėtinys **SQLite Viewer** - lengvesnei duomenų bazės peržiūrai.

## Duomenų atstatymas

Ištrinti .db failą ir vėl paleisti programą iš naujo su dotnet build, tada dotnet run.
