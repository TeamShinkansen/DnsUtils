using System.IO;
using System.Net;
using System.Net.Sockets;

namespace DnsUtils
{
	public class DnsAAAARecord : DnsResourceRecord
	{
		public IPAddress IPAddress { get; set; }

		public override ushort DataLength => 16;

		public DnsAAAARecord(string hostname, DnsRecordClass @class, uint ttl, IPAddress ipAddress)
			: base(hostname, DnsRecordType.AAAA, @class, ttl)
		{
			if (ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new InvalidDataException("IPAddress should be of type AddressFamily InterNetwork");
			}

			IPAddress = ipAddress;
		}

		internal DnsAAAARecord()
			: base(null, DnsRecordType.AAAA, DnsRecordClass.InterNetwork, 0)
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
