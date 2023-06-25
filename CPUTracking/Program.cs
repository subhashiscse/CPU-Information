using CPUTracking.Models;
using CPUTracking.Services;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Core.Types;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("CPUInfoDatabase"));

// Add services to the container.

//builder.Services.AddTransient<HttpSerivce>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IContestService, ContestService>();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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
    pattern: "{controller=MemberInfo}/{action=MemberList}/{id?}");


app.Run();


