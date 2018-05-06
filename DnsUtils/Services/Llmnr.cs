using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DnsUtils.Services
{
	public class Llmnr : INameResolver
	{
		public static readonly IPAddress MulticastAddress = IPAddress.Parse("224.0.0.252");
		public const int Port = 5355;

		public async Task<IPAddress[]> ResolveAsync(string hostname, int timeout = 2000)
		{
			IPEndPoint llmnrEndPoint = new IPEndPoint(MulticastAddress, Port);

			UdpClient client = new UdpClient();
			DnsPacket questionPacket = new DnsPacket();

			questionPacket.Questions.Add(new DnsQuestion(hostname, DnsRecordType.A, DnsRecordClass.InterNetwork));

			byte[] data = questionPacket.ToBytes();

			client.JoinMulticastGroup(MulticastAddress);
			client.Send(data, data.Length, llmnrEndPoint);

			client.Client.ReceiveTimeout = timeout;

			DateTime endTime = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout);

			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeout);

			while (DateTime.Now < endTime)
			{
				UdpReceiveResult receiveTask = await Task.Run(client.ReceiveAsync, cancellationTokenSource.Token);

				DnsPacket responsePacket = DnsPacket.FromBytes(receiveTask.Buffer);

				if (responsePacket.ID == questionPacket.ID && responsePacket.AnswerCount > 0)
				{
                    List<IPAddress> results = new List<IPAddress>();
					foreach (var answer in responsePacket.Answers)
					{
						DnsARecord aRecord = answer as DnsARecord;

						if (aRecord != null)
						{
							results.Add(aRecord.IPAddress);
						}
					}
                    if (results.Count > 0)
                    {
                        return results.ToArray();
                    }
				}
			}

			return null;
		}
	}
}
