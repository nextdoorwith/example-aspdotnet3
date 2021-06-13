using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace ExampleWeb.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "tag-helper-output-example")]
    public class TagHelperOutputExampleTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // 出力するタグ名をdivからspanに変更
            output.TagName = "span";

            // class属性の編集
            output.AddClass("add-class", HtmlEncoder.Default);
            output.RemoveClass("delete-class", HtmlEncoder.Default);

            // 属性全般の編集
            var attrs = output.Attributes;

            // 属性の削除
            if (attrs.ContainsName("delete-attr"))
                attrs.Remove(attrs["delete-attr"]);

            // 属性の設定(上書き)
            attrs.SetAttribute("set-attr1", null);      // 結果: 空値("")
            attrs.SetAttribute("set-attr2", "");        // 結果: 空値("")
            attrs.SetAttribute("set-attr3", "val3");
            attrs.SetAttribute("set-attr4", "newval4"); // 結果: 既存属性の上書き
            attrs.SetAttribute(
                new TagHelperAttribute("set-attr5"));   // 属性名のみ出力

            // 属性の追加
            attrs.Add("add-attr1", null);             // 結果: 空値("")
            attrs.Add("add-attr2", "");               // 結果: 空値("")
            attrs.Add("add-attr3", "val3");
            attrs.Add("add-attr4", "newval4");        // 結果: 同名で追加→無視
            attrs.Add(
                new TagHelperAttribute("add-attr5")); // 属性名のみ出力

            // 既存のタグから属性のコピー
            var copy = new TagBuilder("div");
            copy.AddCssClass("copy-class");
            copy.Attributes.Add("copy-attr1", null);      // 結果: 空値("")
            copy.Attributes.Add("copy-attr2", "");        // 結果: 空値("")
            copy.Attributes.Add("copy-attr3", "val3");
            copy.Attributes.Add("copy-attr4", "newval4"); // 結果: 無視(反映されない)
            output.MergeAttributes(copy); // TagHelperOutputExtensionsのusing宣言が必要
        }
    }
}
