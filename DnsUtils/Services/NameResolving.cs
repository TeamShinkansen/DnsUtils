using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DnsUtils.Services
{
	public static class NameResolving
	{
		public static async Task<IPAddress> ResolveAsync(string hostname, int timeout = 2000, CancellationToken cancellationToken = default(CancellationToken), params INameResolver[] resolvers)
		{
			return await await Task.WhenAny(resolvers.Select(resolver => resolver.ResolveAsync(hostname, timeout, cancellationToken)));
		}
	}
}
