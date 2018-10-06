using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CosStore.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.IO;
using AspNetCore.TencentCos;

namespace CosStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly UploadOptions _uploadOptions;
        private readonly ITencentCosHandler _cosHandler;

        public HomeController(
            IOptions<UploadOptions> uploadOptionsAccessor,
            ITencentCosHandler cosHandler)
        {
            _uploadOptions = uploadOptionsAccessor?.Value ?? throw new ArgumentNullException(nameof(uploadOptionsAccessor));
            _cosHandler = cosHandler ?? throw new ArgumentNullException(nameof(cosHandler));
        }

        public IActionResult Upload()
        {
            return View();
        }

        private StatusCodeResult NotValidUpload() => BadRequest();

        [HttpPost("/cos/upload")]
        public async Task<StatusCodeResult> CosUpload(IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var uploadOptions = _uploadOptions;

            if (file.Length > uploadOptions.MaxLength)
                return NotValidUpload();

            string extension = Path.GetExtension(file.FileName);
            if (extension == null)
                return NotValidUpload();

            extension = extension.ToLowerInvariant();
            if (!uploadOptions.SupportedExtensions.Contains(extension))
                return NotValidUpload();

            var storageUri = uploadOptions.StorageUri;
            var containerExists = await _cosHandler.ExistsAsync(storageUri);
            if (!containerExists)
                return NotValidUpload();

            var filePath = new Uri(storageUri).Append(file.FileName);
            var fileExists = await _cosHandler.ExistsAsync(filePath.ToString());
            if (fileExists && !uploadOptions.IsOverrideEnabled)
                return NotValidUpload();

            var uploadedUri = await _cosHandler.PutObjectAsync(filePath.ToString(), file.OpenReadStream());

            return Ok();
        }

        [HttpPost("/files/upload")]
        public StatusCodeResult FileUpload(IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var uploadOptions = _uploadOptions;

            if (file.Length > uploadOptions.MaxLength)
                return NotValidUpload();

            string extension = Path.GetExtension(file.FileName);
            if (extension == null)
                return NotValidUpload();

            extension = extension.ToLowerInvariant();
            if (!uploadOptions.SupportedExtensions.Contains(extension))
                return NotValidUpload();

            if (!Directory.Exists(uploadOptions.StoragePath))
                Directory.CreateDirectory(uploadOptions.StoragePath);

            string filePath = Path.Combine(uploadOptions.StoragePath, file.FileName);
            if (System.IO.File.Exists(filePath))
            {
                if (uploadOptions.IsOverrideEnabled)
                    System.IO.File.Delete(filePath);
                else
                    return NotValidUpload();
            }

            using (Stream fileContent = new FileStream(filePath, FileMode.OpenOrCreate))
                file.CopyTo(fileContent);

            return Ok();
        }

        [HttpGet("/files/{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var uploadOptions = _uploadOptions;

            if (fileName.Contains(Path.DirectorySeparatorChar) || fileName.Contains(Path.AltDirectorySeparatorChar) || fileName.Contains("..") || Path.IsPathRooted(fileName))
                NotFound();

            string extension = Path.GetExtension(fileName);
            if (extension == null)
                NotFound();

            extension = extension.ToLowerInvariant();
            if (!uploadOptions.SupportedExtensions.Contains(extension))
                return NotFound();

            string filePath = Path.Combine(uploadOptions.StoragePath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                var contentType = ContentTypeHelper.GetContentType(extension);
                return File(new FileStream(filePath, FileMode.Open), contentType);
            }

            return NotFound();
        }

        [HttpGet("/cos/{fileName}/{type?}")]
        public async Task<IActionResult> GetCloudObject(string fileName, string type)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var uploadOptions = _uploadOptions;

            var storageUri = _uploadOptions.StorageUri;
            var fileUri = new Uri(storageUri).Append(fileName);

            string extension = Path.GetExtension(fileName);
            if (extension == null)
                return NotFound();

            extension = extension.ToLowerInvariant();
            if (!uploadOptions.SupportedExtensions.Contains(extension))
                return NotFound();

            var exists = await _cosHandler.ExistsAsync(fileUri.ToString());
            if (!exists)
                return NotFound();

            var contentType = ContentTypeHelper.GetContentType(extension);

            if (string.IsNullOrWhiteSpace(type)) type = "1";
            switch (type)
            {
                case "1":
                    {
                        var downloaded = await _cosHandler.GetObjectAsync(fileUri.ToString(), stream =>
                        {
                        });
                        return File(downloaded, contentType);
                    }
                case "2":
                    {
                        var downloaded = await _cosHandler.GetObjectAsync(fileUri.ToString(), stream =>
                        {
                        });

                        var len = downloaded.Length;
                        var bytes = new byte[len];
                        downloaded.Read(bytes, 0, (int)len);
                        return new FileContentResult(bytes, contentType);
                    }
                case "3":
                    {
                        FileStreamResult fs = null;
                        await _cosHandler.GetObjectAsync(fileUri.ToString(), stream =>
                        {
                            fs = new FileStreamResult(stream, contentType);
                        });
                        return fs;
                    }
                case "4":
                default:
                    {
                        FileContentResult fs = null;
                        await _cosHandler.GetObjectAsync(fileUri.ToString(), stream =>
                        {
                            var len = stream.Length;
                            var bytes = new byte[len];
                            stream.Read(bytes, 0, (int)len);
                            fs = new FileContentResult(bytes, contentType);
                        });
                        return fs;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">fileUri</param>
        /// <param name="type">1/2/3/4</param>
        /// <returns></returns>
        [HttpGet("/cos/open/{url}/{type?}")]
        public async Task<IActionResult> OpenCloudObject(string url, string type)
        {
            url = Uri.UnescapeDataString(url);

            string extension = Path.GetExtension(url);
            if (extension == null)
                return NotFound();
            extension = extension.ToLowerInvariant();

            var exists = await _cosHandler.ExistsAsync(url.ToString());
            if (!exists)
                return NotFound();

            var contentType = ContentTypeHelper.GetContentType(extension);

            if (string.IsNullOrWhiteSpace(type)) type = "1";
            switch (type)
            {
                case "1":
                    {
                        var downloaded = await _cosHandler.GetObjectAsync(url.ToString(), stream =>
                        {
                        });
                        return File(downloaded, contentType);
                    }
                case "2":
                    {
                        var downloaded = await _cosHandler.GetObjectAsync(url.ToString(), stream =>
                        {
                        });

                        var len = downloaded.Length;
                        var bytes = new byte[len];
                        downloaded.Read(bytes, 0, (int)len);
                        return new FileContentResult(bytes, contentType);
                    }
                case "3":
                    {
                        FileStreamResult fs = null;
                        await _cosHandler.GetObjectAsync(url.ToString(), stream =>
                        {
                            fs = new FileStreamResult(stream, contentType);
                        });
                        return fs;
                    }
                case "4":
                default:
                    {
                        FileContentResult fs = null;
                        await _cosHandler.GetObjectAsync(url.ToString(), stream =>
                        {
                            var len = stream.Length;
                            var bytes = new byte[len];
                            stream.Read(bytes, 0, (int)len);
                            fs = new FileContentResult(bytes, contentType);
                        });
                        return fs;
                    }
            }
        }

        [HttpGet("/cos/performance/{times}/{fileName}")]
        public IActionResult PerformanceOfGetCloudObject(string fileName, int times)
        {
            var results = new Dictionary<int, double>();

            // warming up
            for (int type = 1; type <= 4; type++)
            {
                GetCloudObject(fileName, type.ToString()).Wait();
            }

            // counting
            var stopwatch = new Stopwatch();
            for (int type = 1; type <= 4; type++)
            {
                stopwatch.Restart();
                for (int i = 0; i < times; i++)
                {
                    GetCloudObject(fileName, type.ToString()).Wait();
                }
                stopwatch.Stop();
                var result = (double)stopwatch.ElapsedMilliseconds / times;
                results.Add(type, result);
            }

            return Json(new { FileName = fileName, Times = times, Result = results.OrderBy(x => x.Key) });
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/buckets")]
        public async Task<IActionResult> AllBuckets()
        {
            var buckets = await _cosHandler.AllBucketsAsync();
            return View(buckets);
        }

        [HttpGet("/buckets/{url}")]
        public async Task<IActionResult> AllObjects(string url)
        {
            url = Uri.UnescapeDataString(url);
            ViewData["BucketUrl"] = url;

            var cloudObjects = await _cosHandler.AllObjectsAsync(url, "", "");
            return View(cloudObjects);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
