using System.IO;

namespace DnsUtils
{
	public class DnsSrvRecord : DnsResourceRecord
	{
		public ushort Priority { get; set; }
		public ushort Weight { get; set; }
		public ushort Port { get; set; }
		public string SrvHostname { get; set; }

		public override ushort DataLength => (ushort)(6 + SrvHostname.LengthAsDnsHostname());

		public DnsSrvRecord(string hostname, DnsRecordClass @class, uint ttl, ushort priority, ushort weight, ushort port, string srvHostname)
			: base(hostname, DnsRecordType.Srv, @class, ttl)
		{
			Priority = priority;
			Weight = weight;
			Port = port;
			SrvHostname = srvHostname;
		}

		internal DnsSrvRecord()
			: base(null, DnsRecordType.Srv, DnsRecordClass.InterNetwork, 0)
		{

		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);

			writer.Write(NetworkHelper.Nthos(Priority));
			writer.Write(NetworkHelper.Nthos(Weight));
			writer.Write(NetworkHelper.Nthos(Port));
			writer.WriteDnsHostname(SrvHostname);
		}

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			reader.ReadUInt16();

			Priority = NetworkHelper.Nthos(reader.ReadUInt16());
			Weight = NetworkHelper.Nthos(reader.ReadUInt16());
			Port = NetworkHelper.Nthos(reader.ReadUInt16());
			SrvHostname = reader.ReadDnsHostname();
		}
	}
}
