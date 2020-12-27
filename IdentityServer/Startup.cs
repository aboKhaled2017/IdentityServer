using Microsoft.EntityFrameworkCore;
using IdentityServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IdentityServerDb>(c =>
            {
                c.UseSqlServer(Configuration.GetConnectionString("IdentityServerDb"), s =>
                {
                    s.MigrationsAssembly(typeof(Startup).Assembly.FullName);
                });
            });

            

            services.AddIdentity<IdentityServerUser, IdentityRole<Guid>>(c=> {
                c.Password.RequireDigit = false;
                c.Password.RequiredLength = 3;
                c.Password.RequireLowercase = false;
                c.Password.RequireNonAlphanumeric = false;
                c.Password.RequireUppercase = false;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<IdentityServerDb>();


            services.ConfigureApplicationCookie(c => {
                c.Cookie.Name = "IdentityServer.Cookie";
                c.LoginPath = "/Home/Login";
                //c.ExpireTimeSpan = TimeSpan.FromSeconds(5);
                //c.Cookie.MaxAge = TimeSpan.FromSeconds(5);
                //c.Cookie.Expiration = TimeSpan.FromSeconds(15);
            });

            
            services.AddIdentityServer(c=> {
                //c.UserInteraction.ConsentUrl = "/Home/Consent";
            })
                .AddAspNetIdentity<IdentityServerUser>()
                .AddInMemoryApiResources(IDPConfiguration.GetApiResources())
                .AddInMemoryApiScopes(IDPConfiguration.GetApiScopes())
                .AddInMemoryIdentityResources(IDPConfiguration.GetIdentityResources())
                .AddInMemoryClients(IDPConfiguration.GetClients())
             
                .AddDeveloperSigningCredential();


            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
