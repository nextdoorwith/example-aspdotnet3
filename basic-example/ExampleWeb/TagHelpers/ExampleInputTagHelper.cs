using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "example")]
    public class ExampleInputTagHelper : InputTagHelper
    {
        // 属性名に対応するプロパティに属性値が自動的に設定される。
        // - 既定では"mail-to"属性はMailToプロパティにマップされる。
        // - プロパティに対応する属性を指定する場合は[HtmlAttributeName("属性名")]を使用する。
        // - マッピングを抑制する場合は[HtmlAttributeNotBound]を使用する。
        public string Example { get; set; }

        // モデルの値(InputTagHelperではForプロパティが用意されている。)
        //[HtmlAttributeName("asp-for")]
        //public ModelExpression For { get; set; }

        // 実行時のコンテキスト情報
        //[ViewContext]
        //[HtmlAttributeNotBound]
        //public ViewContext ViewContext { get; set; }

        public ExampleInputTagHelper(IHtmlGenerator generator) : base(generator) { }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var preElementTag = new TagBuilder("div");
            preElementTag.AddCssClass("pre-element");
            output.PreElement.SetHtmlContent(preElementTag);

            // input要素は要素値を持たないので(output.TagMode = TagMode.StartTagOnly)
            // 次のコンテンツに対する操作は全て無視される。

            var preContentTag = new TagBuilder("div");
            preContentTag.AddCssClass("pre-content");
            output.PreContent.SetHtmlContent(preContentTag); // 無視される

            var contentTag = new TagBuilder("div");
            contentTag.AddCssClass("content");
            output.Content.SetHtmlContent(contentTag); // 無視される

            var postContentTag = new TagBuilder("div");
            postContentTag.AddCssClass("post-content");
            output.PostContent.SetHtmlContent(postContentTag); // 無視される

            var postElementTag = new TagBuilder("div");
            postElementTag.AddCssClass("post-element");
            output.PostElement.SetHtmlContent(postElementTag);

            base.Process(context, output);
        }
    }
}
