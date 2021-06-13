using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ExampleWeb.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "tag-builder-example")]
    public class TagBuilderExampleTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var preElementDiv = new TagBuilder("div");
            preElementDiv.AddCssClass("pre-element");
            output.PreElement.SetHtmlContent(preElementDiv);

            var preContentDiv = new TagBuilder("div");
            preContentDiv.AddCssClass("pre-content");
            output.PreContent.SetHtmlContent(preContentDiv);

            var postContentDiv = new TagBuilder("div");
            postContentDiv.AddCssClass("post-content");
            output.PostContent.SetHtmlContent(postContentDiv);

            var postElementDiv = new TagBuilder("div");
            postElementDiv.AddCssClass("post-element");
            output.PostElement.SetHtmlContent(postElementDiv);
        }
    }
}
