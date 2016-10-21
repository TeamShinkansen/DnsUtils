using System;
using System.Collections.Generic;
using System.IO;

namespace DnsUtils
{
	public class DnsPacket
	{
		private readonly static Random random = new Random();
		
		public ushort ID { get; set; }
		public DnsPacketFlags Flags { get; set; }
		public ushort QuestionCount => (ushort)Questions.Count;
		public ushort AnswerCount => (ushort)Answers.Count;
		public ushort AuthoritativeCount => (ushort)Authoritatives.Count;
		public ushort AdditionalCount => (ushort)Additionals.Count;

		public List<DnsQuestion> Questions { get; } = new List<DnsQuestion>();
		public List<DnsResourceRecord> Answers { get; } = new List<DnsResourceRecord>();
		public List<DnsResourceRecord> Authoritatives { get; } = new List<DnsResourceRecord>();
		public List<DnsResourceRecord> Additionals { get; } = new List<DnsResourceRecord>();

		public DnsPacket(DnsPacketFlags flags = DnsPacketFlags.None)
		{
			ID = (ushort)random.Next();
			Flags = flags;
		}

		public DnsPacket(ushort id)
			: this(DnsPacketFlags.None)
		{
			ID = id;
		}

		public bool IsAnswer
		{
			get { return Flags.HasFlag(DnsPacketFlags.Answer); }
			set { Flags &= value ? DnsPacketFlags.Answer : ~DnsPacketFlags.Answer; }
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(NetworkHelper.Nthos(ID));
			writer.Write(NetworkHelper.Nthos((ushort)Flags));
			writer.Write(NetworkHelper.Nthos(QuestionCount));
			writer.Write(NetworkHelper.Nthos(AnswerCount));
			writer.Write(NetworkHelper.Nthos(AuthoritativeCount));
			writer.Write(NetworkHelper.Nthos(AdditionalCount));

			foreach (var question in Questions)
			{
				question.Serialize(writer);
			}

			foreach (var answer in Answers)
			{
				answer.Serialize(writer);
			}

			foreach (var authoritative in Authoritatives)
			{
				authoritative.Serialize(writer);
			}

			foreach (var additional in Additionals)
			{
				additional.Serialize(writer);
			}
		}

		public void Deserialize(BinaryReader reader)
		{
			ID = NetworkHelper.Nthos(reader.ReadUInt16());
			Flags = (DnsPacketFlags)NetworkHelper.Nthos(reader.ReadUInt16());
			ushort questionCount = NetworkHelper.Nthos(reader.ReadUInt16());
			ushort answerCount = NetworkHelper.Nthos(reader.ReadUInt16());
			ushort authoritativeCount = NetworkHelper.Nthos(reader.ReadUInt16());
			ushort additionalCount = NetworkHelper.Nthos(reader.ReadUInt16());

			for (int i = 0; i < questionCount; i++)
			{
				DnsQuestion question = new DnsQuestion();
				question.Deserialize(reader);
				Questions.Add(question);
			}

			for (int i = 0; i < answerCount; i++)
			{
				DnsResourceRecord record = DnsResourceRecord.DeserializeResourceRecord(reader);
				Answers.Add(record);
			}

			for (int i = 0; i < authoritativeCount; i++)
			{
				DnsResourceRecord record = DnsResourceRecord.DeserializeResourceRecord(reader);
				Authoritatives.Add(record);
			}

			for (int i = 0; i < additionalCount; i++)
			{
				DnsResourceRecord record = DnsResourceRecord.DeserializeResourceRecord(reader);
				Additionals.Add(record);
			}
		}

		public byte[] ToBytes()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryWriter writer = new BinaryWriter(stream);
				Serialize(writer);
				return stream.ToArray();
			}
		}

		public static DnsPacket FromBytes(byte[] data)
		{
			using (MemoryStream stream = new MemoryStream(data))
			{
				BinaryReader reader = new BinaryReader(stream);
				DnsPacket result = new DnsPacket();
				result.Deserialize(reader);
				return result;
			}
		}
	}
}
