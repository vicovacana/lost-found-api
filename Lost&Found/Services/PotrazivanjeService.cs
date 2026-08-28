using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.Potrazivanje;
using Lost_Found.Models;
using Lost_Found.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class PotrazivanjeService : IPotrazivanjeService
    {
        private readonly ApplicationDbContext _db;

        public PotrazivanjeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PotrazivanjeDto> CreateAsync(int oglasId, int korisnikId)
        {
            var oglasPostoji = await _db.Oglasi.AnyAsync(o => o.OglasId == oglasId);
            if (!oglasPostoji)
            {
                throw new NotFoundException($"Oglas {oglasId} ne postoji.");
            }

            var vecPostoji = await _db.Potrazivanja.AnyAsync(p => p.OglasId == oglasId && p.KorisnikId == korisnikId);
            if (vecPostoji)
            {
                throw new ConflictException("Već postoji potraživanje ovog korisnika za ovaj oglas.");
            }

            var potrazivanje = new Potrazivanje
            {
                OglasId = oglasId,
                KorisnikId = korisnikId,
                DatumKreiranja = DateTime.UtcNow,
                Status = StatusPotrazivanja.NaCekanju
            };

            _db.Potrazivanja.Add(potrazivanje);
            await _db.SaveChangesAsync();

            await _db.Entry(potrazivanje).Reference(p => p.Korisnik).LoadAsync();
            return ToDto(potrazivanje);
        }

        public async Task<IReadOnlyList<PotrazivanjeDto>> GetForOglasAsync(int oglasId, int currentKorisnikId, bool isAdmin)
        {
            var oglas = await _db.Oglasi.FirstOrDefaultAsync(o => o.OglasId == oglasId)
                ?? throw new NotFoundException($"Oglas {oglasId} ne postoji.");

            if (!isAdmin && oglas.KreatorId != currentKorisnikId)
            {
                throw new ForbiddenException("Samo vlasnik oglasa ili admin mogu da vide potraživanja.");
            }

            var potrazivanja = await _db.Potrazivanja
                .Include(p => p.Korisnik)
                .Where(p => p.OglasId == oglasId)
                .OrderBy(p => p.DatumKreiranja)
                .ToListAsync();

            return potrazivanja.Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<PotrazivanjeDto>> GetMineAsync(int korisnikId)
        {
            var potrazivanja = await _db.Potrazivanja
                .Include(p => p.Korisnik)
                .Where(p => p.KorisnikId == korisnikId)
                .OrderByDescending(p => p.DatumKreiranja)
                .ToListAsync();

            return potrazivanja.Select(ToDto).ToList();
        }

        public async Task<PotrazivanjeDto> UpdateStatusAsync(int oglasId, int korisnikId, StatusPotrazivanja noviStatus)
        {
            var potrazivanje = await _db.Potrazivanja
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(p => p.OglasId == oglasId && p.KorisnikId == korisnikId)
                ?? throw new NotFoundException("Potraživanje ne postoji.");

            potrazivanje.Status = noviStatus;
            potrazivanje.DatumRazresavanja = DateTime.UtcNow;

            if (noviStatus == StatusPotrazivanja.Prihvaceno)
            {
                var razgovor = await _db.Razgovori.FirstOrDefaultAsync(r => r.OglasId == oglasId);
                if (razgovor is not null && razgovor.StatusRazgovora == StatusRazgovora.Otvoren)
                {
                    razgovor.StatusRazgovora = StatusRazgovora.Zatvoren;
                }

                var ostalaNaCekanju = await _db.Potrazivanja
                    .Where(p => p.OglasId == oglasId && p.KorisnikId != korisnikId && p.Status == StatusPotrazivanja.NaCekanju)
                    .ToListAsync();

                foreach (var ostalo in ostalaNaCekanju)
                {
                    ostalo.Status = StatusPotrazivanja.Odbijeno;
                    ostalo.DatumRazresavanja = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();

            return ToDto(potrazivanje);
        }

        public async Task WithdrawAsync(int oglasId, int korisnikId, int currentKorisnikId)
        {
            var potrazivanje = await _db.Potrazivanja
                .FirstOrDefaultAsync(p => p.OglasId == oglasId && p.KorisnikId == korisnikId)
                ?? throw new NotFoundException("Potraživanje ne postoji.");

            if (potrazivanje.KorisnikId != currentKorisnikId)
            {
                throw new ForbiddenException("Samo podnosilac može da povuče svoje potraživanje.");
            }

            if (potrazivanje.Status != StatusPotrazivanja.NaCekanju)
            {
                throw new ConflictException("Potraživanje koje je već rešeno ne može da se povuče.");
            }

            _db.Potrazivanja.Remove(potrazivanje);
            await _db.SaveChangesAsync();
        }

        private static PotrazivanjeDto ToDto(Potrazivanje potrazivanje) => new()
        {
            KorisnikId = potrazivanje.KorisnikId,
            KorisnickoIme = potrazivanje.Korisnik?.KorisnickoIme ?? string.Empty,
            OglasId = potrazivanje.OglasId,
            DatumKreiranja = potrazivanje.DatumKreiranja,
            Status = potrazivanje.Status,
            DatumRazresavanja = potrazivanje.DatumRazresavanja
        };
    }
}
