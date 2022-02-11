using System;
using System.Collections.Generic;
using System.Text;

namespace OneEstate.Application.Dtos
{
    public class CreateProjectInput
    {
        public string Name { get; set; }

        public string ProjectType { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public List<string> ImageIds { get; set; } = new List<string> { };

        public DateTime InvestmentDeadlineDate { get; set; }

        public DateTime CompletionDate { get; set; }

        public decimal AmountGoal { get; set; }

        public decimal AmountMinGoal { get; set; }

        public decimal EstimatedReturn { get; set; }
    }
}
