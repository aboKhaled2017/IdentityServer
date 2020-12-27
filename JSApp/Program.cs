using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JSApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((context,services) =>
            {
                HostConfig.CertPath = context.Configuration["CertPath"];
                HostConfig.CertPassword = context.Configuration["CertPassword"];
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(c =>
                    {
                        var host = Dns.GetHostEntry("abokhaled.io");
                        /*c.ListenAnyIP(1200);
                        c.ListenAnyIP(1201, l =>
                        {
                            l.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
                        });*/
                        c.Listen(host.AddressList[0],1200);
                        c.Listen(host.AddressList[0], 1201, l =>
                         {
                             l.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
                         });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
    public static class HostConfig
    {
        public static string CertPath { get; set; }
        public static string CertPassword { get; set; }
    }
}
