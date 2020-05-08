using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Models
{
    public class UploadModel
    {

        [Required]
        [Display(Name = "名前")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "アップロードファイル")]
        public List<IFormFile> FileList { get; set; }

    }
}
