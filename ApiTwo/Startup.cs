using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Common;

namespace ApiTwo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("jwt")
                .AddJwtBearer("jwt", c => {
                    //c.Authority = Constants.IdentityServerBaseAdress;
                    //c.Audience = "ApiTwo";
                });
            services.AddHttpClient(Constants.AppsNames.APIOneApp,c=> {
                c.BaseAddress = new Uri(Constants.APIOneBaseAdress);
            });
            services.AddHttpClient(Constants.AppsNames.IdentityServerApp, c => {
                c.BaseAddress = new Uri(Constants.IdentityServerBaseAdress);
            });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", ctx =>
                {
                    ctx.Response.WriteAsync("Wlcom to APi Two");
                    return Task.CompletedTask;
                });
                endpoints.MapControllers();
            });
        }
    }
}
