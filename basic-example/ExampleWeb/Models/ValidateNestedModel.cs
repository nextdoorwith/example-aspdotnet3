using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Models
{
    public class ValidateNestedModel
    {
        public string prop1 { get; set; }

        public int prop2 { get; set; }

        public DateTime prop3 {get; set; }

        public IEnumerable<string> prop4 { get; set; }

        public ValidateNestedModel2 prop5 { get; set; }
    }
}
