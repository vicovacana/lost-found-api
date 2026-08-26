using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.Auth;
using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext db, IJwtTokenService jwtTokenService, IConfiguration configuration)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var postoji = await _db.Korisnici.AnyAsync(k =>
                k.KorisnickoIme == dto.KorisnickoIme || k.Email == dto.Email);
            if (postoji)
            {
                throw new ConflictException("Korisničko ime ili email su već zauzeti.");
            }

            var korisnik = new StandardniKorisnik
            {
                KorisnickoIme = dto.KorisnickoIme,
                Email = dto.Email,
                LozinkaHash = BCrypt.Net.BCrypt.HashPassword(dto.Lozinka),
                VremeKreiranja = DateTime.UtcNow
            };

            _db.StandardniKorisnici.Add(korisnik);
            await _db.SaveChangesAsync();

            return BuildResponse(korisnik);
        }

        public async Task<AuthResponseDto> RegisterAdminAsync(RegisterAdminDto dto)
        {
            var tajniKod = _configuration["Admin:RegistrationSecret"];
            if (string.IsNullOrEmpty(tajniKod) || dto.TajniKod != tajniKod)
            {
                throw new ValidationException("Pogrešan tajni kod.");
            }

            var postoji = await _db.Korisnici.AnyAsync(k =>
                k.KorisnickoIme == dto.KorisnickoIme || k.Email == dto.Email);
            if (postoji)
            {
                throw new ConflictException("Korisničko ime ili email su već zauzeti.");
            }

            var admin = new Admin
            {
                KorisnickoIme = dto.KorisnickoIme,
                Email = dto.Email,
                LozinkaHash = BCrypt.Net.BCrypt.HashPassword(dto.Lozinka),
                VremeKreiranja = DateTime.UtcNow
            };

            _db.Admini.Add(admin);
            await _db.SaveChangesAsync();

            return BuildResponse(admin);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var korisnik = await _db.Korisnici
                .FirstOrDefaultAsync(k => k.KorisnickoIme == dto.KorisnickoIme);

            if (korisnik is null || !BCrypt.Net.BCrypt.Verify(dto.Lozinka, korisnik.LozinkaHash))
            {
                throw new ValidationException("Pogrešno korisničko ime ili lozinka.");
            }

            return BuildResponse(korisnik);
        }

        private AuthResponseDto BuildResponse(Korisnik korisnik) => new()
        {
            Token = _jwtTokenService.GenerateToken(korisnik),
            KorisnikId = korisnik.KorisnikId,
            KorisnickoIme = korisnik.KorisnickoIme,
            Email = korisnik.Email,
            Uloga = korisnik is Admin ? "Admin" : "StandardniKorisnik"
        };
    }
}
