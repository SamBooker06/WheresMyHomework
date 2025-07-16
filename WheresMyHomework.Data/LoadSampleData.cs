using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data;

public class LoadSampleData
{
    public static void LoadData(ApplicationDbContext context, RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        
    }
}
