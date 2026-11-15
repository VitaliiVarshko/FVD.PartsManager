
using FVD.PartsManager.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FVD.PartsManager.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Artikel> Artikel => Set<Artikel>();

    public DbSet<Lagerbewegung> Lagerbewegungen =>
        Set<Lagerbewegung>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Artikelstammdaten
        modelBuilder.Entity<Artikel>(entity =>
        {
            entity.ToTable("Artikel", table =>
            {
                table.HasCheckConstraint(
                    "CK_Artikel_Lagerbestand",
                    "\"Lagerbestand\" >= 0");

                table.HasCheckConstraint(
                    "CK_Artikel_Mindestbestand",
                    "\"Mindestbestand\" >= 0");
            });

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Artikelnummer)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(a => a.Artikelnummer)
                .IsUnique();

            entity.Property(a => a.Bezeichnung)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(a => a.Beschreibung)
                .HasMaxLength(1000);

            entity.Property(a => a.Lagerbestand)
                .HasDefaultValue(0);

            entity.Property(a => a.Mindestbestand)
                .HasDefaultValue(0);
        });

        // Wareneingänge und Warenausgänge
        modelBuilder.Entity<Lagerbewegung>(entity =>
        {
            entity.ToTable("Lagerbewegungen", table =>
            {
                table.HasCheckConstraint(
                    "CK_Lagerbewegung_Menge",
                    "\"Menge\" > 0");
            });

            entity.HasKey(b => b.Id);

            entity.Property(b => b.Bewegungsart)
                .HasConversion<int>();

            entity.Property(b => b.Menge)
                .IsRequired();

            entity.Property(b => b.Datum)
                .IsRequired();

            entity.Property(b => b.Bemerkung)
                .HasMaxLength(500);

            entity.HasOne(b => b.Artikel)
                .WithMany(a => a.Lagerbewegungen)
                .HasForeignKey(b => b.ArtikelId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
