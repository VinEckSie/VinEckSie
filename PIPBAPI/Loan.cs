using System;

namespace PocketInvest.LoanModels
{
    public class Loan
    {
        public int LoanId { get; set; }
        public string Country { get; set; }
        public double InterestRate { get; set; }
        public string loanOriginator { get; set; }
        public int originatorId { get; set; }
        public DateOnly issuedDate { get; set; }
        public DateOnly finalPaymentDate { get; set; }
        public string termType { get; set; }
        public string status { get; set; }
        public int remainingTerm { get; set; }
        public int initialTerm { get; set; }
        public double loanAmount { get; set; }

        public double availableToInvest { get; set; }
        public int minimumInvestmentAmount { get; set; }
        public double investedAmount { get; set; }
        public string currency { get; set; }
        public bool buyback { get; set; }
    }
}
