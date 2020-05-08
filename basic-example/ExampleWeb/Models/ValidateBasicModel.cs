using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Models
{
    public class ValidateBasicModel
    {
        [Display(Name = "必須")]
        [Required]
        public string RequiredItem { get; set; }

        [Display(Name = "必須(int)")]
        [Required]
        public int RequiredIntItem { get; set; }

        [Display(Name = "必須(Decimal)")]
        [Required]
        public Decimal RequiredDecimalItem { get; set; }

        [Display(Name = "必須(DateTime)")]
        [Required]
        public DateTime RequiredDateTimeItem { get; set; }

        [Display(Name = "クレジットカード")]
        [CreditCard]
        public string CreditCardItem { get; set; }

        [Display(Name = "比較")]
        [Compare("CompareItem2")]
        public string CompareItem { get; set; }

        [Display(Name = "比較(2)")]
        public string CompareItem2 { get; set; }

        [Display(Name = "メールアドレス")]
        [EmailAddress]
        public string EmailAddressItem { get; set; }

        [Display(Name = "電話番号")]
        [Phone]
        public string PhoneItem { get; set; }

        [Display(Name = "範囲")]
        [Range(1, 100)]
        public int? RangeItem { get; set; }

        [Display(Name = "範囲(日付)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1/1/1966", "1/1/2020")]
        public DateTime? RangeDateItem { get; set; }

        [Display(Name = "正規表現")]
        [RegularExpression("A-Z")]
        public string RegularExpressionItem { get; set; }

        [Display(Name = "正規表現2")]
        [RegularExpression("A-Z", ErrorMessage = "{0}は大文字アルファベットを指定してください。")]
        public string RegularExpressionItem2 { get; set; }

        [Display(Name = "文字長")]
        [StringLength(10)]
        public string StringLengthItem { get; set; }

        [Display(Name = "文字長(最小あり)")]
        [StringLength(10, MinimumLength = 5)]
        public string StringLengthMinItem { get; set; }

        [Display(Name = "URL")]
        [Url]
        public string UrlItem { get; set; }

        // 注意点
        // ・クライアント側検証が有効でないと実行されない
        // ・サブミット後、キー押下単位で実行される
        // ・サブミット時に検証されないので独自に検証が必要
        [Display(Name = "リモート")]
        [Remote(action: "Verify", controller: "ValidateBasic")]
        public string RemoteItem { get; set; }

    }
}
