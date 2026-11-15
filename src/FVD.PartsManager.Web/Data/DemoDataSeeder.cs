
using FVD.PartsManager.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FVD.PartsManager.Web.Data;

public static class DemoDataSeeder
{
    private const string DemoHinweis =
        "DEMO-DATEN: Fiktiver Lagerbestand, kein echter FVD-Bestand.";

    private record DemoArtikel(
        string Nummer,
        string Bezeichnung,
        int Mindestbestand,
        int Anfangsbestand);

    private static readonly DemoArtikel[] DemoArtikelListe =
    [
        new("FVD11004",
            "LB2 Leistungs-Kit 964 - Straße - min.98 Oktan", 3, 8),

        new("FVD11005",
            "LB2 Leistungs-Kit 964 - Race - ohne TÜV", 2, 1),

        new("FVD11004SF",
            "LB2 Leistungs-Kit 964 - Straße - SPRINT Sportluftfilter", 3, 5),

        new("FVD11003",
            "LB2 Leistungs-Kit 964 - Straße - mit Sportkatalysator", 2, 4),

        new("FVD11003SF",
            "LB2 Leistungs-Kit 964 - Straße - Sportkatalysator SPRINT", 2, 0),

        new("FVD11008",
            "LB2 Leistungs-Kit 964 - Race - mit Sportkatalysator", 2, 3),

        new("FVD11001",
            "LB1 Leistungs-Kit - Straße - 911 1984-89", 4, 12),

        new("FVD11002",
            "LB1 Leistungs-Kit - Straße - mit Sportkatalysator", 3, 2),

        new("99334701102AT",
            "Lenkung (Neuteil) 964/993 C2/C4/Turbo - im Austausch", 2, 6),

        new("FVD34399300",
            "Stützlager FVD (Set: VA + HA) 993, Uniball", 4, 7)
    ];

    public static async Task SeedAsync(
        IDbContextFactory<AppDbContext> contextFactory)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        // Alle Demo-Artikel und Buchungen gemeinsam speichern.
        await using var transaction =
            await db.Database.BeginTransactionAsync();

        var vorhandeneNummern = (await db.Artikel
                .AsNoTracking()
                .Select(a => a.Artikelnummer)
                .ToListAsync())
            .ToHashSet(StringComparer.Ordinal);

        int neueArtikel = 0;
        int neueBuchungen = 0;

        foreach (var demo in DemoArtikelListe)
        {
            // Vorhandene Artikel niemals überschreiben.
            if (vorhandeneNummern.Contains(demo.Nummer))
            {
                Console.WriteLine(
                    $"Übersprungen: {demo.Nummer} (bereits vorhanden)");
                continue;
            }

            var artikel = new Artikel
            {
                Artikelnummer = demo.Nummer,
                Bezeichnung = demo.Bezeichnung,
                Beschreibung = DemoHinweis,
                Lagerbestand = 0,
                Mindestbestand = demo.Mindestbestand
            };

            db.Artikel.Add(artikel);

            // ID für die Lagerbewegung erzeugen.
            await db.SaveChangesAsync();

            if (demo.Anfangsbestand > 0)
            {
                var bewegung = new Lagerbewegung
                {
                    ArtikelId = artikel.Id,
                    Bewegungsart = Bewegungsart.Eingang,
                    Menge = demo.Anfangsbestand,
                    Datum = DateTime.UtcNow,
                    Bemerkung = "DEMO: Fiktiver Anfangsbestand"
                };

                db.Lagerbewegungen.Add(bewegung);

                // Der Anfangsbestand wird als Wareneingang dokumentiert.
                artikel.Lagerbestand = demo.Anfangsbestand;

                neueBuchungen++;
            }

            vorhandeneNummern.Add(demo.Nummer);
            neueArtikel++;
        }

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        Console.WriteLine(
            $"Demo-Seeding abgeschlossen: " +
            $"{neueArtikel} neue Artikel, " +
            $"{neueBuchungen} neue Lagerbewegungen.");
    }
}
