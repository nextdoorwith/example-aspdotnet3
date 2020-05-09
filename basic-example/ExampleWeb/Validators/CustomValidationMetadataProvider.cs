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

        private ResourceManager resourceManager;

        private Type resourceType;

        // 複数スレッドによる読み取りであればスレッドセーフ
        // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?redirectedfrom=MSDN&view=netcore-3.1
        private Dictionary<Type, string> defaultMessageDic;

        public CustomValidationMetadataProvider()
        {
            // サテライトアセンブリ等で動的に言語を変更する予定はないので固定で指定
            resourceType = typeof(Resource);
            string baseName = resourceType.FullName ?? string.Empty;
            Assembly ass = resourceType.GetTypeInfo().Assembly;
            resourceManager = new ResourceManager(baseName, ass);

            // 既定でメッセージが設定されている属性と既定メッセージ
            // (後の「既存メッセージからの変更有無」を判定するために使用)
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
            // 暗黙的に必須属性が追加されるので、そのメッセージも置き換え
            if (context.Key.ModelType.GetTypeInfo().IsValueType &&
                metaData.Where(m => m.GetType() == typeof(RequiredAttribute)).Count() == 0)
            {
                metaData.Add(new RequiredAttribute());
            }

            // 対象プロパティに紐づく全ての属性に対して処理
            foreach (var obj in metaData)
            {
                if (!(obj is ValidationAttribute attr))
                {
                    continue;
                }

                // リソースやメッセージが変更されている場合はそれを優先
                // (新旧メッセージが共にnullも「変更なし」とみなす)
                if (attr.ErrorMessageResourceName != null)
                {
                    continue;
                }
                Type type = attr.GetType();
                string message = attr.ErrorMessage;
                string? defaultMessage = this.defaultMessageDic.GetValueOrDefault(type);
                if (!string.Equals(message, defaultMessage))
                {
                    continue;
                }

                // メッセージが既定から変更されておらず、
                // 対応するメッセージが未定義の場合は既定の動作に任せる
                string name = RESOURCE_KEY_PREFIX + type.Name;
                string? newMessage = resourceManager.GetString(name);
                if (string.IsNullOrEmpty(newMessage) )
                {
                    continue;
                }

                // メッセージが既定から変更されておらず、
                // 対応するメッセージが定義されている場合、それで上書き
                attr.ErrorMessageResourceType = resourceType;
                attr.ErrorMessageResourceName = name;
                attr.ErrorMessage = null;
            }
        }

    }
}

