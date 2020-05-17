using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Models
{
    public class ValidateSummaryListModel
    {
        [Required]
        public string child { get; set; }
    }
}
