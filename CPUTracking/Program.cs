using CPUTracking.Models;
using CPUTracking.Services;
using NuGet.Protocol.Core.Types;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("CPUInfoDatabase"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISessionService, SessionService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


