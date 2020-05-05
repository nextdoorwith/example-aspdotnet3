using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExampleWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExampleWeb.Controllers
{
    public class ValidateBasicController : Controller
    {
        private readonly ILogger<ValidateBasicController> _logger;

        public ValidateBasicController(ILogger<ValidateBasicController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult Verify(string value)
        {
            _logger.LogDebug($"Verify(): {value}");

            // ダミーの実装
            if (string.IsNullOrEmpty(value) || !Regex.IsMatch(value, "0-9")) 
            {
                return Json($"数字を入力してください（サーバ検証）");
            }

            return Json(true);
        }

        [HttpPost]
        public IActionResult Regist(ValidateBasicModel model)
        {
            if( !ModelState.IsValid)
            {
                return View("Index", model);
            }

            return Json(model);
        }

    }
}