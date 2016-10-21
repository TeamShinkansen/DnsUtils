using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DnsUtils
{
	public interface INameResolver
	{
		Task<IPAddress> ResolveAsync(string hostname, int timeout = 2000, CancellationToken cancellationToken = default(CancellationToken));
	}
}
