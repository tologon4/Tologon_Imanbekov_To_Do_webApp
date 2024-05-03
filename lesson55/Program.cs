using lesson55.Models;
using lesson55.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        string connection = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<MyTaskContext>(options => options.UseNpgsql(connection))
            .AddIdentity<User, IdentityRole<int>>()
            .AddEntityFrameworkStores<MyTaskContext>();

        var app = builder.Build();
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var rolesManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await AdminInitializer.SeedAdminUser(rolesManager, userManager);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }

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

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=LogIn}/{id?}");

        app.Run();
    }
}


