using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DnsUtils
{
	internal static class DnsExtensions
    {
		//TODO: Add label pointers for smaller packets
		public static void WriteDnsHostname(this BinaryWriter writer, string name)
		{
			string[] labels = name.Split('.');

			foreach (var label in labels)
			{
				writer.Write((byte)label.Length);
				byte[] data = Encoding.ASCII.GetBytes(label);
				writer.Write(data, 0, data.Length);
			}

			writer.Write((byte)0);
		}

		//TODO: Test following pointers in packet data
		public static string ReadDnsHostname(this BinaryReader reader)
		{
			List<string> labels = new List<string>();
			byte length;

			while ((length = reader.ReadByte()) > 0)
			{
				if (length >= 192)
				{
					long nextPosition = reader.BaseStream.Position + 1;
					ushort offset = (ushort)((length & 0x3F) << 8 | reader.ReadByte());

					reader.BaseStream.Seek(offset, SeekOrigin.Begin);
					labels.Add(reader.ReadDnsHostname());
					reader.BaseStream.Seek(nextPosition, SeekOrigin.Begin);
					break;
				}
				else
				{
					labels.Add(Encoding.ASCII.GetString(reader.ReadBytes(length)));
				}
			}

			return string.Join(".", labels);
		}

		//INFO: Length could only be calculated for non-compressed hostnames
		public static int LengthAsDnsHostname(this string hostname)
		{
			return Encoding.ASCII.GetByteCount(hostname) + hostname.Count(c => c == '.') + 1;
		}
	}
}
