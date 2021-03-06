using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Movilway.API.Core.IPAddressExtensions
{
    public static class IPAddressExtensions
    {

        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            if (subnetMask != null)
            {
                IPAddress network1 = address.GetNetworkAddress(subnetMask);
                IPAddress network2 = address2.GetNetworkAddress(subnetMask);
                return network1.Equals(network2);
            }
            else
            {
                return address2.Equals(address);
            }


        }

    }
}