using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DnsUtils.Services
{
	public interface INameResolver
	{
		Task<IPAddress[]> ResolveAsync(string hostname, int timeout = 2000);
	}
}
