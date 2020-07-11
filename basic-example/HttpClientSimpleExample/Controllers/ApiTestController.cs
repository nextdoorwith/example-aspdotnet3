using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HttpClientSimple.Controllers
{
    [Route("api/[controller]")]
    public class ApiTestController : Controller
    {
        private ILogger<ApiTestController> _logger;

        public ApiTestController(ILogger<ApiTestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery]string param1, [FromQuery]string param2)
        {
            _logger.LogDebug("Get() is invoked!");

            var dic = new Dictionary<string, string>();
            dic.Add("from", "ApiTest.Get()");
            dic.Add("param1", param1);
            dic.Add("param2", param2);
            return Json(dic);
        }

        [HttpPost]
        public IActionResult Post([FromForm]string param1, [FromForm]string param2)
        {
            _logger.LogDebug("Post() is invoked!");

            var dic = new Dictionary<string, string>();
            dic.Add("from", "ApiTest.Post()");
            dic.Add("param1", param1);
            dic.Add("param2", param2);
            return Json(dic);
        }

    }
}
