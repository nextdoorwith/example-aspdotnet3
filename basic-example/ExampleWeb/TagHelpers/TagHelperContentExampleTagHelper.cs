using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ExampleWeb.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "tag-helper-content-example")]
    public class TagHelperContentExampleTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // 例題としてoutput.Content(TagHelperContent)を編集
            var content = output.Content;

            // コンテンツをdivで上書き
            var topDiv1 = new TagBuilder("div");
            topDiv1.AddCssClass("div-class1");
            topDiv1.Attributes.Add("div-attr1", null);
            topDiv1.Attributes.Add("div-attr2", "");
            topDiv1.Attributes.Add("div-attr3", "val3");
            content.SetHtmlContent(topDiv1);

            // HTMLソースコード上で改行を入れたい場合
            // (HtmlContentBuilderExtensionsのusing宣言が必要)
            content.AppendLine();

            // コンテンツにdivを追加(子としてspan, brを持つ)
            var topDiv2 = new TagBuilder("div");
            topDiv2.AddCssClass("div-class2");
            // 子要素spanを生成
            var childSpan = new TagBuilder("span");
            childSpan.AddCssClass("span-class1");
            topDiv2.InnerHtml.AppendHtml(childSpan);
            // 子要素brを生成
            var childBr = new TagBuilder("br");
            childBr.TagRenderMode = TagRenderMode.SelfClosing; // <br />
            topDiv2.InnerHtml.AppendHtml(childBr);
            // コンテンツにdivを追加
            content.AppendHtml(topDiv2);
        }
    }
}
