using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Web.Account;

internal static class EndpointRouteBuilderExtensions
{
    // This works via extending the WebApplication used in Program.cs
    public static void MapLogoutEndpoint(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapPost("/Logout", async (
            ClaimsPrincipal _,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            Console.WriteLine("Signing out");
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect($"/{returnUrl}");
        });
    }
}

