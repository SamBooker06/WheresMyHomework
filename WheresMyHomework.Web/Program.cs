using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Auth;
using WheresMyHomework.Core.Services.Class;
using WheresMyHomework.Core.Services.Homework;
using WheresMyHomework.Core.Services.MessagingService;
using WheresMyHomework.Core.Services.SubjectService;
using WheresMyHomework.Core.Services.TagService;
using WheresMyHomework.Core.Services.TodoService;
using WheresMyHomework.Core.Services.Users;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models.Users;
using WheresMyHomework.Web.Account;
using WheresMyHomework.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Enabled use of razor components and InteractiveServer rendering
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Provides access to authorisation to components in the application
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services
    .AddScoped<AuthenticationStateProvider,
        IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Configure HSTS max age. This tells browsers to only communicate over HTTPS rather than HTTP
builder.Services.AddHsts(options => { options.MaxAge = TimeSpan.FromDays(365); });

// Configure SQL connection
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException(
        "Invalid connection string");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Custom services
builder.Services.AddScoped<IMessagingService, MessagingService>();
builder.Services.AddScoped<IHomeworkService, HomeworkService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IStandardUserService, StandardUserService>();

builder.Services.AddScoped<AdminAuthService>();
builder.Services.AddScoped<TeacherAuthService>();
builder.Services.AddScoped<StudentAuthService>();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITagService, TagService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapLogoutEndpoint();

app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");


await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await SeedData.InitialiseDataAsync(context, roleManager, userManager, true);
}

await app.RunAsync();

