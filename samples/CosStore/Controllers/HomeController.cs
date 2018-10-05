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
                string contentType = "application/octet-stream";
                if (extension == ".gif")
                    contentType = "image/gif";
                else if (extension == ".png")
                    contentType = "image/png";
                else if (extension == ".jpg" || extension == ".jpeg")
                    contentType = "image/jpg";

                return File(new FileStream(filePath, FileMode.Open), contentType);
            }

            return NotFound();
        }

        public IActionResult Index()
        {
            return View();
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
