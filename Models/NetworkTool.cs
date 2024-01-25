using System.Net.NetworkInformation;

namespace HappyFarm.Models
{
    public static class NetworkTool
    {
        public static async Task<long> GetPingAsync(string host)
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = await ping.SendPingAsync(host);
                    if (reply.Status == IPStatus.Success)
                    {
                        return reply.RoundtripTime;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {reply.Status}");
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    return -1;
                }
            }
        }

        public static async Task<string> GetExternalIpAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                while (true)
                {
                    try
                    {
                        return await client.GetStringAsync("http://api.ipify.org");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
