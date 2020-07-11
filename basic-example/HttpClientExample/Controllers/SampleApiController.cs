using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HttpClientExample.Controllers
{
    [Route("api/[controller]")]
    public class SampleApiController : Controller
    {
        private readonly ILogger<SampleApiController> _logger;

        public SampleApiController(ILogger<SampleApiController> logger)
        {
            _logger = logger;
        }

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Get() is invoked!");

            // サンプルデータの作成
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("key1", "value1");
            dic.Add("key2", "value2");

            // JSON形式で返却
            return Json(dic);
        }

        // POST api/<controller>
        [Route("post-json")]
        [HttpPost]
        public async Task<IActionResult> PostJson()
        {
            _logger.LogInformation("PostJson() is invoked!");

            // HTTP要求のボディからJSONを復元して値を追加
            var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            Dictionary<string, string> dic =
                JsonSerializer.Deserialize<Dictionary<string, string>>(body);
            dic.Add("serverkey1", "servervalue1");

            // JSON形式で返却
            return Json(dic);
        }

        [Route("post-only")]
        [HttpPost]
        public IActionResult PostOnly()
        {
            return Ok();
        }

    }
}
