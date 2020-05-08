using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace ExampleWeb.Validators
{

    // 参考
    // https://docs.microsoft.com/ja-jp/archive/blogs/mvpawardprogram/aspnetcore-mvc-error-message

    public class CustomValidationMetadataProvider : IValidationMetadataProvider
    {
        private const string RESOURCE_KEY_PREFIX = "Validator_";

        // 既定でメッセージが設定される属性
        private object[,] ent = new object[,]
        {
            {typeof(CreditCardAttribute), @"The {0} field is not a valid credit card number." },
            {typeof(EmailAddressAttribute), @"The {0} field is not a valid e-mail address." },
            {typeof(PhoneAttribute), @"The {0} field is not a valid phone number." },
            {typeof(UrlAttribute), @"The {0} field is not a valid fully-qualified http, https, or ftp URL." }
        };

        private ResourceManager resourceManager;

        private Type resourceType;

        private Dictionary<Type, string> defaultMessageDic;

        public CustomValidationMetadataProvider()
        {
            // サテライトアセンブリ等で動的に言語を変更する予定はないので
            // 固定で指定する。
            resourceType = typeof(Resource);
            string baseName = resourceType.FullName ?? string.Empty;
            Assembly ass = resourceType.GetTypeInfo().Assembly;
            resourceManager = new ResourceManager(baseName, ass);

            // メッセージ変更の検知用データ
            var dic = new Dictionary<Type, string>();
            dic.Add(typeof(CreditCardAttribute), @"The {0} field is not a valid credit card number.");
            dic.Add(typeof(EmailAddressAttribute), @"The {0} field is not a valid e-mail address.");
            dic.Add(typeof(PhoneAttribute), @"The {0} field is not a valid phone number.");
            dic.Add(typeof(UrlAttribute), @"The {0} field is not a valid fully-qualified http, https, or ftp URL.");
            this.defaultMessageDic = dic;
        }

        public void CreateValidationMetadata(
            ValidationMetadataProviderContext context)
        {
            var metaData = context.ValidationMetadata.ValidatorMetadata;

            // int/Decimal/DateTime等の値型の場合、
            // 暗黙的に必須属性が追加されるので、そのメッセージも置き換える
            if (context.Key.ModelType.GetTypeInfo().IsValueType &&
                metaData.Where(m => m.GetType() == typeof(RequiredAttribute)).Count() == 0)
            {
                metaData.Add(new RequiredAttribute());
            }

            // 含まれる属性全てを処理
            foreach (var obj in metaData)
            {
                if (!(obj is ValidationAttribute attr))
                {
                    continue;
                }

                // メッセージが既定から変更なし＆新メッセージがある場合、
                // 新メッセージで上書きする。
                // (属性のErrorMessageプロパティでメッセージが指定されている場合、
                // そのメッセージを優先する。)
                //

                // リソース指定の場合は既定の動作に移譲
                if (attr.ErrorMessageResourceName != null)
                {
                    continue;
                }

                // メッセージが指定されている場合はそれを優先
                Type type = attr.GetType();
                string message = attr.ErrorMessage;
                string? defaultMessage = this.defaultMessageDic.GetValueOrDefault(type);
                if (!string.Equals(message, defaultMessage))
                {
                    continue;
                }

                // メッセージが既定のままで対応リソースが定義されている場合、
                // そのメッセージで上書き
                string name = RESOURCE_KEY_PREFIX + type.Name;
                string? newMessage = resourceManager.GetString(name);
                if (newMessage != null)
                {
                    attr.ErrorMessageResourceType = resourceType;
                    attr.ErrorMessageResourceName = name;
                    attr.ErrorMessage = null;
                }

                // 【参考】
                // ・CreditCard/EmailAddress/Url属性は
                // 　既定でメッセージが設定されているため、ネットのサンプルのように
                // 　ErrorMessageやErrorMessageResourceNameがnullかを判定する方法では
                // 　日本語メッセージを設定できない。
                // ・ErrorMessageは個別にメッセージを指定する可能性があるので、
                //   この項目がnullかどうかの判定は適切ではない。
                // 　(ex. [Required(ErrorMessage = "...")])
            }
        }

    }
}

