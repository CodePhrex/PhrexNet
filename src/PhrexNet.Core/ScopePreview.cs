using System.Net;
namespace PhrexNet.Core;

public static class ScopePreview
{
    public static IReadOnlyList<IPAddress> Build(string cidr, string exclusions, bool confirmed, CancellationToken cancellationToken = default)
    {
        var parsed = new List<IPAddress>();
        foreach (var token in exclusions.Split(new[] { ',', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!IPAddress.TryParse(token, out var address) || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork || address.ToString() != token)
                throw new ArgumentException("Exclusions must be canonical IPv4 addresses.");
            parsed.Add(address);
        }
        return ScopePolicy.Plan(cidr.Trim(), confirmed, parsed, cancellationToken);
    }
}
