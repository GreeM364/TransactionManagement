namespace TransactionManagement.Models.Requests
{
    public class TransactionRequestScv
    {
        public string transaction_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string amount { get; set; }
        public string transaction_date { get; set; }
        public string client_location { get; set; }
    }
}
