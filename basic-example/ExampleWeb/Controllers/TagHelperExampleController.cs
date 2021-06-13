using ExampleWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Controllers
{
    public class TagHelperExampleController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new TagHelperExampleModel());
        }

        [HttpPost]
        public IActionResult Index(TagHelperExampleModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return View(model);
        }
    }
}
