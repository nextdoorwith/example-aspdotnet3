using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Controllers
{
    public class CheckboxExampleController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
