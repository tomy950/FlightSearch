# Flight Search Application

Ovo je Flight Search aplikacija razvijena koristeći .NET 6 i Blazor WebAssembly. Aplikacija omogućava korisnicima da pretražuju ponude letova temeljem različitih kriterija poput polaznog i odredišnog aerodroma, datuma polaska i povratka, broja putnika te valute.

## Pokretanje Projekta

Slijedite korake ispod za pokretanje aplikacije lokalno:

### Preduvjeti

- .NET 6 SDK
- Visual Studio 2022

### Konfiguracija
Dodavanje Connection Stringa:

U appsettings.json datoteci unutar FlightSearchApi projekta, dodajte ili uredite connection string za vašu lokalnu bazu podataka. Na primjer:


{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FlightSearchDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}



Nakon pokretanja automatski se kreira baza i primjenjuju se migracije.
