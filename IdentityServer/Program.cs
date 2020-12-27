using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope=host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityServerUser>>();
                if (!userManager.Users.Any())
                {
                    var user = new IdentityServerUser { UserName = "mohamed" };
                    userManager.CreateAsync(user, "123").Wait();
                    var res=userManager.AddClaimsAsync(user,new List<Claim> {
                     new Claim("get", "true"),
                     new Claim("add", "true"),
                     new Claim("update", "true"),
                     new Claim("delete", "true"),
                      new Claim(ClaimTypes.Role, "admin")
                    })
                        .GetAwaiter()
                        .GetResult();
                }
            }
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
