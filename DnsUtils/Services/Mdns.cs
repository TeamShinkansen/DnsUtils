using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DnsUtils.Services
{
	public class Mdns : INameResolver
	{
		public static readonly IPAddress MulticastAddress = IPAddress.Parse("224.0.0.251");
		public const int Port = 5353;
		public const string HostnameSuffix = ".local";

		public async Task<IPAddress> ResolveAsync(string hostname, int timeout = 2000)
		{
			IPEndPoint mdnsEndPoint = new IPEndPoint(MulticastAddress, Port);

			UdpClient client = new UdpClient();
			DnsPacket questionPacket = new DnsPacket();

			if (!hostname.EndsWith(HostnameSuffix))
			{
				hostname += HostnameSuffix; 
			}

			questionPacket.Questions.Add(new DnsQuestion(hostname, DnsRecordType.A, DnsRecordClass.InterNetwork));

			byte[] data = questionPacket.ToBytes();

			client.JoinMulticastGroup(MulticastAddress);
			client.Send(data, data.Length, mdnsEndPoint);

			client.Client.ReceiveTimeout = timeout;

			DateTime endTime = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout);

			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeout);

			while (DateTime.Now < endTime)
			{
				UdpReceiveResult receiveTask = await Task.Run(client.ReceiveAsync, cancellationTokenSource.Token);

				DnsPacket responsePacket = DnsPacket.FromBytes(receiveTask.Buffer);

				if (responsePacket.ID == questionPacket.ID && responsePacket.AnswerCount > 0)
				{
					foreach (var answer in responsePacket.Answers)
					{
						DnsARecord aRecord = answer as DnsARecord;

						if (aRecord != null)
						{
							return aRecord.IPAddress;
						}
					}
				}
			}

			return null;
		}
	}
}
