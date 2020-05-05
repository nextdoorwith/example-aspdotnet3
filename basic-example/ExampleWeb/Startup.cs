using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using ExampleWeb;
using ExampleWeb.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;

namespace ExampleWeb
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
            // HTMLのエンコーディングをUTF-8にする(日本語のHTMLエンコード防止)
            services.Configure<WebEncoderOptions>(options => {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            // MVC関連
            IMvcBuilder mvcBuilder = services.AddControllersWithViews(options => {
                // Null許容プロパティに対する暗黙的な必須チェックを無効にする。
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

                // 入力検証のエラーメッセージのカスタマイズ用
                // https://docs.microsoft.com/ja-jp/archive/blogs/mvpawardprogram/aspnetcore-mvc-error-message
                options.ModelMetadataDetailsProviders.Add(
                    new CustomValidationMetadataProvider()
                );
                // 汎用エラー
                options.ModelBindingMessageProvider
                    .SetAttemptedValueIsInvalidAccessor((x, y)=> 
                        string.Format(Resource.AttemptedValueIsInvalidAccessor, x, y));
            });
#if DEBUG
            // ページの変更内容を都度反映
            mvcBuilder.AddRazorRuntimeCompilation();
#endif

            // HttpClient
            // ※サーバ証明書のエラーを無視
            services.AddHttpClient(Options.DefaultName)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = 
                        (httpRequestMessage, cert, cetChain, policyErrors) => true
                });
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
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }

    }
}
