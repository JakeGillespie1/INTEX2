using INTEX2.Data;
using INTEX2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using System.Configuration;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var config = builder.Configuration;


        // Add services to the container.
        //Add Connections from context files to sqlserver
        var connectionString = builder.Configuration.GetConnectionString("INTEX2db") ?? throw new InvalidOperationException("Connection string 'INTEX2db' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDbContext<INTEX2Context>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<AppUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.User.RequireUniqueEmail = true;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();


        //Register Custom Tools and User Classes with the program.
        builder.Services.AddScoped<Tools>();
        builder.Services.AddScoped<IINTEX2Repository, EFINTEX2Repository>();
        builder.Services.AddRazorPages();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseSession();

        app.UseRouting();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();
        app.MapControllers();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        //When we create a scope, we are able to access the services that we have configured in the identity
        using (var scope = app.Services.CreateScope())
        {
            //Add your roles here (seeding some initial data into our system)
            //var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //var roles = new[] { "Admin", "Member" };

            //foreach (var role in roles)
            //{
            //    //Check if a role exists
            //    if (!await roleManager.RoleExistsAsync(role))
            //        await roleManager.CreateAsync(new IdentityRole(role));
            //}

        }

        //It will be good to seed some default users
        using (var scope = app.Services.CreateScope())
        {
            //Add your users here (seeding some initial data into our system)
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            string email = "admin@admin.com";
            string password = "Test1234,56789!";

            //Seed a new admin user if there is no data available
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new AppUser();
                user.UserName = email;
                user.Email = email;
                user.EmailConfirmed = true;
                user.FirstName = "Admin";
                user.LastName = "Admin";

                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, "Admin");
            }


        }
        app.Run();
    }
}