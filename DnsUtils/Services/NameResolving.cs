using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DnsUtils.Services
{
	public static class NameResolving
	{
        #pragma warning disable CS1998
        public static async Task<IPAddress[]> ResolveAsync(string hostname, int timeout = 2000, params INameResolver[] resolvers)
		{
			Task<IPAddress[]>[] tasks = resolvers.Select(resolver => resolver.ResolveAsync(hostname, timeout)).ToArray();
			int result = Task.WaitAny(tasks, timeout);

			if (result == -1)
			{
				return null;
			}

			return tasks[result].Result;
		}
        #pragma warning restore CS1998
    }
}
