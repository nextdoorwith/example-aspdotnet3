using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.TagHelpers
{
    [HtmlTargetElement("div", Attributes = MyAttributeName)]
    public class CustomValidationSummaryTagHelper: TagHelper
    {
        private const string MyAttributeName = "asp-custom-validation-summary";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeNotBound]
        protected IHtmlGenerator Generator { get; }

        public CustomValidationSummaryTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if( ViewContext == null)
            {
                throw new ArgumentNullException(nameof(ViewContext));
            }

            // エラーメッセージがあればHTMLを生成
            var viewData = ViewContext.ViewData;

            bool isHtmlSummaryModified = false;

            var modelStates = GetAllModelStateList(viewData);

            var htmlSummary = new TagBuilder("ul");
            htmlSummary.AddCssClass(HtmlHelper.ValidationSummaryCssClassName);
            foreach(var modelState in modelStates)
            {
                for (var i = 0; i < modelState.Errors.Count; i++)
                {
                    var modelError = modelState.Errors[i];
                    var htmlMessage = new TagBuilder("li");
                    htmlMessage.InnerHtml.Append(modelError.ErrorMessage);
                    htmlSummary.InnerHtml.AppendLine(htmlMessage);
                    isHtmlSummaryModified = true;
                }
            }

            if( !isHtmlSummaryModified)
            {
                output.SuppressOutput();
                return;
            }

            // 自身の属性名を除外
            output.Attributes.Remove(new TagHelperAttribute(MyAttributeName));

            output.Content.AppendLine(htmlSummary);
        }

        private IList<ModelStateEntry> GetAllModelStateList(ViewDataDictionary viewData)
        {
            var entries = new List<ModelStateEntry>();

            // 項目の順番とエラーメッセージの順番が一致するよう、
            // 複数のネストされたモデルの一番深い所から順番に追加
            if (viewData.ModelState.Count <= 0)
            {
                return entries;
            }
            var metadata = viewData.ModelMetadata;
            var modelStateDictionary = viewData.ModelState;
            Visit(modelStateDictionary.Root, metadata, entries);

            if (entries.Count < modelStateDictionary.Count)
            {
                // ModelMetadataを持たないエントリも追加
                foreach (var entry in modelStateDictionary)
                {
                    if (!entries.Contains(entry.Value))
                    {
                        entries.Add(entry.Value);
                    }
                }
            }
            return entries;
        }

        /// <summary>
        /// ネストされた複数のモデルからエラー情報を収集する。
        /// </summary>
        /// <param name="modelStateEntry"></param>
        /// <param name="metadata"></param>
        /// <param name="entries"></param>
        private static void Visit(
            ModelStateEntry modelStateEntry, 
            ModelMetadata metadata, 
            List<ModelStateEntry> entries)
        {
            if (metadata.ElementMetadata != null && modelStateEntry.Children != null)
            {
                foreach (var indexEntry in modelStateEntry.Children)
                {
                    Visit(indexEntry, metadata.ElementMetadata, entries);
                }
            }

            for (var i = 0; i < metadata.Properties.Count; i++)
            {
                var propertyMetadata = metadata.Properties[i];
                var propertyModelStateEntry = modelStateEntry.GetModelStateForProperty(propertyMetadata.PropertyName);
                if (propertyModelStateEntry != null)
                {
                    Visit(propertyModelStateEntry, propertyMetadata, entries);
                }
            }

            if (!modelStateEntry.IsContainerNode)
            {
                entries.Add(modelStateEntry);
            }
        }
    }
}
