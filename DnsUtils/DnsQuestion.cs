namespace DnsUtils
{
	public class DnsQuestion : DnsRecord
	{
		public DnsQuestion(string hostname, DnsRecordType type, DnsRecordClass @class)
			: base(hostname, type, @class)
		{

		}

		internal DnsQuestion()
			: base(null, DnsRecordType.A, DnsRecordClass.InterNetwork)
		{

		}
	}
}
