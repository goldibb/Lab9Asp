using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Webapp.Models.Movies;
using Webapp.Models.Superheroes;

namespace Webapp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Register MoviesDbContext
        builder.Services.AddDbContext<MoviesDbContext>(op =>
        {
            op.UseSqlite(builder.Configuration["MoviesDatabase:ConnectionString"]);
        });
        
        // Register SuperheroesContext with Identity
        builder.Services.AddDbContext<SuperheroesContext>(options =>
        {
            options.UseSqlite(builder.Configuration["SuperheroDatabase:ConnectionString"]);
        });

        // Add Identity services
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Password settings if you want to customize
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<SuperheroesContext>()
            .AddDefaultTokenProviders();

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

        // Add these lines for authentication
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}