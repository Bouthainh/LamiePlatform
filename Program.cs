using BadeePlatform.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Parent/Login";
        options.AccessDeniedPath = "/Parent/AccessDenied";
    });

builder.Services.AddScoped<BadeePlatform.Services.IChildService, BadeePlatform.Services.ChildService>();

builder.Services.AddScoped<BadeePlatform.Services.IParentService, BadeePlatform.Services.ParentService>();

builder.Services.AddScoped<BadeePlatform.Services.IDashboardService, BadeePlatform.Services.DashboardService>();

builder.Services.AddScoped<IPasswordHasher<BadeePlatform.Models.Parent>, PasswordHasher<BadeePlatform.Models.Parent>>();

// Add services to the container.
builder.Services.AddControllersWithViews();

//DI for connection string 
builder.Services.AddDbContext<BadeedbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
// for the unity api
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();