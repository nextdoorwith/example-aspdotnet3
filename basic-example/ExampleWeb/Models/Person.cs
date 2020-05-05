using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Models
{
    public class Person
    {
        [Required]
        public string Name { get; set; }

        [Required]
        //[ZenKana]
        public string NameKana { get; set; }

        public bool? Gender { get; set; }

        public DateTime? birthDate { get; set; }

        [Range(0, 1_000_000_000)]
        public int? AnnualIncome { get; set; }

        [Required]
        [Remote(action: "VerifyEmail", controller: "Person")]
        public string EMail { get; set; }

        public int job { get; set; }
        
        //[RequiredIf(property: "job", value; "1", label: "学生")]
        public string FinalEducation { get; set; }

        //[RequiredIf(property: "job", value; "2", lable: "社会人")]
        public string FinalCompany { get; set; }
    }
}
