using System.ComponentModel.DataAnnotations;
using Lost_Found.Models.Enums;

namespace Lost_Found.DTOs.Claim
{
    public class UpdateClaimStatusDto
    {
        [Required]
        public ClaimStatus Status { get; set; }
    }
}
