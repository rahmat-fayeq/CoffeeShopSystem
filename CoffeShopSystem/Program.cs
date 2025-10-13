using AspNetCoreHero.ToastNotification;
using CoffeShopSystem.Data;
using CoffeShopSystem.Profiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Auto Mapper Configurations
builder.Services.AddAutoMapper(cfg => {}, typeof(AutoMapperProfile).Assembly);

// Notification
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 2;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopCenter;
    config.HasRippleEffect = true;
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Block access to unused Identity pages by returning 404 Not Found
var blockedRoutes = new[]
{
    "/Identity/Account/Register",
    "/Identity/Account/ConfirmEmail",
    "/Identity/Account/ForgotPassword",
    "/Identity/Account/ForgotPasswordConfirmation",
    "/Identity/Account/ResetPassword",
    "/Identity/Account/ResetPasswordConfirmation",

    "/Identity/Account/Manage/Email",
    "/Identity/Account/Manage/ExternalLogins",
    "/Identity/Account/Manage/PersonalData",
    "/Identity/Account/Manage/TwoFactorAuthentication",
    "/Identity/Account/Manage/EnableAuthenticator",
    "/Identity/Account/Manage/Disable2fa",
    "/Identity/Account/Manage/GenerateRecoveryCodes",
    "/Identity/Account/Manage/RecoveryCodes",
    "/Identity/Account/Manage/DownloadPersonalData",
    "/Identity/Account/Manage/DeletePersonalData"
};

foreach (var route in blockedRoutes)
{
    app.MapGet(route, () => Results.NotFound());
    app.MapPost(route, () => Results.NotFound());
}

// Seed to Db
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}

app.Run();
