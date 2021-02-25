using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CARLA_Hennery.Master.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CARLA_Hennery.Master
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();

            // Startup carla version capture
            var carlaVersionCapture = scope.ServiceProvider.GetRequiredService<CarlaVersionCaptureService>();
            carlaVersionCapture.Run();
            
            // Startup python version capture
            var pythonVersionCapture = scope.ServiceProvider.GetRequiredService<PythonVersionCaptureService>();
            pythonVersionCapture.Run();
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}