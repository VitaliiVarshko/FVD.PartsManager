
using FVD.PartsManager.Web.Data;
using FVD.PartsManager.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FVD.PartsManager.Web.Services;

public class ArtikelService(
    IDbContextFactory<AppDbContext> contextFactory)
{
    // Alle Artikel laden
    public async Task<List<Artikel>> GetAllAsync()
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        return await db.Artikel
            .AsNoTracking()
            .OrderBy(a => a.Artikelnummer)
            .ToListAsync();
    }

    // Artikel anhand der ID laden
    public async Task<Artikel?> GetByIdAsync(int id)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        return await db.Artikel
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    // Neuen Artikel anlegen
    public async Task<Artikel> CreateAsync(Artikel artikel)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        if (await db.Artikel.AnyAsync(
            a => a.Artikelnummer == artikel.Artikelnummer))
        {
            throw new InvalidOperationException(
                "Die Artikelnummer existiert bereits.");
        }

        db.Artikel.Add(artikel);
        await db.SaveChangesAsync();

        return artikel;
    }

    // Vorhandenen Artikel bearbeiten
    public async Task UpdateAsync(Artikel artikel)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        var existing = await db.Artikel.FindAsync(artikel.Id);

        if (existing is null)
        {
            throw new KeyNotFoundException(
                "Der Artikel wurde nicht gefunden.");
        }

        if (await db.Artikel.AnyAsync(
            a => a.Artikelnummer == artikel.Artikelnummer
                 && a.Id != artikel.Id))
        {
            throw new InvalidOperationException(
                "Die Artikelnummer existiert bereits.");
        }

        existing.Artikelnummer = artikel.Artikelnummer;
        existing.Bezeichnung = artikel.Bezeichnung;
        existing.Beschreibung = artikel.Beschreibung;
        existing.Mindestbestand = artikel.Mindestbestand;

        // Lagerbestand wird nur über Lagerbewegungen geändert.

        await db.SaveChangesAsync();
    }

    // Artikel löschen
    public async Task DeleteAsync(int id)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        var artikel = await db.Artikel.FindAsync(id);

        if (artikel is null)
        {
            throw new KeyNotFoundException(
                "Der Artikel wurde nicht gefunden.");
        }

        if (await db.Lagerbewegungen.AnyAsync(
            b => b.ArtikelId == id))
        {
            throw new InvalidOperationException(
                "Artikel mit Lagerbewegungen können nicht gelöscht werden.");
        }

        db.Artikel.Remove(artikel);
        await db.SaveChangesAsync();
    }
}
