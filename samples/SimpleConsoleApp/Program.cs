using AspNetCore.TencentCos.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.TencentCos
{

    public class Program
    {
        public async static Task<List<CosBucket>> ListBuckets(ITencentCosHandler cosHandler)
        {
            var bucketsResult = await cosHandler.AllBucketsAsync();
            var result = bucketsResult.Buckets.Select(x => new CosBucket(x.Name, x.Location)).ToList();

            Console.WriteLine("Buckets:");
            Console.WriteLine(string.Join(Environment.NewLine, result.Select(x => x.ToHttps(""))));

            return result;
        }
        
        public async static Task<Uri> UploadObject(ITencentCosHandler cosHandler, CosBucket cosBucket0)
        {
            Uri resultUri = null;

            var filePath = ReadLineOrDefault("Please input local file path to upload:", "");
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var bucketUrl = ReadLineOrDefault($"Please input remote bucket host to upload({cosBucket0.ToHttps("")}):", cosBucket0.ToHttps("").ToString());
                var container = ReadLineOrDefault("Please input remote folder to upload(/):", "/");

                try
                {
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var fileName = Path.GetFileName(filePath);
                        resultUri = await cosHandler.PutObjectAsync(bucketUrl, container, fileName, fs);
                        Console.WriteLine("Succeeded to upload");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to upload. Reason:" + ex.Message);
                }
            }

            return resultUri;
        }

        private static async Task<Uri> DownloadObject(ITencentCosHandler cosHandler, Uri uri0)
        {
            Uri resultUri = null;

            var url = ReadLineOrDefault($"Please input remote object url to download({uri0})...", uri0.ToString());
            if (!string.IsNullOrWhiteSpace(url))
            {
                var localFolder = ReadLineOrDefault("Please input local path to download...", "");

                try
                {
                    await cosHandler.GetObjectAsync(new Uri(url), stream =>
                    {
                        var remoteFilePath = new Uri(url).LocalPath;
                        string dlFileName = Path.GetFileName(remoteFilePath);
                        var dlFilePath = Path.Combine(localFolder, dlFileName);
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

            return resultUri;
        }

        private static async Task<bool> DeleteObject(ITencentCosHandler cosHandler, Uri uri0)
        {
            var result = false;

            var url = ReadLineOrDefault($"Please input object url to delete({uri0})...", uri0.ToString());

            try
            {
                result = await cosHandler.DeleteObjectAsync(new Uri(url));
                Console.WriteLine("Deleted");
            }
            catch
            {
                Console.WriteLine("Failed to Delete");
            }

            return result;
        }

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

            services.AddTencentCos(options =>
            {
                options.SecretId = configuration["TencentCos:SecretId"];
                options.SecretKey = configuration["TencentCos:SecretKey"];
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var cosHandler = serviceProvider.GetService<ITencentCosHandler>();

            var buckets = await ListBuckets(cosHandler);

            var uploadUri = await UploadObject(cosHandler, buckets[0]);
            if (uploadUri == null) { return; }

            var downloadUri = await DownloadObject(cosHandler, uploadUri);
            if (downloadUri == null) { return; }

            var deleteResult = await DeleteObject(cosHandler, uploadUri);
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        #region ReadLineOrDefault
        private const int READLINE_BUFFER_SIZE = 1024;
        private static string ReadLine()
        {
            var inputStream = Console.OpenStandardInput(READLINE_BUFFER_SIZE);
            var buff = new byte[READLINE_BUFFER_SIZE];
            Console.SetIn(new StreamReader(inputStream, Console.InputEncoding, false, buff.Length));

            var strInput = Console.ReadLine();
            return strInput;
        }

        private static string ReadLineOrDefault(string prompt, string defaultValue)
        {
            Console.WriteLine(prompt);

            var result = ReadLine();
            if (string.IsNullOrWhiteSpace(result))
            {
                result = defaultValue;
            }
            return result;
        }
        #endregion
    }
}
