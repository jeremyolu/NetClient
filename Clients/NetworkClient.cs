using System.Net.Sockets;
using System.Net;
using NetClient.Interfaces;

namespace NetClient.Clients
{
    public class NetworkClient : INetworkClient
    {
        /// <summary>
        /// Gets all IPv4 addresses for the specified remote host name.
        /// </summary>
        /// <returns>A list of IPv4 addresses as strings.</returns>
        public List<string> GetRemoteIPv4Addresses(string hostNameOrAddress)
        {
            if (string.IsNullOrWhiteSpace(hostNameOrAddress))
                throw new ArgumentNullException(nameof(hostNameOrAddress), "Host name or address cannot be null or empty.");

            var ipAddresses = new List<string>();

            var hostEntry = Dns.GetHostEntry(hostNameOrAddress);

            var addressList = hostEntry.AddressList.ToList();

            addressList.ForEach(ip =>
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddresses.Add(ip.ToString());
                }
            });

            return ipAddresses;
        }
    }
}
