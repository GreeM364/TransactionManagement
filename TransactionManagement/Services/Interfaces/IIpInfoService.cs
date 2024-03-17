namespace TransactionManagement.Services.Interfaces
{
    public interface IIpInfoService
    {
        Task<string> GetCurrentTimeZoneAsync(string clientIp);
    }
}
