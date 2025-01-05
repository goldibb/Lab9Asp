using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Webapp.Models.Movies;
using Webapp.Models.Superheroes;
using WebApp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Webapp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddMemoryCache();
        builder.Services.AddSession();
        builder.Services.AddControllersWithViews();

        // Register MoviesDbContext
        builder.Services.AddDbContext<MoviesDbContext>(op =>
        {
            op.UseSqlite(builder.Configuration["MoviesDatabase:ConnectionString"]);
        });

        // Register AppDbContext
        builder.Services.AddDbContext<AppDbContext>(op =>
        {
            op.UseSqlite(builder.Configuration["AccountDatabase:ConnectionString"]);
        });

        // Register SuperheroesContext with Identity
        builder.Services.AddDbContext<SuperheroesContext>(options =>
        {
            options.UseSqlite(builder.Configuration["SuperheroDatabase:ConnectionString"]);
        });

        // Add Identity services
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        // Register IEmailSender service
        builder.Services.AddTransient<IEmailSender, EmailSender>();

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
        app.UseSession();
        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

// Simple implementation of IEmailSender
public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Implement your email sending logic here
        return Task.CompletedTask;
    }
}