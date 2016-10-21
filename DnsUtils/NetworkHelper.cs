using System.Net;

namespace DnsUtils
{
	internal static class NetworkHelper
	{
		public static ushort Nthos(ushort value)
		{
			return (ushort)IPAddress.NetworkToHostOrder((short)value);
		}

		public static short Nthos(short value)
		{
			return IPAddress.NetworkToHostOrder(value);
		}
		public static uint Nthos(uint value)
		{
			return (uint)IPAddress.NetworkToHostOrder((int)value);
		}

		public static int Nthos(int value)
		{
			return IPAddress.NetworkToHostOrder(value);
		}

		public static ulong Nthos(ulong value)
		{
			return (ulong)IPAddress.NetworkToHostOrder((long)value);
		}

		public static long Nthos(long value)
		{
			return IPAddress.NetworkToHostOrder(value);
		}
	}
}
