using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class BaseRequest<T>
{
    public List<(string, string)> Headers { get; set; }
    public T Body { get; set; }
    public HttpStatusCode ExpectedResponseStatusCode { get; set; }

    public void AddHeaders(List<(string, string)> additionalHeaders)
    {
        foreach (var header in additionalHeaders)
        {
            Headers.Add(header);
        }
    }

    public void RemoveHeaders(List<(string, string)> unwantedHeaders)
    {
        foreach (var header in unwantedHeaders)
        {
            Headers.Remove(header);
        }
    }

    public static string GetLocalHostIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
