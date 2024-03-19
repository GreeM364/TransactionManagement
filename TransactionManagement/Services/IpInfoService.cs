using IPinfo.Models;
using IPinfo;
using TransactionManagement.Services.Interfaces;
using Microsoft.Extensions.Options;
using TransactionManagement.Persistence;

namespace TransactionManagement.Services
{
    /// <summary>
    /// Service for retrieving IP information.
    /// </summary>
    public class IpInfoService : IIpInfoService
    {
        private readonly string _token;

        public IpInfoService(IOptions<IpInfoOptions> options)
        {
            _token = options.Value.Token;
        }

        /// <summary>
        /// Retrieves the current time zone for the specified IP address.
        /// </summary>
        /// <param name="clientIp">The IP address of the client.</param>
        /// <returns>The current time zone.</returns>
        public async Task<string> GetCurrentTimeZoneAsync(string clientIp)
        {
            IPResponse ipResponse = await GetIpDetailsAsync(clientIp);

            return ipResponse.Timezone;
        }

        /// <summary>
        /// Retrieves IP details asynchronously.
        /// </summary>
        /// <param name="clientIp">The IP address of the client.</param>
        /// <returns>The IP details response.</returns>
        private async Task<IPResponse> GetIpDetailsAsync(string clientIp)
        {
            IPinfoClient client = new IPinfoClient.Builder()
                .AccessToken(_token)
                .Build();

            IPResponse ipResponse;

            if (clientIp == "::1")
                ipResponse = await client.IPApi.GetDetailsAsync();
            else
                ipResponse = await client.IPApi.GetDetailsAsync(clientIp);

            return ipResponse;
        }
    }
}
