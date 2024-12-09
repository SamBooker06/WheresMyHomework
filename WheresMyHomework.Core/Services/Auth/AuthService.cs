using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Auth;

public abstract class AuthService(
    AuthenticationStateProvider authenticationStateProvider,
    UserManager<ApplicationUser> userManager,
    string roleName) : IAuthService
{
    public async Task<AuthInfo?> GetAuthenticatedUserInfoAsync()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        var claimsPrincipal = state.User;

        if (!claimsPrincipal.IsInRole(roleName)) return null;

        var userModel = (await userManager.GetUserAsync(claimsPrincipal))!;
        return new AuthInfo
        {
            UserId = userModel.Id,
            Email = userModel.Email,
        };
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        var claimsPrincipal = state.User;

        return claimsPrincipal.IsInRole(roleName);
    }
}

// Individual auth services
public class AdminAuthService(
    AuthenticationStateProvider authenticationStateProvider,
    UserManager<ApplicationUser> userManager) : AuthService(authenticationStateProvider, userManager, "Admin");

public class TeacherAuthService(
    AuthenticationStateProvider authenticationStateProvider,
    UserManager<ApplicationUser> userManager) : AuthService(authenticationStateProvider, userManager, "Teacher");

public class StudentAuthService(
    AuthenticationStateProvider authenticationStateProvider,
    UserManager<ApplicationUser> userManager) : AuthService(authenticationStateProvider, userManager, "Student");