using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MVC
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(c=> {
                c.DefaultScheme = "MvcCookie";
                c.DefaultChallengeScheme = "oidc";              
            })
                .AddCookie("MvcCookie",c=> {
                    c.LoginPath = "/aa";
                    //c.ExpireTimeSpan = TimeSpan.FromSeconds(10);
                    //c.Cookie.MaxAge = TimeSpan.FromSeconds(4);
                })
                .AddOpenIdConnect("oidc", c => {
                    c.Authority = Constants.IdentityServerBaseAdress;
                    c.ResponseType = "code";
                    c.SaveTokens = true;
                    c.ClientId = "MvcClient_id";
                    c.ClientSecret = "MvcClient_Secret";
                    c.Scope.Add("privilages.scope");
                    c.Scope.Add("ApiOne");
                    c.Scope.Add("offline_access");
                    c.GetClaimsFromUserInfoEndpoint = true;
                    c.ClaimActions.MapUniqueJsonKey("get", "get");
                    c.ClaimActions.MapUniqueJsonKey("add", "add");
                    c.ClaimActions.MapUniqueJsonKey("update", "update");
                    c.ClaimActions.MapUniqueJsonKey("delete", "delete");
                    c.ClaimActions.MapUniqueJsonKey(ClaimTypes.Role, ClaimTypes.Role);
                });
            services.AddAuthorization(c =>
            {
                c.AddPolicy("get", p => {
                    p.RequireAssertion(ctx =>
                    {
                        return ctx.User.HasClaim(ClaimTypes.Role, "admin")
                        || ctx.User.HasClaim(c => c.Type == "get" && c.Value == "true");
                    });
                });
                c.AddPolicy("add", p => {
                    p.RequireAssertion(ctx =>
                    {
                        return ctx.User.HasClaim(ClaimTypes.Role, "admin")
                        || ctx.User.HasClaim(c => c.Type == "add" && c.Value == "true");
                    });
                });
                c.AddPolicy("get_and_add", p =>
                {                    
                    p.RequireClaim("get", "true")
                    .RequireClaim("add", "true");
                });
                c.AddPolicy("admin", p => {
                    p.RequireClaim(ClaimTypes.Role);
                });
                c.AddPolicy("add", p => p.RequireClaim("add", "true"));
                c.AddPolicy("update", p => p.RequireClaim("update", "true"));
                c.AddPolicy("delete", p => p.RequireClaim("delete", "true"));
                
            });

            services.AddHttpClient(Constants.AppsNames.APIOneApp, c => {
                c.BaseAddress = new Uri(Constants.APIOneBaseAdress);
            });
            services.AddHttpClient(Constants.AppsNames.IdentityServerApp, c => {
                c.BaseAddress = new Uri(Constants.IdentityServerBaseAdress);
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/aa", async ctx => await ctx.Response.WriteAsync("welcom to mvc login page"));
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
