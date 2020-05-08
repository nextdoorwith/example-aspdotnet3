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
using Microsoft.Extensions.Localization;
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
                // (int?���̌^�ɑ΂��関���͂����e����)
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

                // ���f���o�C���f�B���O���s���̃G���[���b�Z�[�W���J�X�^�}�C�Y
                // (�T�[�o���Ń��f���ɒl���i�[����ۂɔ�������\��������B)
                Func<string, string, string> f1 = (f, a1) => string.Format(f, a1);
                Func<string, string, string, string> f2 = (f, a1, a2) => string.Format(f, a1, a2);
                var mp = options.ModelBindingMessageProvider;
                mp.SetAttemptedValueIsInvalidAccessor((x, y) =>
                    f2(Resource.ModelBinding_AttemptedValueIsInvalid, x, y));
                mp.SetMissingBindRequiredValueAccessor((x) =>
                    f1(Resource.ModelBinding_MissingBindRequiredValue, x));
                mp.SetMissingKeyOrValueAccessor(() => 
                    Resource.ModelBinding_MissingKeyOrValue);
                mp.SetMissingRequestBodyRequiredValueAccessor(() =>
                    Resource.ModelBinding_MissingRequestBodyRequiredValue);
                mp.SetNonPropertyAttemptedValueIsInvalidAccessor((x) =>
                    f1(Resource.ModelBinding_NonPropertyAttemptedValueIsInvalid, x));
                mp.SetNonPropertyUnknownValueIsInvalidAccessor(() =>
                    Resource.ModelBinding_NonPropertyUnknownValueIsInvalid);
                mp.SetNonPropertyValueMustBeANumberAccessor(() =>
                    Resource.ModelBinding_NonPropertyValueMustBeANumber);
                mp.SetUnknownValueIsInvalidAccessor((x) =>
                    f1(Resource.ModelBinding_UnknownValueIsInvalid, x));
                mp.SetValueIsInvalidAccessor((x) =>
                    f1(Resource.ModelBinding_ValueIsInvalid, x));
                mp.SetValueMustBeANumberAccessor((x) =>
                    f1(Resource.ModelBinding_ValueMustBeANumber, x));
                mp.SetValueMustNotBeNullAccessor((x) =>
                    f1(Resource.ModelBinding_ValueMustNotBeNull, x));

                // ���͌��؎��s���̃G���[���b�Z�[�W�̃J�X�^�}�C�Y
                // https://docs.microsoft.com/ja-jp/archive/blogs/mvpawardprogram/aspnetcore-mvc-error-message
                options.ModelMetadataDetailsProviders.Add(
                    new CustomValidationMetadataProvider()
                );

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
