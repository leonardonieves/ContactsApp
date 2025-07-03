using ContactsApp.Data;
using ContactsApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC and views
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ContactsDB"));

// Identity setup
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Build app
var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "contacts",
    pattern: "contacts/{action=Index}/{id?}",
    defaults: new { controller = "Contacts" });

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndUsersAsync(services);
}

app.Run();

async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    string[] roles = { "admin", "basic" };

    // Crear roles si no existen
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Crear usuario admin
    var adminEmail = "admin@dev.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new User { UserName = adminEmail, Email = adminEmail, FullName = "Admin user", ProfileImageUrl = "" };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "admin");
    }

    // Crear usuario basic
    var basicEmail = "user@dev.com";
    var basicUser = await userManager.FindByEmailAsync(basicEmail);
    if (basicUser == null)
    {
        basicUser = new User { UserName = basicEmail, Email = basicEmail, FullName = "Basic User", ProfileImageUrl = "" };
        var result = await userManager.CreateAsync(basicUser, "User123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(basicUser, "basic");
    }
}
