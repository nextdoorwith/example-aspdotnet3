using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ExampleWeb.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "basic")]
    public class BasicTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.PreElement.SetHtmlContent("<div class=\"pre-element\"></div>");

            output.PreContent.SetHtmlContent("<div class=\"pre-content\"></div>");
            output.PostContent.SetHtmlContent("<div class=\"post-content\"></div>");

            output.PostElement.SetHtmlContent("<div class=\"post-element\"></div>");
        }
    }
}
