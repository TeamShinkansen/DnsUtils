using System.IO;

namespace DnsUtils
{
	public abstract class DnsRecord : IDnsSerializable
	{
		public string Hostname { get; set; }
		public DnsRecordType Type { get; set; }
		public DnsRecordClass Class { get; set; }

		public DnsRecord(string hostname, DnsRecordType type, DnsRecordClass @class)
		{
			Hostname = hostname;
			Type = type;
			Class = @class;
		}
		
		public virtual void Serialize(BinaryWriter writer)
		{
			writer.WriteDnsHostname(Hostname);
			writer.Write(NetworkHelper.Nthos((ushort)Type));
			writer.Write(NetworkHelper.Nthos((ushort)Class));
		}

		public virtual void Deserialize(BinaryReader reader)
		{
			Hostname = reader.ReadDnsHostname();
			Type = (DnsRecordType)NetworkHelper.Nthos(reader.ReadUInt16());
			Class = (DnsRecordClass)NetworkHelper.Nthos(reader.ReadUInt16());
		}

		public static void ParseDnsRecordHeader(BinaryReader reader, out string hostname, out DnsRecordType type, out DnsRecordClass @class)
		{
			hostname = reader.ReadDnsHostname();
			type = (DnsRecordType)NetworkHelper.Nthos(reader.ReadUInt16());
			@class = (DnsRecordClass)NetworkHelper.Nthos(reader.ReadUInt16());
		}
	}
}
