using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.Razgovor;
using Lost_Found.Models;
using Lost_Found.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class RazgovorService : IRazgovorService
    {
        private readonly ApplicationDbContext _db;

        public RazgovorService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<RazgovorDto> OpenAsync(int oglasId)
        {
            var oglas = await _db.Oglasi.FirstOrDefaultAsync(o => o.OglasId == oglasId)
                ?? throw new NotFoundException($"Oglas {oglasId} ne postoji.");

            var vecPostoji = await _db.Razgovori.AnyAsync(r => r.OglasId == oglasId);
            if (vecPostoji)
            {
                throw new ConflictException("Razgovor za ovaj oglas već postoji.");
            }

            var razgovor = new Razgovor
            {
                OglasId = oglasId,
                DatumKreiranja = DateTime.UtcNow,
                StatusRazgovora = StatusRazgovora.Otvoren
            };

            _db.Razgovori.Add(razgovor);
            await _db.SaveChangesAsync();

            return ToDto(razgovor, oglas.Naziv);
        }

        public async Task<RazgovorDto> GetForOglasAsync(int oglasId, int currentKorisnikId, bool isAdmin)
        {
            var razgovor = await _db.Razgovori.Include(r => r.Oglas).FirstOrDefaultAsync(r => r.OglasId == oglasId)
                ?? throw new NotFoundException($"Razgovor za oglas {oglasId} ne postoji.");

            await EnsureParticipantAsync(currentKorisnikId, isAdmin, razgovor);
            return ToDto(razgovor, razgovor.Oglas.Naziv);
        }

        public async Task<RazgovorDto> GetByIdAsync(int razgovorId, int currentKorisnikId, bool isAdmin)
        {
            var razgovor = await _db.Razgovori.Include(r => r.Oglas).FirstOrDefaultAsync(r => r.RazgovorId == razgovorId)
                ?? throw new NotFoundException($"Razgovor {razgovorId} ne postoji.");

            await EnsureParticipantAsync(currentKorisnikId, isAdmin, razgovor);
            return ToDto(razgovor, razgovor.Oglas.Naziv);
        }

        public async Task<RazgovorDto> UpdateStatusAsync(int razgovorId, StatusRazgovora noviStatus)
        {
            var razgovor = await _db.Razgovori.Include(r => r.Oglas).FirstOrDefaultAsync(r => r.RazgovorId == razgovorId)
                ?? throw new NotFoundException($"Razgovor {razgovorId} ne postoji.");

            razgovor.StatusRazgovora = noviStatus;
            await _db.SaveChangesAsync();

            return ToDto(razgovor, razgovor.Oglas.Naziv);
        }

        public async Task<IReadOnlyList<RazgovorDto>> GetMineAsync(int currentKorisnikId, bool isAdmin)
        {
            IQueryable<Razgovor> query = _db.Razgovori.Include(r => r.Oglas);

            query = isAdmin
                ? query.Where(r => r.Oglas.AdminId == currentKorisnikId)
                : query.Where(r => r.Oglas.KreatorId == currentKorisnikId
                    || _db.Potrazivanja.Any(p => p.OglasId == r.OglasId && p.KorisnikId == currentKorisnikId));

            var razgovori = await query.OrderByDescending(r => r.DatumKreiranja).ToListAsync();
            return razgovori.Select(r => ToDto(r, r.Oglas.Naziv)).ToList();
        }

        public async Task EnsureParticipantAsync(int razgovorId, int currentKorisnikId, bool isAdmin)
        {
            var razgovor = await _db.Razgovori.FirstOrDefaultAsync(r => r.RazgovorId == razgovorId)
                ?? throw new NotFoundException($"Razgovor {razgovorId} ne postoji.");

            await EnsureParticipantAsync(currentKorisnikId, isAdmin, razgovor);
        }

        private async Task EnsureParticipantAsync(int currentKorisnikId, bool isAdmin, Razgovor razgovor)
        {
            if (isAdmin)
            {
                return;
            }

            var jeVlasnikOglasa = await _db.Oglasi.AnyAsync(o => o.OglasId == razgovor.OglasId && o.KreatorId == currentKorisnikId);
            if (jeVlasnikOglasa)
            {
                return;
            }

            var imaPotrazivanje = await _db.Potrazivanja.AnyAsync(p => p.OglasId == razgovor.OglasId && p.KorisnikId == currentKorisnikId);
            if (imaPotrazivanje)
            {
                return;
            }

            throw new ForbiddenException("Nemate pristup ovom razgovoru.");
        }

        private static RazgovorDto ToDto(Razgovor razgovor, string oglasNaziv) => new()
        {
            RazgovorId = razgovor.RazgovorId,
            DatumKreiranja = razgovor.DatumKreiranja,
            StatusRazgovora = razgovor.StatusRazgovora,
            OglasId = razgovor.OglasId,
            OglasNaziv = oglasNaziv
        };
    }
}
