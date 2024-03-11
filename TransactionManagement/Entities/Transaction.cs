namespace TransactionManagement.Entities
{
    /// <summary>
    /// Represents a transaction entity.
    /// </summary>
    public class Transaction
    {
        public required string TransactionId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required decimal Amount { get; set; }
        public required DateTime TransactionDate { get; set; }
        public required string Timezone { get; set; }
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
    }
}
