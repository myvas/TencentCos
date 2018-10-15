using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var webhostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())

                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureLogging(ConfigureLogging)

                .UseIISIntegration()
                .UseDefaultServiceProvider(DefaultServiceProvider);

            var webHost = webhostBuilder
                .CaptureStartupErrors(true)
                .UseStartup<Startup>();

            return webHost;
        }

        private static void ConfigureAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var env = hostingContext.HostingEnvironment;
            var environmentName = env.EnvironmentName;

            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddUserSecrets<Startup>()
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
        }
        
        private static void ConfigureLogging(WebHostBuilderContext hostingContext, ILoggingBuilder logging)
        {
            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            logging.AddConsole();
            logging.AddDebug();
        }

        private static void DefaultServiceProvider(WebHostBuilderContext hostingContext, ServiceProviderOptions options)
        {
            // To detect: InvalidOperationException: Cannot consume scoped service 'Ruhu.AppDbContext' from singleton 'Microsoft.AspNetCore.Authorization.IAuthorizationHandler'.
            options.ValidateScopes = hostingContext.HostingEnvironment.IsDevelopment();
        }
    }
}
