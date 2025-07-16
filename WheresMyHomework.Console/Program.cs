using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WheresMyHomework.Core.Services.SchoolService;
using WheresMyHomework.Data;

var app = Host.CreateApplicationBuilder()
    .Build();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

var services = new ServiceCollection()
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer("YourConnectionStringHere"))
    .AddScoped<ISchoolService, SchoolService>()
    .BuildServiceProvider();

await using var context = services.GetRequiredService<ApplicationDbContext>();
var schoolService = services.GetService<ISchoolService>();

if (args.Length == 0) return;

var command = args[0];

switch (command)
{
    case "create":
    {
        await HandleCreateCommand();
        break;
    }
    default:
        Console.WriteLine("Invalid command");
        break;
}

return;

async Task HandleCreateCommand()
{
    var createType = args[1];
    switch (createType)
    {
        case "school":
        {
            var schoolName = args[2];
            var schoolId = await CreateSchool(schoolName);
            Console.WriteLine($"Created school {schoolId}");

            break;
        }
    }
}

async Task<int> CreateSchool(string schoolName)
{
    return await schoolService.CreateSchoolAsync(new SchoolRequestInfo { Name = schoolName, });
}