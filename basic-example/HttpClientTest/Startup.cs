using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HttpClientTest.Handler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HttpClientTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // ���ؗp�̊����HttpClient
            services.AddTransient<HttpTraceHandler>();

            // �����HttpClient���\��
            services.AddHttpClient(Options.DefaultName, options =>
            {
                options.BaseAddress = new Uri("https://localhost:44399");
                options.DefaultRequestHeaders.Add("X-ACCESS-KEY", "secret");
                options.DefaultRequestHeaders.Accept.Clear();
                options.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });

            // ��{
            services.AddHttpClient("basic", options =>
            {
                options.BaseAddress = new Uri("https://localhost:44399");
                //options.BaseAddress = new Uri("http://localhost:8080");
                options.DefaultRequestHeaders.Add("X-ACCESS-KEY", "secret");
                options.DefaultRequestHeaders.Accept.Clear();
                options.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });
            //.AddHttpMessageHandler<HttpTraceHandler>();

            // �v���L�V
            // .NET Core 2.1 �ȍ~�̊���̃��b�Z�[�W�n���h���ł���SocketsHttpHandler��
            // �v���L�V�̐ݒ���s���B
            // (����ȑO��HttpClientHandler��荂���\�Ńv���b�g�t�H�[����ˑ�)
            services.AddHttpClient("proxy")
                .ConfigurePrimaryHttpMessageHandler(() =>
                new SocketsHttpHandler
                {
                    UseProxy = true,
                    Proxy = new WebProxy("localhost", 8888)
                }
            );

            // SSL�G���[�����T���v��
#if false
            services.AddHttpClient("bypass-ssl-validation");
#else
            services.AddHttpClient("bypass-ssl-validation")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler{ 
                    ServerCertificateCustomValidationCallback = (
                        httpRequestMessage, cert, certChain, policyErrors) => {
                       return true;
                    }
                });
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
