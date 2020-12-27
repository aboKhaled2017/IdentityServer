using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ApiOne
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("jwt")
                .AddJwtBearer("jwt", c => {
                    c.Authority = Constants.IdentityServerBaseAdress;
                    c.Audience = "ApiOne";
                    
                    c.TokenValidationParameters = new TokenValidationParameters
                    {
                        AudienceValidator=(
                        IEnumerable<string> audiences, 
                        SecurityToken securityToken, 
                        TokenValidationParameters validationParameters) =>
                        {
                            /*var tokenStr = new JwtSecurityTokenHandler().WriteToken(securityToken);
                            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenStr);
                            
                            if (token.Claims.Any(c => c.Type == "scope" && c.Value == "ApiOne"))
                                return true;*/
                            //accepts all tokens from identity server
                            return true;
                        },
                        ValidAudiences =new string [] {"ApiTwo","MvcClient_id"}
                       // ValidateAudience=false
                    };
                    /*c.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            if(ctx.Request.Query.TryGetValue("token",out var token) && token.ToString().Length>3)
                            {
                                ctx.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };*/
                });
            services.AddAuthorization(configure =>
            {
                //we can apply it [globally | specified controller/action | all endpintes]
                configure.AddPolicy("scope", c => {
                    c.RequireAuthenticatedUser();
                    c.RequireClaim("scope", "ApiOne");
                    
                });
            });
           
            services.AddControllers(c=> {
                //apply policy globally
                c.Filters.Add(new AuthorizeFilter("scope"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", ctx =>
                {
                    ctx.Response.WriteAsync("Welcom to Api One");
                    return Task.CompletedTask;
                });
                endpoints.MapControllers();
            });
        }
    }
}
