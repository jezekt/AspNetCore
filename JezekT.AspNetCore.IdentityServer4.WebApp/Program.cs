using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace JezekT.AspNetCore.IdentityServer4.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer";

            var cert = new X509Certificate2(Path.Combine("c:/users/jezek/desktop", "identity.local.net.pfx"), "best");

            var host = new WebHostBuilder()
                .UseKestrel(cfg => cfg.UseHttps(cert))
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
