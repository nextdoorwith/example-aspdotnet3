using ExampleWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleWeb.Controllers
{
    public class ValidateSummaryController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Regist(ValidateSummaryModel model)
        {
            if( !ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "モデルエラーです。");
                ModelState.AddModelError("prop5.in2_val", "個別指定のエラーです。");
                ModelState.AddModelError("noexists", "項目が紐づかないエラーです。");
                return View("Index", model);
            }

            return Ok();
        }
    }
}