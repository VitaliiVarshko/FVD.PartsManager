
namespace FVD.PartsManager.Web.Models;

public class Artikel
{
    public int Id { get; set; }

    public string Artikelnummer { get; set; } = string.Empty;

    public string Bezeichnung { get; set; } = string.Empty;

    public string? Beschreibung { get; set; }

    public int Lagerbestand { get; set; }

    public int Mindestbestand { get; set; }

    public ICollection<Lagerbewegung> Lagerbewegungen { get; set; }
        = new List<Lagerbewegung>();
}
