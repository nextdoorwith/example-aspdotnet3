using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HttpClientTest.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;

namespace HttpClientTest.Controllers
{
    public class HomeController : Controller
    {
        private const string MIME_APP_JSON = "application/json";

        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _factory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        public IActionResult Index()
        {
            return View();
        }

        // (参考)
        // HttpRequestMessage.Content プロパティ:
        // https://docs.microsoft.com/ja-jp/dotnet/api/system.net.http.httprequestmessage.content?view=netcore-3.1#System_Net_Http_HttpRequestMessage_Content

        public async Task<IActionResult> DoBasicGet()
        {
            // HTTP要求の送信
            // (HTTPヘッダを指定できない)
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = await client.GetAsync("/api/SampleApi");
            if (!response.IsSuccessStatusCode)
            {
                return Json(false);
            }

            // 応答データの取得
            string recvJson = await response.Content.ReadAsStringAsync();

            return Content(recvJson, MIME_APP_JSON); // JSON形式のテキストを変換せずに応答
        }

        public async Task<IActionResult> DoGenericGet()
        {
            // SendAsyncは汎用的なメソッド、GetAsync/PostAyncはコンビニエントメソッド

            // HTTP要求の作成
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/SampleApi");
            request.Headers.Add("X-KEY", "value");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(MIME_APP_JSON));

            // HTTP要求の送信
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return Json(false);
            }

            // 応答データの取得
            string recvJson = await response.Content.ReadAsStringAsync();

            return Content(recvJson, MIME_APP_JSON); // JSON形式のテキストを変換せずに応答
        }

        public async Task<IActionResult> DoBasicPost()
        {
            // 送信データを作成
            string sendJson = CreateSampleJson();

            // HTTP要求(HttpContent)の作成
            StringContent content = new StringContent(sendJson, Encoding.UTF8, MIME_APP_JSON);
            content.Headers.Add("X-KEY", "value");

            // HTTP要求の送信
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = await client.PostAsync("/api/SampleApi/post-json", content);
            if (!response.IsSuccessStatusCode)
            {
                return Json(false);
            }

            // 応答データの取得
            string recvJson = await response.Content.ReadAsStringAsync();

            return Content(recvJson, MIME_APP_JSON); // JSON形式のテキストを変換せずに応答
        }

        public async Task<IActionResult> DoGenericPost()
        {
            // 送信データを作成
            string sendJson = CreateSampleJson();

            // HTTP要求の作成
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/SampleApi/post-json");
            request.Headers.Add("X-KEY", "value");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(MIME_APP_JSON));
            request.Content = new StringContent(sendJson, Encoding.UTF8, MIME_APP_JSON);

            // HTTP要求の送信
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            _logger.LogDebug($"Response Code: \n{response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                return Json(false);
            }

            // 応答データの取得
            string recvJson = await response.Content.ReadAsStringAsync();

            return Content(recvJson, MIME_APP_JSON); // JSON形式のテキストを変換せずに応答
        }

        public IActionResult DoCheckContentType()
        {
            string POST_ONLY_URI = "/api/SampleApi/post-only";

            var mediaTypeOctedStream = new MediaTypeHeaderValue("application/octed-stream");
            string str = "test";
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            using var stream = new MemoryStream(bytes);

            HttpClient client = _factory.CreateClient("basic");

            _logger.LogDebug("Post(StringContent):");
            client.PostAsync(POST_ONLY_URI, new StringContent(str));

            _logger.LogDebug("Post(StreamContent):");
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = mediaTypeOctedStream;
            client.PostAsync(POST_ONLY_URI, streamContent);

            _logger.LogDebug("Post(ByteArrayContent):");
            var byteArrayContent = new ByteArrayContent(bytes);
            byteArrayContent.Headers.ContentType = mediaTypeOctedStream;
            client.PostAsync(POST_ONLY_URI, byteArrayContent);

            _logger.LogDebug("Post(FormUrlEncodedContent):");
            var dic = new Dictionary<string, string>();
            dic.Add("key1", "value1");
            dic.Add("key2", "value2");
            client.PostAsync(POST_ONLY_URI, new FormUrlEncodedContent(dic));
            
            _logger.LogDebug("Post(MultipartFormDataContent):");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("test1"), "part1");
            multipart.Add(new ByteArrayContent(bytes), "part2");
            client.PostAsync(POST_ONLY_URI, multipart);

            return Ok("done!");
        }

        public async Task<IActionResult> DoGetWithQuery()
        {
            var queries = new Dictionary<string, string>();
            queries.Add("arg1", "abcxyz!#$%&_=-003");
            queries.Add("arg2", "あいうえお");
            string uri = QueryHelpers.AddQueryString("/api/SampleApi", queries);

            HttpClient client = _factory.CreateClient("basic");
            HttpResponseMessage response = await client.GetAsync(uri);
            if( response.IsSuccessStatusCode)
            {
                // ...
            }

            return Ok("done!");
        }

        public async Task<IActionResult> DoGetWithHeader()
        {
            // HTTP要求の作成
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/SampleApi");
            request.Headers.Add("X-ARG1", "abcxyz!#$%&_=-003");

            // HTTP要求の送信
            HttpClient client = _factory.CreateClient("basic");
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // ...
            }

            return Ok("done!");
        }


        public async Task<IActionResult> DoPostAsForm()
        {
            var values = new Dictionary<string, string>();
            values.Add("arg1", "abcxyz!#$%&_=-003");
            values.Add("arg2", "あいうえお");
            var content = new FormUrlEncodedContent(values);

            HttpClient client = _factory.CreateClient("basic");
            HttpResponseMessage response = 
                await client.PostAsync("/api/SampleApi/post-only", content);
            if (response.IsSuccessStatusCode)
            {
                // ...
            }

            return Ok("ok!");
        }

        public async Task<IActionResult> DoPostAsMultipart()
        {
            // サンプルとして添付するストリーム
            var bin = new byte[] {1, 2, 3, 4, 5, 6, 7, 8};
            var stream = new MemoryStream(bin);

            var content = new MultipartFormDataContent();

            // マルチパート: arg1
            content.Add(new StringContent("abcxyz!#$%&_=-003"), "arg1");

            // マルチパート: arg2
            content.Add(new StringContent("あいうえお"), "arg2");

            // マルチパート: arg3
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = 
                new MediaTypeHeaderValue("application/octet-stream");
            streamContent.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileNameStar = "添付ファイル1.dat", // エンコードしたファイル名(RFC5987)
                    FileName = "attached-file1.dat" // ファイル名(FileNameStarが優先)
                };
            content.Add(streamContent, "arg3");

            HttpClient client = _factory.CreateClient("basic");
            HttpResponseMessage response =
                await client.PostAsync("/api/SampleApi/post-only", content);
            if (response.IsSuccessStatusCode)
            {
                // ...
            }

            return Ok("ok!");
        }

        public async Task<IActionResult> DoGetViaProxy()
        {
            HttpClient client = _factory.CreateClient("proxy");
            HttpResponseMessage response = await client.GetAsync("http://www.yahoo.co.jp");
            if (response.IsSuccessStatusCode)
            {
                // ...
            }

            string recv = await response.Content.ReadAsStringAsync();
            return Content(recv);
        }

        public async Task<IActionResult> DoBypassSSLValidation()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get, "https://localserver:44399/api/SampleApi");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(MIME_APP_JSON));

            HttpClient client = _factory.CreateClient("bypass-ssl-validation");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return Json(false);
            }

            string recvJson = await response.Content.ReadAsStringAsync();
            return Content(recvJson, MIME_APP_JSON);
        }

        // ====================================================

        private string CreateSampleJson()
        {
            var dic = new Dictionary<string, string>();
            dic.Add("arg1", "value1");
            dic.Add("arg2", "value2");
            return JsonSerializer.Serialize(dic);
        }

        public IActionResult GetIgnoringSSLError()
        {
            return Json(true);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
