using Lost_Found.DTOs.Oglas;
using Lost_Found.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Lost_Found.Services
{
    public interface IOglasService
    {
        Task<IReadOnlyList<OglasDto>> GetAllAsync(TipOglasa? tip, int? kreatorId, int? adminId, Kategorija? kategorija, string? grad, bool? samoAktivni);
        Task<OglasDto> GetByIdAsync(int oglasId, int? currentKorisnikId, bool isAdmin);
        Task<OglasDto> CreateAsync(int kreatorId, OglasCreateDto dto);
        Task<OglasDto> UpdateAsync(int oglasId, int currentKorisnikId, bool isAdmin, OglasUpdateDto dto);
        Task DeleteAsync(int oglasId, int currentKorisnikId, bool isAdmin);
        Task<OglasDto> AssignAdminAsync(int oglasId, int? adminId);
        Task<string> SacuvajFotografijuAsync(IFormFile fajl, string baseUrl);
    }
}
