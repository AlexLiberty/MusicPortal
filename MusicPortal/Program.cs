using Microsoft.EntityFrameworkCore;
using MusicPortal.Models.Repository;
using MusicPortal.Models.DataBase;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.Extensions.Options;
using MusicPortal.Hubs;

var builder = WebApplication.CreateBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MusicPortalContext>(options => options.UseSqlServer(connection));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IMusicRepository, MusicRepository>();
builder.Services.AddScoped<NotificationHub>();
builder.Services.AddSignalR();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "uk", "en" };
    options.DefaultRequestCulture = new RequestCulture("uk");
    options.SupportedCultures = supportedCultures.Select(culture => new CultureInfo(culture)).ToList();
    options.SupportedUICultures = supportedCultures.Select(culture => new CultureInfo(culture)).ToList();
    options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowSpecificOrigins");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notification");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
