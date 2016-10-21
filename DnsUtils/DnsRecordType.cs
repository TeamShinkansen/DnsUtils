namespace DnsUtils
{
	public enum DnsRecordType : ushort
    {
		A = 1,
		Ptr = 12,
		Txt = 16,
		AAAA = 28,
		Srv = 33
    }
}
