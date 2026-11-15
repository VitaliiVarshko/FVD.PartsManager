
namespace FVD.PartsManager.Web.Models;

public enum Bewegungsart
{
    Eingang = 1,
    Ausgang = 2
}

public class Lagerbewegung
{
    public int Id { get; set; }

    public int ArtikelId { get; set; }

    public Artikel Artikel { get; set; } = null!;

    public Bewegungsart Bewegungsart { get; set; }

    public int Menge { get; set; }

    public DateTime Datum { get; set; } = DateTime.UtcNow;

    public string? Bemerkung { get; set; }
}
