using Demo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Myvas.AspNetCore.TencentCos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerDemo.Controllers
{
	public class OpenCloudObjectController : Controller
	{
		private readonly CosUploadOptions _cosUploadOptions;
		private readonly ITencentCosHandler _cosHandler;

		public OpenCloudObjectController(
			IOptions<CosUploadOptions> cosUploadOptionsAccessor,
			ITencentCosHandler cosHandler)
		{
			_cosUploadOptions = cosUploadOptionsAccessor?.Value ?? throw new ArgumentNullException(nameof(cosUploadOptionsAccessor));
			_cosHandler = cosHandler ?? throw new ArgumentNullException(nameof(cosHandler));
		}

		[HttpGet("/cos/{fileName}/{type?}")]
		public async Task<IActionResult> GetCloudObject(string fileName, string type)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentNullException(nameof(fileName));
			}

			var uploadOptions = _cosUploadOptions;

			var storageUri = uploadOptions.CosStorageUri;
			var fileUri = new Uri(new Uri(storageUri), fileName);

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
		[HttpGet("/cos/open")]
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

	}
}