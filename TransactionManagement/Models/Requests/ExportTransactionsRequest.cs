namespace TransactionManagement.Models.Requests
{
    public class ExportTransactionsRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeTransactionId { get; set; }
        public bool IncludeName { get; set; }
        public bool IncludeEmail { get; set; }
        public bool IncludeAmount { get; set; }
        public bool IncludeTransactionDate { get; set; }
        public bool IncludeTimezone { get; set; }
        public bool IncludeLocation { get; set; }


        public bool IsAtLeastOneFieldIncluded()
        {
            return IncludeTransactionId ||
                   IncludeName ||
                   IncludeEmail ||
                   IncludeAmount ||
                   IncludeTransactionDate ||
                   IncludeTimezone ||
                   IncludeLocation;
        }
    }
}
