using System.IO;
using System.Net;
using System.Net.Sockets;

namespace DnsUtils
{
	public class DnsARecord : DnsResourceRecord
	{
		public IPAddress IPAddress { get; set; }

		public override ushort DataLength => 4;

		public DnsARecord(string hostname, DnsRecordClass @class, uint ttl, IPAddress ipAddress)
			: base(hostname, DnsRecordType.A, @class, ttl)
		{
			if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
			{
				throw new InvalidDataException("IPAddress should be of type AddressFamily InterNetwork");
			}

			IPAddress = ipAddress;
		}

		internal DnsARecord()
			: base(null, DnsRecordType.A, DnsRecordClass.InterNetwork, 0)
		{

		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);

			writer.Write(IPAddress.GetAddressBytes());
		}

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			ushort dataLength = reader.ReadUInt16();

			IPAddress = new IPAddress(reader.ReadBytes(4));
		}
	}
}
