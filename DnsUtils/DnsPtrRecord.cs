using System.IO;

namespace DnsUtils
{
	public class DnsPtrRecord : DnsResourceRecord
	{
		public string PtrHostname { get; set; }

		public override ushort DataLength => (ushort)PtrHostname.LengthAsDnsHostname();

		public DnsPtrRecord(string hostname, DnsRecordClass @class, uint ttl, string ptrHostname)
			: base(hostname, DnsRecordType.Ptr, @class, ttl)
		{
			PtrHostname = ptrHostname;
		}

		internal DnsPtrRecord()
			: base(null, DnsRecordType.Ptr, DnsRecordClass.InterNetwork, 0)
		{

		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);

			writer.WriteDnsHostname(PtrHostname);
		}

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			reader.ReadUInt16();

			PtrHostname = reader.ReadDnsHostname();
		}
	}
}
 