# FVD Parts Manager

**Webbasierte Lagerverwaltung fГјr Ersatzteile вЂ“ entwickelt mit ASP.NET Core 10, Blazor, Entity Framework Core und PostgreSQL 18.**

Dieses Projekt ist eine eigenstГ¤ndig entwickelte Demo-Anwendung zur Verwaltung von Ersatzteilen und Lagerbewegungen. Es demonstriert die Umsetzung typischer ERP-Funktionen вЂ“ von der Datenmodellierung und GeschГ¤ftslogik bis zur Bereitstellung auf einem Linux-Server.

## Live-Demo

**[FVD Parts Manager online Г¶ffnen](https://fvd-partsmanager.178.105.105.251.sslip.io/)**

Die Anwendung lГ¤uft auf einem Hetzner-Server und ist Гјber HTTPS erreichbar. Der Zugriff ist durch HTTP Basic Authentication geschГјtzt. Die Zugangsdaten werden auf Anfrage zur VerfГјgung gestellt.

## Screenshots

### Dashboard

![Dashboard](docs/screenshots/01-dashboard.png)

Гњbersicht Гјber Lagerkennzahlen, kritische BestГ¤nde und aktuelle Lagerbewegungen.

### Artikelverwaltung

![Artikelverwaltung](docs/screenshots/02-artikelverwaltung.png)

Verwaltung von Ersatzteilen mit Artikelnummer, Bezeichnung, Lagerbestand und Mindestbestand.

### Lagerbewegungen

![Lagerbewegungen](docs/screenshots/03-lagerbewegungen.png)

Erfassung und Гњbersicht von WareneingГ¤ngen und WarenausgГ¤ngen.

## Funktionen

- **Dashboard:** Aktuelle Lagerkennzahlen, Mindestbestandswarnungen und letzte Lagerbewegungen.
- **Artikelverwaltung:** Artikel anlegen, anzeigen, bearbeiten und lГ¶schen.
- **Lagerbewegungen:** WareneingГ¤nge und WarenausgГ¤nge erfassen und dokumentieren.
- **BestandsfГјhrung:** Automatische Aktualisierung der LagerbestГ¤nde bei Buchungen.
- **DatenintegritГ¤t:** Transaktionsbasierte Verarbeitung von Lagerbewegungen.
- **Demo-Daten:** Reproduzierbare Initialisierung mit Beispielartikeln und Lagerbewegungen.
- **Datenbankmigrationen:** Versionsverwaltung des Datenbankschemas mit EF Core Migrations.

## Technologie-Stack

| Bereich | Technologie |
|---|---|
| Backend | C#, .NET 10, ASP.NET Core |
| BenutzeroberflГ¤che | Blazor Web App, Razor Components, Bootstrap |
| Datenzugriff | Entity Framework Core 10 |
| Datenbank | PostgreSQL 18 |
| Containerisierung | Docker, Docker Compose |
| Webserver / Reverse Proxy | Nginx |
| HTTPS | Let's Encrypt / Certbot |
| Hosting | Hetzner Cloud, Ubuntu Linux |
| Versionsverwaltung | Git / GitHub |

## Architektur

Die Anwendung ist in mehrere Schichten gegliedert:

- **Components:** Blazor-Komponenten fГјr die BenutzeroberflГ¤che.
- **Services:** GeschГ¤ftslogik fГјr Artikelverwaltung und Lagerbewegungen.
- **Models:** Datenmodelle fГјr Artikel und Lagerbewegungen.
- **Data:** Entity Framework Core DbContext, Migrationen und Demo-Daten.

Die Kommunikation mit der Datenbank erfolgt Гјber Entity Framework Core und den PostgreSQL-Provider.

### Deployment-Architektur

```text
Browser
   |
   | HTTPS (443)
   v
Nginx + Let's Encrypt
   |
   | HTTP (localhost:8090)
   v
Docker: ASP.NET Core / Blazor
   |
   | Internes Docker-Netzwerk
   v
Docker: PostgreSQL 18
   |
   v
Persistentes Docker Volume
```

Die PostgreSQL-Datenbank ist nicht direkt Гјber einen Г¶ffentlichen Port erreichbar.

## Lokale Entwicklung

Voraussetzungen:

- .NET SDK 10
- PostgreSQL
- Eine Entwicklungsumgebung wie Visual Studio oder Visual Studio Code

Repository klonen:

```bash
git clone https://github.com/VitaliiVarshko/FVD.PartsManager.git
cd FVD.PartsManager
```

Die lokale PostgreSQL-Verbindung Гјber die Konfiguration oder .NET User Secrets einrichten. Zugangsdaten dГјrfen nicht im Repository gespeichert werden.

Anwendung starten:

```bash
dotnet restore
dotnet run --project src/FVD.PartsManager.Web
```

## Deployment mit Docker Compose

Die Datei `compose.yaml` definiert folgende Dienste:

| Dienst | Aufgabe |
|---|---|
| `db` | PostgreSQL-18-Datenbank |
| `web` | ASP.NET-Core-/Blazor-Anwendung |
| `migrate` | EF-Core-Datenbankmigrationen |
| `seed` | Initialisierung der Demo-Daten |

Vor dem Start muss die Umgebungsvariable `FVD_DB_PASSWORD` gesetzt werden, beispielsweise Гјber eine lokale `.env`-Datei, die nicht versioniert wird.

Datenbank starten:

```bash
docker compose up -d db
```

Datenbankmigrationen anwenden:

```bash
docker compose run --rm migrate
```

Demo-Daten initialisieren:

```bash
docker compose run --rm seed
```

Webanwendung starten:

```bash
docker compose up -d web
```

Der Webdienst ist standardmГ¤Гџig unter `127.0.0.1:8090` erreichbar. FГјr den Г¶ffentlichen Zugriff wird ein separater Reverse Proxy mit HTTPS verwendet.

## Sicherheit

- Keine PasswГ¶rter oder ZugangsschlГјssel im Repository.
- Datenbankzugriff ausschlieГџlich Гјber das interne Docker-Netzwerk.
- Webcontainer wird ohne Root-Rechte ausgefГјhrt.
- HTTPS-Terminierung Гјber Nginx.
- Passwortschutz fГјr die Г¶ffentliche Demo Гјber Nginx.
- Persistente Docker Volumes fГјr Datenbank und ASP.NET Core Data Protection Keys.

## Projektziel

Das Projekt wurde als praxisnahes Portfolio-Beispiel entwickelt, um Kenntnisse in der **Entwicklung datenbankgestГјtzter Webanwendungen**, in der **ERP-GeschГ¤ftslogik** sowie im **Deployment moderner .NET-Anwendungen unter Linux** zu demonstrieren.

Es handelt sich um ein unabhГ¤ngiges Demonstrationsprojekt und nicht um ein offizielles Produkt der FVD Brombacher GmbH.

## Autor

**Vitalii Varshko**

Softwareentwickler mit Schwerpunkt Backend-Entwicklung, ERP-Integration und datenbankgestГјtzte GeschГ¤ftsanwendungen.
