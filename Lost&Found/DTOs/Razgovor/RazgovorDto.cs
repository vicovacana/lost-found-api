using Lost_Found.Models.Enums;

namespace Lost_Found.DTOs.Razgovor
{
    public class RazgovorDto
    {
        public int RazgovorId { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public StatusRazgovora StatusRazgovora { get; set; }
        public int OglasId { get; set; }
        public string OglasNaziv { get; set; } = string.Empty;
        public string? OpisLokacije { get; set; }
    }
}
