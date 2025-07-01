using ContactosApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ContactsDB")); // In-memory DB

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Ruta principal -> HomeController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ruta adicional para ContactsController
app.MapControllerRoute(
    name: "contacts",
    pattern: "contacts/{action=Index}/{id?}",
    defaults: new { controller = "Contacts" });

app.Run();