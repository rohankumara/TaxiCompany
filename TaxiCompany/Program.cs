using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaxiCompany.Data;

namespace TaxiCompany
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var Scope = host.Services.CreateScope())
            {
                var services = Scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                    var config = host.Services.GetRequiredService<IConfiguration>();

                    var testuserPW = config["SeedUserPW"];

                    if (String.IsNullOrEmpty(testuserPW))
                    {
                        throw new System.Exception("Use secrets manager to set SeedUserPW \n" + "dotnet user-secrets set SeedUserPW <pw>");
                    }

                   // SeedData.SeedDB(context,testuserPW);
                    SeedData.Initialize(services, testuserPW).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured while seeding the database.");
                }
            }
            host.Run();
        }
        
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
