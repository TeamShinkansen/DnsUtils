using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DnsUtils
{
	public class DnsTxtRecord : DnsResourceRecord
	{
		public List<string> Strings { get; } = new List<string>();

		public override ushort DataLength => (ushort)Strings.Sum(s => Encoding.ASCII.GetByteCount(s) + 1);

		public DnsTxtRecord(string hostname, DnsRecordClass @class, uint ttl, params string[] strings)
			: base(hostname, DnsRecordType.Txt, @class, ttl)
		{
			if (strings != null)
			{
				Strings.AddRange(strings);
			}
		}

		internal DnsTxtRecord()
			: base(null, DnsRecordType.Txt, DnsRecordClass.InterNetwork, 0)
		{

		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);

			foreach (var @string in Strings)
			{
				byte[] data = Encoding.ASCII.GetBytes(@string);
				writer.Write((byte)data.Length);
				writer.Write(data, 0, data.Length);
			}
		}

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			ushort dataLength = reader.ReadUInt16();

			while (DataLength < dataLength)
			{
				byte stringLength = reader.ReadByte();
				byte[] data = reader.ReadBytes(stringLength);
				Strings.Add(Encoding.ASCII.GetString(data));
			}
		}
	}
}
