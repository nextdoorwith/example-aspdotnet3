using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ExampleWeb.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "tag-helper-nested-example")]
    public class TagHelperNestedExampleTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // div開始タグを追加
            var startDiv = new TagBuilder("div");
            startDiv.AddCssClass("start-class");
            startDiv.TagRenderMode = TagRenderMode.StartTag;
            output.PreElement.AppendHtml(startDiv);

            // 子要素divを追加(メイン出力要素の前に追加)
            var startSubDiv = new TagBuilder("div");
            startSubDiv.AddCssClass("start-sub-div");
            output.PreElement.AppendHtml(startSubDiv);

            // 子要素divを追加(メイン出力要素の後に追加)
            var endSubDiv = new TagBuilder("div");
            endSubDiv.AddCssClass("end-sub-div");
            output.PostElement.AppendHtml(endSubDiv);

            // div終了タグを追加
            var endDiv = new TagBuilder("div");
            endDiv.TagRenderMode = TagRenderMode.EndTag;
            output.PostElement.AppendHtml(endDiv);
        }
    }
}
