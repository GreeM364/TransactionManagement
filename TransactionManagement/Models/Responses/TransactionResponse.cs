namespace TransactionManagement.Models.Responses
{
    /// <summary>
    /// Response model for transaction data.
    /// </summary>
    public class TransactionResponse
    {
        public string TransactionId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Timezone { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
