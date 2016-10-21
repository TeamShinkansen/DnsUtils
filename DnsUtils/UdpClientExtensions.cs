using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DnsUtils
{
	internal static class UdpClientExtensions
	{
		public static Task<UdpReceiveResult> ReceiveAsync(this UdpClient client)
		{
			return Task<UdpReceiveResult>.Factory.FromAsync((callback, state) => client.BeginReceive(callback, state), (ar) =>
				{
					IPEndPoint remoteEP = null;
					Byte[] buffer = client.EndReceive(ar, ref remoteEP);
					return new UdpReceiveResult(buffer, remoteEP);

				}, null);
		}
	}
}
