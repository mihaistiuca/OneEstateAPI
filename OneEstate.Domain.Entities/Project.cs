using OneEstate.Domain.Entities.Base;
using System;
using System.Collections.Generic;

namespace OneEstate.Domain.Entities
{
    public class Project : EntityBase
    {
        public string Name { get; set; }

        // development/rent 
        public string ProjectType { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public List<string> ImageIds { get; set; } = new List<string> { };

        public DateTime InvestmentDeadlineDate { get; set; }

        public DateTime CompletionDate { get; set; }

        public decimal AmountGoal { get; set; }

        public decimal AmountMinGoal { get; set; }

        public decimal AmountInvested { get; set; }

        public decimal NumberOfInvestors { get; set; }

        // the percentage that we estimate the investment will return 
        public decimal EstimatedReturn { get; set; }


        // inactive/investmentOpen/moneyPending/investmentClosed/completed 
        public string Status { get; set; }
    }
    
    public static class ProjectStatus
    {
        public static string Inactive = "inactive";
        public static string InvestmentOpen = "investmentOpen";
        public static string MoneyPending = "moneyPending";
        public static string InvestmentClosed = "investmentClosed";
        public static string Completed = "completed";
    }
}
