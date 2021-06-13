using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Models
{
    public class TagHelperExampleModel
    {
        [Required]
        [Display(Name = "テキスト１")]
        public string Text1 { get; set; }

        [Display(Name = "テキスト２")]
        public string Text2 { get; set; }
    }
}
