using System.IO;

namespace DnsUtils
{
	public abstract class DnsResourceRecord : DnsRecord
    {
		public uint TimeToLive { get; set; }
		public abstract ushort DataLength { get; }

		public DnsResourceRecord(string hostname, DnsRecordType type, DnsRecordClass @class, uint ttl)
			: base(hostname, type, @class)
		{
			TimeToLive = ttl;
		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);

			writer.Write(NetworkHelper.Nthos(TimeToLive));
			writer.Write(NetworkHelper.Nthos(DataLength));
		}

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			TimeToLive = NetworkHelper.Nthos(reader.ReadUInt32());
		}

		public static DnsResourceRecord DeserializeResourceRecord(BinaryReader reader)
		{
			string hostname;
			DnsRecordType type;
			DnsRecordClass @class;

			long previousPosition = reader.BaseStream.Position;
			ParseDnsRecordHeader(reader, out hostname, out type, out @class);
			reader.BaseStream.Position = previousPosition;

			DnsResourceRecord result;

			switch (type)
			{
				case DnsRecordType.A:
					result = new DnsARecord();
					break;
				case DnsRecordType.Ptr:
					result = new DnsPtrRecord();
					break;
				case DnsRecordType.Txt:
					result = new DnsTxtRecord();
					break;
				case DnsRecordType.AAAA:
					result = new DnsAAAARecord();
					break;
				case DnsRecordType.Srv:
					result = new DnsSrvRecord();
					break;
				default:
					return null;
			}

			result.Deserialize(reader);

			return result;
		}
	}
}
