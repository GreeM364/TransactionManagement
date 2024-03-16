using GeoTimeZone;
using System.Globalization;
using TransactionManagement.Entities;
using TransactionManagement.Models.Requests;
using TransactionManagement.Models.Responses;

namespace TransactionManagement.Helpers
{
    public static class Mapper
    {
        public static List<Transaction> TransactionRequestScvToTransaction(List<TransactionRequestScv> transactionsRequestScv)
        {
            return transactionsRequestScv.Select(TransactionRequestScvToTransaction).ToList();
        }

        public static List<TransactionResponse> TransactionToTransactionResponse(List<Transaction> transactions)
        {
            return transactions.Select(TransactionToTransactionResponse).ToList();
        }

        public static Transaction TransactionRequestScvToTransaction(TransactionRequestScv transactionRequestScv)
        {
            var (latitude, longitude) = GetLocationCoordinates(transactionRequestScv.client_location);
            var amount = ParseAmount(transactionRequestScv.amount);
            var timeZone = GetClientTimeZone(latitude, longitude);
            var transactionDateUtc = ConvertToUtc(transactionRequestScv.transaction_date, timeZone);

            return new Transaction
            {
                TransactionId = transactionRequestScv.transaction_id,
                Name = transactionRequestScv.name,
                Email = transactionRequestScv.email,
                Amount = amount,
                TransactionDate = transactionDateUtc,
                Timezone = timeZone,
                Latitude = latitude,
                Longitude = longitude
            };
        }

        public static TransactionResponse TransactionToTransactionResponse(Transaction transaction)
        {
            return new TransactionResponse
            {
                TransactionId = transaction.TransactionId,
                Name = transaction.Name,
                Email = transaction.Email,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Timezone = transaction.Timezone,
                Latitude = transaction.Latitude,
                Longitude = transaction.Longitude
            };
        }

        private static (double latitude, double longitude) GetLocationCoordinates(string clientLocation)
        {
            clientLocation = clientLocation.Trim('"');

            var locationParts = clientLocation.Split(',');

            var latitude = double.Parse(locationParts[0].Trim(), CultureInfo.InvariantCulture);
            var longitude = double.Parse(locationParts[1].Trim(), CultureInfo.InvariantCulture);

            return (latitude, longitude);
        }

        private static decimal ParseAmount(string amount)
        {
            return decimal.Parse(amount.Replace("$", ""), CultureInfo.InvariantCulture);
        }

        private static string GetClientTimeZone(double latitude, double longitude)
        {
            return TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        }

        private static DateTime ConvertToUtc(string transactionDate, string timeZone)
        {
            var localDateTime = DateTime.Parse(transactionDate);

            return DateTimeZoneConverter.ConvertToUtc(localDateTime, timeZone);
        }
    }
}
