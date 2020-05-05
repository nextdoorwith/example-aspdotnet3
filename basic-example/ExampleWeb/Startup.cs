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
            // HTML�̃G���R�[�f�B���O��UTF-8�ɂ���(���{���HTML�G���R�[�h�h�~)
            services.Configure<WebEncoderOptions>(options => {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            // MVC�֘A
            IMvcBuilder mvcBuilder = services.AddControllersWithViews(options => {
                // Null���e�v���p�e�B�ɑ΂���ÖٓI�ȕK�{�`�F�b�N�𖳌��ɂ���B
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

                // ���͌��؂̃G���[���b�Z�[�W�̃J�X�^�}�C�Y�p
                // https://docs.microsoft.com/ja-jp/archive/blogs/mvpawardprogram/aspnetcore-mvc-error-message
                options.ModelMetadataDetailsProviders.Add(
                    new CustomValidationMetadataProvider()
                );
                // �ėp�G���[
                options.ModelBindingMessageProvider
                    .SetAttemptedValueIsInvalidAccessor((x, y)=> 
                        string.Format(Resource.AttemptedValueIsInvalidAccessor, x, y));
            });
#if DEBUG
            // �y�[�W�̕ύX���e��s�x���f
            mvcBuilder.AddRazorRuntimeCompilation();
#endif

            // HttpClient
            // ���T�[�o�ؖ����̃G���[�𖳎�
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
