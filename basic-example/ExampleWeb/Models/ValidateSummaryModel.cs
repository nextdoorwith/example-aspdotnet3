using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWeb.Models
{
    public class ValidateSummaryModel
    {
        public ValidateSummaryModel()
        {
            prop4 = new ValidateSummaryListModel[] { new ValidateSummaryListModel(), new ValidateSummaryListModel() };
        }

        public string prop1 { get; set; }

        public int prop2 { get; set; }

        public DateTime prop3 {get; set; }

        public ValidateSummaryListModel[] prop4 { get; set; }

        public ValidateSummaryNestedModel prop5 { get; set; }

        public List<string> prop6 { get; set; }
    }
}
