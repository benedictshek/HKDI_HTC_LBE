using System.Net;
using System.Net.Sockets;

public static class NetworkUtils
{
    public static string GetLocalIPAddress()
    {
        string localIP = "127.0.0.1"; // Fallback
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogWarning($"Failed to get local IP: {e.Message}");
        }

        return localIP;
    }
}
