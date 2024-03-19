namespace TransactionManagement.Services.Interfaces
{
    /// <summary>
    /// Defines methods for retrieving IP information.
    /// </summary>
    public interface IIpInfoService
    {
        /// <summary>
        /// Retrieves the current time zone for the specified IP address.
        /// </summary>
        /// <param name="clientIp">The IP address of the client.</param>
        /// <returns>The current time zone.</returns>
        Task<string> GetCurrentTimeZoneAsync(string clientIp);
    }
}
