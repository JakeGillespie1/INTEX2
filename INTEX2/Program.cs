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
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

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

        ////Register ONNX runtime model
        //const string modelPath = "gradient_boost_model.onnx";
        //builder.Services.AddSingleton<InferenceSession>(
        //    new InferenceSession(modelPath)
        //    );

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

        ////CSP Header and extra security stuff
        //app.Use(async (context, next) =>
        //{
        //    // Define the base CSP directive.
        //    var csp = "default-src 'self'; " +
        //              "script-src 'self' 'unsafe-inline' https://apis.google.com; " +
        //              "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        //              "img-src 'self' https://inteximg.s3.amazonaws.com; " +
        //              "font-src 'self' https://fonts.gstatic.com; " +
        //              "default-src 'self' https://login.microsoftonline.com; "+
        //              "frame-ancestors 'self' https://login.microsoftonline.com;";

        //    // Modify CSP for development environment.
        //    if (context.Request.Host.Host.Contains("localhost"))
        //    {
        //        csp += "connect-src 'self' ws://localhost:* http://localhost:*; ";
        //    }

        //    // Append the CSP header to the response.
        //    context.Response.Headers.Append("Content-Security-Policy", csp);

        //    // This is for the extra section for 414-- we have added the X-Content-Type-Options and X-Frame-Options
        //    // Add other security headers to the response.
        //    // (1)  instructs browsers not to perform MIME type sniffing, reducing the risk of certain types of attacks
        //    context.Response.Headers.Add("X-Content-Type-Options", "nosniff"); // 
        //                                                                       // (2)  prevents the page from being embedded in frames from different origins, mitigating clickjacking attacks
        //    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");

        //    await next.Invoke(); // Continue processing the request.
        //});

        app.UseAuthorization();

        //app.UseEndpoints(endpoints =>
        //{
        //    endpoints.MapControllerRoute(
        //        name: "productDetail",
        //        pattern: "product/{id}",
        //        defaults: new { controller = "Home", action = "ProductDetail" }
        //    );

        //    endpoints.MapControllerRoute(
        //       name: "ProductsPage",
        //       pattern: "Home/ProductsPage/{page}/{pageSize}",
        //       defaults: new { controller = "Home", action = "ProductsPage" }
        //   );

        //    // Additional routes can be defined here...
        //});
        app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();
        app.MapControllers();
        app.UseHttpsRedirection();
        app.UseAuthentication();

        //When we create a scope, we are able to access the services that we have configured in the identity
        using (var scope = app.Services.CreateScope())
        {
            //Add your roles here(seeding some initial data into our system)
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "Member" };

            foreach (var role in roles)
            {
                //Check if a role exists
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

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