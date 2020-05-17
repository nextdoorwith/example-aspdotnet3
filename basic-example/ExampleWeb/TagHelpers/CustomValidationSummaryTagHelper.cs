using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.TagHelpers
{
    [HtmlTargetElement(MyElement, Attributes = MyAttributeName)]
    public class CustomValidationSummaryTagHelper: TagHelper
    {
        private const string MyElement = "div";
        private const string MyAttributeName = "asp-custom-validation-summary";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeNotBound]
        protected IHtmlGenerator Generator { get; }

        private static ILogger<CustomValidationSummaryTagHelper> _logger;

        public CustomValidationSummaryTagHelper(
            IHtmlGenerator generator, 
            ILogger<CustomValidationSummaryTagHelper> logger)
        {
            Generator = generator;
            _logger = logger;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // 前提知識と処理概要：
            // ・各モデル・プロパティの検証結果はModelStateEntryに格納される。
            // ・これらは、ViewContext.ViewData.ModelState(Dictionary型)に、
            // 　プロパティ名・ModelStateEntryの組で保存されている。
            // ・エラーメッセージを作成するためにModelStateから
            // 　ModelStateEntryを取得すると画面表示順と合わなくなる。
            // ・画面表示順と合わせるために、独自にModelStateEntryのリストを生成する。

            var viewData = ViewContext.ViewData;

            // プロパティ毎のモデル状態エントリのリストと
            // モデル状態エントリからプロパティ名を取得するためのディクショナリ作成
            var modelStates = GetModelStateList(viewData, true);
            var propDic = viewData.ModelState
                .ToDictionary(x => x.Value, y => y.Key); // swap key and value

            // 各モデルに含まれるエラー群を抽出してメッセージ要素を生成
            bool isHtmlSummaryModified = false;
            var htmlSummary = new TagBuilder("ul");
            htmlSummary.InnerHtml.AppendLine();
            htmlSummary.AddCssClass("error-link"); // 独自cssクラス(リンクを赤にする)
            foreach (var entry in modelStates)
            {
                // 対象のプロパティ名
                // ※input要素が出力される際、IDの"[]_"は"_"にエスケープされているので
                // それに合わせて変換する。
                string name = propDic.GetValueOrDefault(entry) ?? string.Empty;
                name = name.Replace('.', '_').Replace('[', '_').Replace(']', '_');

                // 対象プロパティに紐づくエラーメッセージ処理
                foreach (var err in entry.Errors)
                {
                    var htmlMessage = new TagBuilder("li");
                    var msg = err.ErrorMessage;

                    // 例1: テキスト出力する場合 ※自動的にHTMLエンコードされる
                    // ex. <li>[someprop]: メッセージ</li>
                    //htmlMessage.InnerHtml.Append($"{name}: {msg}");

                    // 例2: HTMLを出力する場合 ※HTMLエンコードされないので注意！
                    // ex. <li><b>[someprop]: メッセージ</b></li>
                    //htmlMessage.InnerHtml.AppendHtmlLine($"<b>{name}</b>: {msg}");

                    // 例3: 要素を独自に構築
                    // ex. <li>[someprop]: <a href="..." onclick="...">メッセージ</a></li>
                    htmlMessage.InnerHtml.Append($"{name}: ");
                    if( !string.IsNullOrEmpty(name))
                    {
                        var htmlLink = new TagBuilder("a");
                        htmlLink.InnerHtml.Append(msg);
                        htmlLink.Attributes.Add("href", $"javascript: void(0);");
                        htmlLink.Attributes.Add("onclick", $"jump('{name}');");
                        htmlMessage.InnerHtml.AppendHtml(htmlLink);
                    }
                    else
                    {
                        htmlMessage.InnerHtml.Append(msg);
                    }

                    // サマリに追加
                    htmlSummary.InnerHtml.AppendLine(htmlMessage);
                    isHtmlSummaryModified = true;
                }
            }

            // 変更がない場合は何も出力しない
            if( !isHtmlSummaryModified)
            {
                output.SuppressOutput();
                return;
            }

            // トップレベル要素を仮作成して既存要素(output)にマージ
            var topTag = new TagBuilder(MyElement);
            topTag.AddCssClass(HtmlHelper.ValidationSummaryCssClassName);
            topTag.InnerHtml.AppendLine();
            topTag.InnerHtml.AppendHtml(htmlSummary);
            output.MergeAttributes(topTag);
            output.Content.AppendLine(topTag.InnerHtml);
            output.Attributes.Remove(new TagHelperAttribute(MyAttributeName));
        }

        /// <summary>
        /// エラー情報を含むモデル状態エントリリストを取得します。
        /// </summary>
        /// <param name="viewData"></param>
        /// <returns></returns>
        private List<ModelStateEntry> GetModelStateList(
            ViewDataDictionary viewData, bool withProperty = false)
        {
            var entries = new List<ModelStateEntry>();
            var metadata = viewData.ModelMetadata;
            var modelState = viewData.ModelState;

            if (modelState.Count <= 0)
            {
                return entries;
            }

            // モデルエラーのみの場合は
            // プロパティがHtmlFieldPrefix(空値)になっているもののみを返却
            if (!withProperty)
            {
                if( modelState.TryGetValue(
                    viewData.TemplateInfo.HtmlFieldPrefix, out var entry))
                {
                    entries.Add(entry);
                }
                return entries;
            }

            // モデルがネストされる場合があるため、
            // 再帰的に全てのモデルを探索してエラー情報を収集
            Visit(modelState.Root, metadata, entries, metadata.Name);

            // 項目名が紐づかいないエラーがある場合は漏らさず追加
            // (参考)
            // ・上記の再帰処理はモデル・プロパティに基づいてエラーを取得している。
            // 　モデルに実在しないプロパティに対するエラーメッセージを拾えないため、
            // 　ここで漏れないようにしている。（このようなケースはバグと思われる。）
            // 　例) ModelState.AddModelError("noexists", "エラーメッセージ")
            // ・次のように項目を指定しないエラーはトップレベルのモデルに紐づく
            // 　エラーとして上記の再帰処理で取得可能である。
            // 　例) ModelState.AddModelError("", "モデルエラーメッセージ")
            if (entries.Count < modelState.Count)
            {
                foreach (var pair in modelState)
                {
                    if (!entries.Contains(pair.Value))
                    {
                        _logger.LogTrace($"AddMissingEntry[{pair.Key}]: Errors={pair.Value.Errors.Count}");
                        entries.Add(pair.Value);
                    }
                }
            }
            return entries;
        }

        /// <summary>
        /// ネストされた複数のモデルからエラー情報を収集する。
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="metadata"></param>
        /// <param name="entries"></param>
        /// <param name="namePrefix">デバッグ用なので不要</param>
        private void Visit(
            ModelStateEntry entry, 
            ModelMetadata metadata, 
            IList<ModelStateEntry> entries, string name)
        {
            // 配列やリスト型のプロパティの場合、含まれる全てを再帰処理
            if (metadata.ElementMetadata != null && entry.Children != null)
            {
                for(int i = 0; i<entry.Children.Count; i++)
                {
                    Visit(entry.Children[i], metadata.ElementMetadata, 
                        entries, name + metadata.ElementMetadata.Name + $"[{i}]");
                }
            }

            // プロパティを含む場合はそれぞれに対して再帰処理
            for (var i = 0; i < metadata.Properties.Count; i++)
            {
                var propMetadata = metadata.Properties[i];
                var propModelStateEntry = entry.GetModelStateForProperty(propMetadata.PropertyName);
                if (propModelStateEntry != null)
                {
                    string newName = 
                        (!string.IsNullOrEmpty(name) ? name + "." : "") + propMetadata.Name;
                    Visit(propModelStateEntry, propMetadata, entries, newName);
                }
            }

            // 対象がプロパティの場合はエントリ追加
            if (!entry.IsContainerNode)
            {
                _logger.LogTrace($"AddEntry[{name}]: Errors={entry.Errors.Count}");
                entries.Add(entry);
            }
        }

    }
}
