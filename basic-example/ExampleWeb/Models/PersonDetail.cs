using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Models
{
    public enum Gender{
        Male, Female
    }

    public class PersonDetail
    {
        [Required]
        [Display(Name ="姓")]
        public string Sei { get; set; }

        [Required]
        [Display(Name = "名")]
        public string Mei { get; set; }

        [Display(Name = "セイ")]
        public string SeiKana { get; set; }

        [Display(Name = "メイ")]
        public string MeiKana { get; set; }

        [Display(Name = "性別")]
        public Gender Gender;

        [Required]
        [Display(Name = "生年月日")]
        public DateTime BirthDay { get; set; }

        [Display(Name = "電話番号")]
        public string Tel { get; set; }

        [Required]
        [Display(Name = "電話番号（携帯）")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "郵便番号")]
        public string PostalCode { get; set; }

        [Display(Name = "住所（都道府県）")]
        public string Prefectures { get; set; }

        [Display(Name = "住所（市区町村）")]
        public string Address1 { get; set; }

        [Display(Name = "住所（市区町村）")]
        public string Address2 { get; set; }

        [Required]
        [Range(0, 9999)]
        [Display(Name = "年収")]
        public int AnnualIncome { get; set; }

        [Display(Name = "メモ")]
        public string Note { get; set; }
    }
}
