using System.Net.NetworkInformation;
namespace PhrexNet.Core;
public sealed record AdapterAddress(string Address, int PrefixLength);
public sealed record AdapterSnapshot(string Name, string Kind, string Status, IReadOnlyList<AdapterAddress> Addresses, IReadOnlyList<string> Gateways, IReadOnlyList<string> Dns);
public interface IAdapterSource { IReadOnlyList<AdapterSnapshot> Read(); }
public sealed class LocalAdapterSource : IAdapterSource
{
    public IReadOnlyList<AdapterSnapshot> Read() => NetworkInterface.GetAllNetworkInterfaces().Select(n =>
    {
        var p = n.GetIPProperties();
        return new AdapterSnapshot(n.Name, n.NetworkInterfaceType.ToString(), n.OperationalStatus.ToString(),
            p.UnicastAddresses.Select(a => new AdapterAddress(a.Address.ToString(), a.PrefixLength)).ToArray(),
            p.GatewayAddresses.Select(a => a.Address.ToString()).ToArray(),
            p.DnsAddresses.Select(a => a.ToString()).ToArray());
    }).ToArray();
}
public sealed class AdapterInventory(IAdapterSource source)
{
    public IReadOnlyList<AdapterSnapshot> Read() => source.Read().OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase).ToArray();
    public static string Describe(AdapterSnapshot a) =>
        $"{a.Name} | {a.Kind} | {a.Status}\nAddresses: {Join(a.Addresses.Select(x => $"{x.Address}/{x.PrefixLength}"))}\nGateways: {Join(a.Gateways)}\nDNS: {Join(a.Dns)}";
    private static string Join(IEnumerable<string> values) => string.Join(", ", values.DefaultIfEmpty("Not reported"));
}
