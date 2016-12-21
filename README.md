# DnsUtils
A simple DNS library that allows serialization and deserialization of DNS packets, along with advertising and polling using mDNS, LLMNR and DNS-SD.

# Example

## Synchronous

```csharp
Mdns mdns = new Mdns();
Llmnr llmnr = new Llmnr();

IPAddress address = NameResolving.ResolveAsync("vdownsrv-sql", 2000, mdns, llmnr).Result;

if (address != null)
{
    // Success!
}
```

## Asynchronous

```csharp
Mdns mdns = new Mdns();
Llmnr llmnr = new Llmnr();

IPAddress address = await NameResolving.ResolveAsync("vdownsrv-sql", 2000, mdns, llmnr);

if (address != null)
{
    // Success!
}
```
