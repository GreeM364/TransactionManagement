using IPinfo.Models;
using IPinfo;
using TransactionManagement.Services.Interfaces;
using Microsoft.Extensions.Options;
using TransactionManagement.Persistence;

namespace TransactionManagement.Services
{
    public class IpInfoService : IIpInfoService
    {
        private readonly string _token;

        public IpInfoService(IOptions<IpInfoOptions> options)
        {
            _token = options.Value.Token;
        }

        public async Task<string> GetCurrentTimeZoneAsync(string clientIp)
        {
            IPResponse ipResponse = await GetIpDetailsAsync(clientIp);

            return ipResponse.Timezone;
        }

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
