using Lost_Found.Models.Enums;

namespace Lost_Found.Models
{
    public class Oglas
    {
        public int OglasId { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public DateTime DatumKreiranja { get; set; }
        public TipOglasa Tip { get; set; }
        public Kategorija Kategorija { get; set; }
        public string Grad { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Fotografija { get; set; }

        public int KreatorId { get; set; }
        public StandardniKorisnik Kreator { get; set; } = null!;

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        public ICollection<Potrazivanje> Potrazivanja { get; set; } = new List<Potrazivanje>();
        public Razgovor? Razgovor { get; set; }
    }
}
