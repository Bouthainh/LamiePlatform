using LamiePlatform.Data;
using LamiePlatform.Services;
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
// register HttpClient for OpenAI API
builder.Services.AddHttpClient("OpenAI", client =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"]
        ?? throw new InvalidOperationException("OpenAI:ApiKey not set in appsettings.json.");

    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});

builder.Services.AddScoped<LamiePlatform.Services.IChildService, LamiePlatform.Services.ChildService>();

builder.Services.AddScoped<LamiePlatform.Services.IParentService, LamiePlatform.Services.ParentService>();

builder.Services.AddScoped<LamiePlatform.Services.IDashboardService, LamiePlatform.Services.DashboardService>();

builder.Services.AddScoped<LamiePlatform.Services.IEducatorService, LamiePlatform.Services.EducatorService>();

builder.Services.AddScoped<LamiePlatform.Services.IChatService, LamiePlatform.Services.ChatService>();
builder.Services.AddScoped<LamiePlatform.Services.IRecommendationService, LamiePlatform.Services.RecommendationService>();
builder.Services.AddScoped<LamiePlatform.Services.GameService, LamiePlatform.Services.GameService>();



builder.Services.AddScoped<IPasswordHasher<LamiePlatform.Models.Parent>, PasswordHasher<LamiePlatform.Models.Parent>>();
builder.Services.AddScoped<IPasswordHasher<LamiePlatform.Models.Educator>, PasswordHasher<LamiePlatform.Models.Educator>>();
builder.Services.AddScoped<LamiePlatform.Services.IRequestService, LamiePlatform.Services.RequestService>();
builder.Services.AddScoped<LamiePlatform.Services.IGroupingService, LamiePlatform.Services.GroupingService>();


// Add services to the container.
builder.Services.AddControllersWithViews();

//DI for connection string 
builder.Services.AddDbContext<LamieDbContext>(options =>
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