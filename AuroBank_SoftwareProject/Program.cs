using AuroBank_SoftwareProject.Data;
using AuroBank_SoftwareProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();



//Database Option 1: SQL Server
//builder.Services.AddDbContext<BankDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BankDbContext>(opts =>
opts.UseSqlServer(connString, opts =>
{
    opts.EnableRetryOnFailure();
    opts.CommandTimeout(120);
    opts.UseCompatibilityLevel(110);
}));

builder.Services.AddIdentity<AppUser, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 8;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireDigit = false;
    opts.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<BankDbContext>();

builder.Services.AddIdentityCore<AppUser>().AddRoles<IdentityRole>()
.AddEntityFrameworkStores<BankDbContext>();
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "allpaging",
    pattern: "{controller}/{action}/{id=all}/page{BookingPage}");

// route for sorting
app.MapControllerRoute(
    name: "sortingcategory",
    pattern: "{controller}/{action}/{id}/orderby{sortBy}");
// least specific route - 0 required segments 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}/{slug?}");

SeedData.EnsurePopulatedAsync(app);

app.Run();
