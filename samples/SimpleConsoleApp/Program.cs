using AspNetCore.TencentCos.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
                options.SecretId = configuration["TencentCos:SecretId"];
                options.SecretKey = configuration["TencentCos:SecretKey"];
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var cosHandler = serviceProvider.GetService<ITencentCosHandler>();

            var bucketsResult = await cosHandler.AllBucketsAsync();
            Console.WriteLine("Buckets:");
            Console.WriteLine(string.Join(Environment.NewLine, bucketsResult.Buckets.Select(x => new CosBucket(x.Name, x.Location).ToHttps(""))));

            string url = "";
            var uploadSuccess = false;
            Console.WriteLine("Please input local file path to upload:");
            var filePath = ReadLine();
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var cosBucket0 = new CosBucket(bucketsResult.Buckets[0]);
                Console.WriteLine($"Please input remote bucket host to upload({cosBucket0.ToHttps("")}):");
                var bucket = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(bucket))
                {
                    bucket = cosBucket0.ToHttps("").ToString();
                }

                Console.WriteLine("Please input remote folder to upload(/):");
                var folder = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(folder))
                {
                    folder = "/";
                }

                try
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var fileName = Path.GetFileName(filePath);
                        var putResultUri = await cosHandler.PutObjectAsync(bucket, folder, fileName, fs);
                        url = putResultUri.AbsoluteUri;

                        uploadSuccess = true;
                        Console.WriteLine("Succeeded to upload");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to upload. Reason:" + ex.Message);
                }
            }

            string url2 = "";
            //if (uploadSuccess)
            {
                Console.WriteLine($"Please input remote object url to download({url})...");
                url2 = ReadLine();
                if (string.IsNullOrWhiteSpace(url2))
                {
                    url2 = url;
                }

                Console.WriteLine("Please input local path to download...");
                var dlPath = Console.ReadLine();

                try
                {
                    await cosHandler.GetObjectAsync(new Uri(url2), stream =>
                    {
                        var remoteFilePath = new Uri(url2).LocalPath;
                        string dlFileName = Path.GetFileName(remoteFilePath);
                        var dlFilePath = Path.Combine(dlPath, dlFileName);
                        using (var fs = File.Create(dlFilePath))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.CopyTo(fs);
                        }
                        Console.WriteLine("Downloaded");
                    });

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to Download. Reason:" + ex.Message);
                }
            }

            string url3 = "";
            if (uploadSuccess)
            {
                Console.WriteLine($"Please input object url to delete({url2})...");
                url3 = ReadLine();
                if (string.IsNullOrWhiteSpace(url3))
                {
                    url3 = url2;
                }

                try
                {
                    await cosHandler.DeleteObjectAsync(new Uri(url3));
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

        private const int READLINE_BUFFER_SIZE = 1024;
        private static string ReadLine()
        {
            Stream inputStream = Console.OpenStandardInput(READLINE_BUFFER_SIZE);
            byte[] buff = new byte[READLINE_BUFFER_SIZE];
            Console.SetIn(new StreamReader(inputStream, Console.InputEncoding, false, buff.Length));
            string strInput = Console.ReadLine();
            return strInput;
            //int outputLength = inputStream.Read(bytes, 0, READLINE_BUFFER_SIZE);
            ////Console.WriteLine(outputLength);
            //char[] chars = Encoding.UTF8.GetChars(bytes, 0, outputLength);
            //return new string(chars).TrimEnd('\n').TrimEnd('\r');
        }
    }
}
