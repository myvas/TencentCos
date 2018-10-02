using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.TencentCos
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();

            IServiceCollection services = new ServiceCollection();
            services.AddLogging(builder => builder
                .AddConfiguration(configuration.GetSection("Logging"))
                .AddConsole());

            //services.AddOptions();
            services.AddTencentCos(options =>
            {
                options.AppId = configuration["TencentCos:AppId"];
                options.SecretId = configuration["TencentCos:SecretId"];
                options.SecretKey = configuration["TencentCos:SecretKey"];
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var cosHandler = serviceProvider.GetService<ITencentCosHandler>();

            var buckets = await cosHandler.ListBucketsAsync();
            Console.WriteLine("Buckets:");
            Console.WriteLine(string.Join(";", buckets.Select(x => x.ToString())));

            var uploadSuccess = false;
            Console.WriteLine("Please input file path:");
            var filePath = Console.ReadLine();
            Console.WriteLine("Please input destination:");
            var url = Console.ReadLine();
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    await cosHandler.PutObjectAsync(url, fs);

                    uploadSuccess = true;
                    Console.WriteLine("Succeeded to upload");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to upload. Reason:" + ex.StackTrace);
                }
            }

            if (uploadSuccess)
            {
                Console.WriteLine("Press any key to delete...");
                Console.ReadKey();

                try
                {
                    await cosHandler.DeleteObjectAsync(url);
                    Console.WriteLine("Deleted");
                }
                catch
                {
                    Console.WriteLine("Failed to Delete");
                }
            }


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
