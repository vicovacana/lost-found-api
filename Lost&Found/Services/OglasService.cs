using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.Oglas;
using Lost_Found.Models;
using Lost_Found.Models.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class OglasService : IOglasService
    {
        private static readonly Dictionary<string, string> DozvoljeniTipovi = new()
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp",
            ["image/gif"] = ".gif",
        };
        private const long MaxVelicinaFajlaBajtova = 5 * 1024 * 1024;

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public OglasService(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IReadOnlyList<OglasDto>> GetAllAsync(TipOglasa? tip, int? kreatorId, int? adminId, Kategorija? kategorija, string? grad, bool? samoAktivni)
        {
            var query = _db.Oglasi.Include(o => o.Kreator).AsQueryable();

            if (tip.HasValue) query = query.Where(o => o.Tip == tip.Value);
            if (kreatorId.HasValue) query = query.Where(o => o.KreatorId == kreatorId.Value);
            if (adminId.HasValue) query = query.Where(o => o.AdminId == adminId.Value);
            if (kategorija.HasValue) query = query.Where(o => o.Kategorija == kategorija.Value);
            if (!string.IsNullOrWhiteSpace(grad))
            {
                var gradTrimmed = grad.Trim();
                query = query.Where(o => EF.Functions.ILike(o.Grad, gradTrimmed));
            }
            if (samoAktivni.HasValue)
            {
                query = samoAktivni.Value
                    ? query.Where(o => o.Razgovor == null || o.Razgovor.StatusRazgovora != StatusRazgovora.Zatvoren)
                    : query.Where(o => o.Razgovor != null && o.Razgovor.StatusRazgovora == StatusRazgovora.Zatvoren);
            }

            var oglasi = await query.OrderByDescending(o => o.DatumKreiranja).ToListAsync();
            return oglasi.Select(o => ToDto(o)).ToList();
        }

        public async Task<OglasDto> GetByIdAsync(int oglasId, int? currentKorisnikId, bool isAdmin)
        {
            var oglas = await _db.Oglasi.Include(o => o.Kreator)
                .FirstOrDefaultAsync(o => o.OglasId == oglasId)
                ?? throw new NotFoundException($"Oglas {oglasId} ne postoji.");

            var smeVidiOpisLokacije = isAdmin || (currentKorisnikId.HasValue && oglas.KreatorId == currentKorisnikId.Value);
            return ToDto(oglas, smeVidiOpisLokacije);
        }

        public async Task<OglasDto> CreateAsync(int kreatorId, OglasCreateDto dto)
        {
            var oglas = new Oglas
            {
                Naziv = dto.Naziv,
                Opis = dto.Opis,
                Tip = dto.Tip,
                Kategorija = dto.Kategorija,
                Grad = dto.Grad,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Fotografija = dto.Fotografija,
                OpisLokacije = NormalizujOpisLokacije(dto.Tip, dto.OpisLokacije),
                KreatorId = kreatorId,
                DatumKreiranja = DateTime.UtcNow
            };

            _db.Oglasi.Add(oglas);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(oglas.OglasId, kreatorId, isAdmin: false);
        }

        public async Task<OglasDto> UpdateAsync(int oglasId, int currentKorisnikId, bool isAdmin, OglasUpdateDto dto)
        {
            var oglas = await _db.Oglasi.Include(o => o.Kreator)
                .FirstOrDefaultAsync(o => o.OglasId == oglasId)
                ?? throw new NotFoundException($"Oglas {oglasId} ne postoji.");

            if (!isAdmin && oglas.KreatorId != currentKorisnikId)
            {
                throw new ForbiddenException("Samo vlasnik oglasa ili admin mogu da ga izmene.");
            }

            oglas.Naziv = dto.Naziv;
            oglas.Opis = dto.Opis;
            oglas.Tip = dto.Tip;
            oglas.Kategorija = dto.Kategorija;
            oglas.Grad = dto.Grad;
            oglas.Latitude = dto.Latitude;
            oglas.Longitude = dto.Longitude;
            oglas.Fotografija = dto.Fotografija;
            oglas.OpisLokacije = NormalizujOpisLokacije(dto.Tip, dto.OpisLokacije);

            await _db.SaveChangesAsync();

            return ToDto(oglas, ukljuciOpisLokacije: true);
        }

        public async Task DeleteAsync(int oglasId, int currentKorisnikId, bool isAdmin)
        {
            var oglas = await _db.Oglasi.FirstOrDefaultAsync(o => o.OglasId == oglasId)
                ?? throw new NotFoundException($"Oglas {oglasId} ne postoji.");

            if (!isAdmin && oglas.KreatorId != currentKorisnikId)
            {
                throw new ForbiddenException("Samo vlasnik oglasa ili admin mogu da ga obrišu.");
            }

            _db.Oglasi.Remove(oglas);
            await _db.SaveChangesAsync();
        }

        public async Task<OglasDto> AssignAdminAsync(int oglasId, int? adminId)
        {
            var oglas = await _db.Oglasi.Include(o => o.Kreator)
                .FirstOrDefaultAsync(o => o.OglasId == oglasId)
                ?? throw new NotFoundException($"Oglas {oglasId} ne postoji.");

            if (adminId.HasValue)
            {
                var adminPostoji = await _db.Admini.AnyAsync(a => a.KorisnikId == adminId.Value);
                if (!adminPostoji)
                {
                    throw new NotFoundException($"Admin {adminId} ne postoji.");
                }
            }

            oglas.AdminId = adminId;
            await _db.SaveChangesAsync();

            return ToDto(oglas);
        }

        public async Task<string> SacuvajFotografijuAsync(IFormFile fajl, string baseUrl)
        {
            if (fajl is null || fajl.Length == 0)
            {
                throw new ValidationException("Fajl nije poslat.");
            }

            if (fajl.Length > MaxVelicinaFajlaBajtova)
            {
                throw new ValidationException("Fotografija ne sme biti veća od 5 MB.");
            }

            if (!DozvoljeniTipovi.TryGetValue(fajl.ContentType, out var ekstenzija))
            {
                throw new ValidationException("Dozvoljeni formati su JPG, PNG, WEBP i GIF.");
            }

            var direktorijum = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "oglasi");
            Directory.CreateDirectory(direktorijum);

            var imeFajla = $"{Guid.NewGuid()}{ekstenzija}";
            var putanja = Path.Combine(direktorijum, imeFajla);

            await using (var stream = new FileStream(putanja, FileMode.Create))
            {
                await fajl.CopyToAsync(stream);
            }

            return $"{baseUrl}/uploads/oglasi/{imeFajla}";
        }

        private static string? NormalizujOpisLokacije(TipOglasa tip, string? opisLokacije)
        {
            if (tip != TipOglasa.Nadjeno || string.IsNullOrWhiteSpace(opisLokacije))
            {
                return null;
            }

            return opisLokacije.Trim();
        }

        private static OglasDto ToDto(Oglas oglas, bool ukljuciOpisLokacije = false) => new()
        {
            OglasId = oglas.OglasId,
            Naziv = oglas.Naziv,
            Opis = oglas.Opis,
            DatumKreiranja = oglas.DatumKreiranja,
            Tip = oglas.Tip,
            Kategorija = oglas.Kategorija,
            Grad = oglas.Grad,
            Latitude = oglas.Latitude,
            Longitude = oglas.Longitude,
            Fotografija = oglas.Fotografija,
            OpisLokacije = ukljuciOpisLokacije ? oglas.OpisLokacije : null,
            KreatorId = oglas.KreatorId,
            KreatorKorisnickoIme = oglas.Kreator?.KorisnickoIme ?? string.Empty,
            AdminId = oglas.AdminId
        };
    }
}
