using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExampleWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExampleWeb.Controllers
{
    public class UploadController : Controller
    {
        private readonly string _tempFolder;

        private readonly ILogger<UploadController> _logger;

        public UploadController(ILogger<UploadController> logger)
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), "UploadTest");
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Regist(UploadTestModel uploadTestModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", uploadTestModel);
            }
            else
            {
                var fileList = uploadTestModel.FileList;
                SaveFiles(fileList);
                return Ok(new { fileList.Count });
            }
        }

        public IActionResult UploadByBuffered(List<IFormFile> fileList)
        {
            SaveFiles(fileList);
            return Ok();
        }

        private async void SaveFiles(List<IFormFile> fileList)
        {
            _logger.LogDebug($"count: {fileList.Count}");
            long size = fileList.Sum(f => f.Length);

            if (!Directory.Exists(_tempFolder))
            {
                Directory.CreateDirectory(_tempFolder);
            }

            string filePrefix = DateTime.Now.ToString("yyMMdd_HHmmss_fff_");
            for (int i = 0; i < fileList.Count; i++)
            {
                var formFile = fileList[i];
                var formFilename = formFile.FileName;
                var tmpFilename = filePrefix + i + ".tmp";
                var savePath = Path.Combine(_tempFolder, tmpFilename);
                using (var stream = System.IO.File.Create(savePath))
                {
                    await formFile.CopyToAsync(stream);
                }
                _logger.LogDebug($"saved[{i}]: {formFilename} -> {savePath}");
            }
        }
    }
}