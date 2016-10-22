using System;
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

		public async Task<IPAddress> ResolveAsync(string hostname, int timeout = 2000, CancellationToken cancellationToken = default(CancellationToken))
		{
			IPEndPoint llmnrEndPoint = new IPEndPoint(MulticastAddress, Port);

			UdpClient client = new UdpClient();
			DnsPacket questionPacket = new DnsPacket();

			questionPacket.Questions.Add(new DnsQuestion(hostname, DnsRecordType.A, DnsRecordClass.InterNetwork));

			byte[] data = questionPacket.ToBytes();

			client.JoinMulticastGroup(MulticastAddress);
			client.Send(data, data.Length, llmnrEndPoint);

			client.Client.ReceiveTimeout = timeout;

			return await Task.Run(delegate
			{
				DateTime endTime = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout);

				while (DateTime.Now < endTime)
				{
					TimeSpan remainingTime = endTime - DateTime.Now;

					Task delayTask = Task.Delay((int)remainingTime.TotalMilliseconds);
					Task<UdpReceiveResult> receiveTask = Task.Run(() => client.ReceiveAsync(), cancellationToken);

					Task<Task> finishedTask = Task.WhenAny(delayTask, receiveTask);

					if (finishedTask == delayTask)
					{
						return null;
					}

					if (cancellationToken.IsCancellationRequested)
					{
						return null;
					}

					DnsPacket responsePacket = DnsPacket.FromBytes(receiveTask.Result.Buffer);

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
			}, cancellationToken);
		}
	}
}
