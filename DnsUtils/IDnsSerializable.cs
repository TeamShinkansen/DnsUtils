using System.IO;

namespace DnsUtils
{
	public interface IDnsSerializable
    {
		void Serialize(BinaryWriter writer);
		void Deserialize(BinaryReader reader);
    }
}
