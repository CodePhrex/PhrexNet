using System.Net;
using System.Net.Sockets;
namespace PhrexNet.Core;

// Offline validation only. No sockets or network requests.
public static class ScopePolicy
{
    public const int MaxAddresses = 256;
    public static IReadOnlyList<IPAddress> Plan(string cidr, bool confirmed, IEnumerable<IPAddress> exclusions, CancellationToken cancellationToken = default)
    {
        if (!confirmed) throw new InvalidOperationException("Explicit scope confirmation required.");
        cancellationToken.ThrowIfCancellationRequested();
        var parts = cidr.Split('/');
        if (parts.Length != 2 || !IPAddress.TryParse(parts[0], out var ip) ||
            ip.AddressFamily != AddressFamily.InterNetwork || ip.ToString() != parts[0] ||
            !int.TryParse(parts[1], out int prefix) || prefix < 24 || prefix > 32)
            throw new ArgumentException("Use canonical IPv4 CIDR /24 through /32.", nameof(cidr));
        uint value = 0;
        foreach (byte b in ip.GetAddressBytes()) value = (value << 8) | b;
        int count = 1 << (32 - prefix);
        uint start = value & ~((uint)count - 1);
        if (start != value) throw new ArgumentException("Use the subnet network address.", nameof(cidr));
        var excluded = exclusions.ToHashSet();
        var result = new List<IPAddress>();
        for (int i = 0; i < count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (prefix <= 30 && (i == 0 || i == count - 1)) continue;
            uint n = start + (uint)i;
            var address = new IPAddress(new byte[] { (byte)(n >> 24), (byte)(n >> 16), (byte)(n >> 8), (byte)n });
            if (!excluded.Contains(address)) result.Add(address);
        }
        return result.AsReadOnly();
    }
}
