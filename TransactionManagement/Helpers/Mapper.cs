using GeoTimeZone;
using System.Globalization;
using TransactionManagement.Entities;
using TransactionManagement.Models.Requests;
using TransactionManagement.Models.Responses;

namespace TransactionManagement.Helpers
{
    /// <summary>
    /// Provides methods for mapping between different transaction-related models.
    /// </summary>
    public static class Mapper
    {
        /// <summary>
        /// Converts a list of TransactionRequestCsv objects to a list of Transaction objects.
        /// </summary>
        /// <param name="transactionsRequestScv">The list of TransactionRequestCsv objects to convert.</param>
        /// <returns>A list of Transaction objects.</returns>
        public static List<Transaction> TransactionRequestCsvToTransaction(List<TransactionRequestCsv> transactionsRequestScv)
        {
            return transactionsRequestScv.Select(TransactionRequestCsvToTransaction).ToList();
        }

        /// <summary>
        /// Converts a list of Transaction objects to a list of TransactionResponse objects.
        /// </summary>
        /// <param name="transactions">The list of Transaction objects to convert.</param>
        /// <returns>A list of TransactionResponse objects.</returns>
        public static List<TransactionResponse> TransactionToTransactionResponse(List<Transaction> transactions)
        {
            return transactions.Select(TransactionToTransactionResponse).ToList();
        }

        /// <summary>
        /// Converts a TransactionRequestCsv object to a Transaction object.
        /// </summary>
        /// <param name="transactionRequestScv">The TransactionRequestCsv object to convert.</param>
        /// <returns>A Transaction object.</returns>
        public static Transaction TransactionRequestCsvToTransaction(TransactionRequestCsv transactionRequestScv)
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

        /// <summary>
        /// Converts a Transaction object to a TransactionResponse object.
        /// </summary>
        /// <param name="transaction">The Transaction object to convert.</param>
        /// <returns>A TransactionResponse object.</returns>
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
            var timeZoneId = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;

            return TimeZoneHelper.ConvertIanaToWindows(timeZoneId);
        }

        private static DateTime ConvertToUtc(string transactionDate, string timeZone)
        {
            var localDateTime = DateTime.Parse(transactionDate);

            return DateTimeZoneConverter.ConvertToUtc(localDateTime, timeZone);
        }
    }
}
