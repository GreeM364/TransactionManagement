﻿namespace TransactionManagement.Models.Requests
{
    /// <summary>
    /// A model that represents columns in a CSV file.
    /// </summary>
    public class TransactionRequestCsv
    {
        public string transaction_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string amount { get; set; }
        public string transaction_date { get; set; }
        public string client_location { get; set; }
    }
}
