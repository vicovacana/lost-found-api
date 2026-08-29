using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Lost_Found.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        protected int? CurrentUserIdOrNull =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        protected bool IsAdmin => User.IsInRole("Admin");
    }
}
