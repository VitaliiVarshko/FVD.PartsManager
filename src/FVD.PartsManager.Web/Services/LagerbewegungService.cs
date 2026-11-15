
using FVD.PartsManager.Web.Data;
using FVD.PartsManager.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FVD.PartsManager.Web.Services;

public class LagerbewegungService(
    IDbContextFactory<AppDbContext> contextFactory)
{
    // Lagerbewegungen mit Artikelinformationen laden
    public async Task<List<Lagerbewegung>> GetAllAsync()
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        return await db.Lagerbewegungen
            .AsNoTracking()
            .Include(b => b.Artikel)
            .OrderByDescending(b => b.Datum)
            .ThenByDescending(b => b.Id)
            .ToListAsync();
    }

    // Wareneingang oder Warenausgang buchen
    public async Task BuchenAsync(
        int artikelId,
        Bewegungsart bewegungsart,
        int menge,
        string? bemerkung)
    {
        if (menge <= 0)
        {
            throw new InvalidOperationException(
                "Die Menge muss größer als 0 sein.");
        }

        if (bewegungsart != Bewegungsart.Eingang
            && bewegungsart != Bewegungsart.Ausgang)
        {
            throw new InvalidOperationException(
                "Ungültige Bewegungsart.");
        }

        await using var db = await contextFactory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();

        int aktualisierteArtikel;

        if (bewegungsart == Bewegungsart.Eingang)
        {
            // Bestand atomar erhöhen, ohne Integer-Überlauf.
            aktualisierteArtikel = await db.Artikel
                .Where(a => a.Id == artikelId
                    && a.Lagerbestand <= int.MaxValue - menge)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        a => a.Lagerbestand,
                        a => a.Lagerbestand + menge));
        }
        else
        {
            // Bestand nur bei ausreichender Menge reduzieren.
            aktualisierteArtikel = await db.Artikel
                .Where(a => a.Id == artikelId
                    && a.Lagerbestand >= menge)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        a => a.Lagerbestand,
                        a => a.Lagerbestand - menge));
        }

        if (aktualisierteArtikel != 1)
        {
            throw new InvalidOperationException(
                "Die Buchung ist nicht möglich. " +
                "Bitte Artikel und verfügbaren Bestand prüfen.");
        }

        var bewegung = new Lagerbewegung
        {
            ArtikelId = artikelId,
            Bewegungsart = bewegungsart,
            Menge = menge,
            Datum = DateTime.UtcNow,
            Bemerkung = string.IsNullOrWhiteSpace(bemerkung)
                ? null
                : bemerkung.Trim()
        };

        db.Lagerbewegungen.Add(bewegung);
        await db.SaveChangesAsync();

        await transaction.CommitAsync();
    }
}
