# DnsUtils
A simple DNS library that allows serialization and deserialization of DNS packets, along with advertising and polling using mDNS, LLMNR and DNS-SD.

# Example

```csharp
Mdns mdns = new Mdns();
Llmnr llmnr = new Llmnr();

IPAddress address = NameResolving.ResolveAsync("TheHostname", 2000, CancellationToken.None, mdns, llmnr).Result;

if (address != null)
{
    // Success!
}
```
