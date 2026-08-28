using Lost_Found.Models.Enums;

namespace Lost_Found.DTOs.Oglas
{
    public class OglasDto
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
        public string? OpisLokacije { get; set; }

        public int KreatorId { get; set; }
        public string KreatorKorisnickoIme { get; set; } = string.Empty;
        public int? AdminId { get; set; }
    }
}
