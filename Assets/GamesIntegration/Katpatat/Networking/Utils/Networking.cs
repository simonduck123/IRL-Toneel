using System.Net;
using System.Net.NetworkInformation;

namespace Katpatat.Networking.Utils
{
    public static class NetworkingHelper
    {
        public static IPAddress GetIPAddress(bool onlyWifi=false)
        {
            IPAddress address = null;
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var nI in interfaces)
            {
                if (nI.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                    nI.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;

                if (onlyWifi && nI.NetworkInterfaceType != NetworkInterfaceType.Wireless80211) continue;


                var props = nI.GetIPProperties();

                if (props.GatewayAddresses.Count <= 0) continue;
                
                foreach (var ip in nI.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        address = ip.Address.MapToIPv4();
                    }
                }
                break;
            }

            return address;
        }
    }
}