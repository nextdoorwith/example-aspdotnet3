using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace HttpClientSimple.Controllers
{
    public class HomeController : Controller
    {

        private readonly IHttpClientFactory _factory;

        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IHttpClientFactory factory, 
            ILogger<HomeController> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DoGet()
        {
            // HTTP要求の実行
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = 
                await client.GetAsync("https://localhost:44372/api/ApiTest?param1=abc&param2=xyz");

            // ApiTestから取得した応答をテキストとして応答
            return Content(await response.Content.ReadAsStringAsync(), "text/plain");
        }

        public async Task<IActionResult> DoPost()
        {
            // フォームデータの作成
            var values = new Dictionary<string, string>();
            values.Add("param1", "123");
            values.Add("param2", "234");
            HttpContent content = new FormUrlEncodedContent(values);

            // HTTP要求の実行
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response =
                await client.PostAsync("https://localhost:44372/api/ApiTest", content);

            // ApiTestから取得した応答をテキストとして応答
            string responseBody = await response.Content.ReadAsStringAsync();
            return Content(responseBody, "text/plain");
        }

    }
}
