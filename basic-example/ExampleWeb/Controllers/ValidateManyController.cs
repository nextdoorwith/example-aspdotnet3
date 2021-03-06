﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleWeb.Controllers
{
    public class ValidateManyController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Regist(ValidateManyModel personDetail)
        {
            if( !ModelState.IsValid)
            {
                ModelState.AddModelError("", "エラーがあります。");
                return View("Index", personDetail);
            }

            return Ok();
        }
    }
}