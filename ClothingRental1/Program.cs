using BusinessLayer.Concrate;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var cookieOptions = builder.Configuration.GetSection("CookieAuthOption").Get<CookieAuthOption>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    //options.AccessDeniedPath = cookieOptions.AccessDeniedPath;
    options.Cookie.Name = cookieOptions.Name;
    options.LoginPath = cookieOptions.LoginPath;
    options.LogoutPath = cookieOptions.LogOutPath;
    options.SlidingExpiration = cookieOptions.SlidingExpiration;
    options.ExpireTimeSpan = TimeSpan.FromSeconds(cookieOptions.TimeOut);
});

// Add DbContext and SQL Server connection
builder.Services.AddDbContext<ClothingRental1DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".ClothingRental1.Session";
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<ProductRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
