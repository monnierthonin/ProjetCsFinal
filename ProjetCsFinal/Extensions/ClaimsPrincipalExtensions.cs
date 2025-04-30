using System.Security.Claims;
using DAL.Models;

namespace ProjetCsFinal.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == Role.Admin.ToString());
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}
