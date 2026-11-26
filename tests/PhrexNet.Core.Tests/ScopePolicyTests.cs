using System.Net;
using PhrexNet.Core;
using Xunit;
public sealed class ScopePolicyTests
{
    [Fact] public void RequiresConfirmation() => Assert.Throws<InvalidOperationException>(() => ScopePolicy.Plan("192.0.2.0/24", false, []));
    [Theory]
    [InlineData("192.0.2.0/23")]
    [InlineData("192.0.2.1/24")]
    [InlineData("2001:db8::/64")]
    [InlineData("invalid")]
    public void RejectsInvalidScope(string value) => Assert.Throws<ArgumentException>(() => ScopePolicy.Plan(value, true, []));
    [Fact] public void ExcludesReservedEndpointsAndExplicitExclusions()
    {
        var rows = ScopePolicy.Plan("192.0.2.0/29", true, [IPAddress.Parse("192.0.2.2")]);
        Assert.Equal(new[] {"192.0.2.1","192.0.2.3","192.0.2.4","192.0.2.5","192.0.2.6"}, rows.Select(x => x.ToString()));
    }
    [Fact] public void HandlesSingleHostAndPointToPoint()
    {
        Assert.Single(ScopePolicy.Plan("192.0.2.1/32", true, []));
        Assert.Equal(2, ScopePolicy.Plan("192.0.2.0/31", true, []).Count);
    }
    [Fact] public void BoundsAndCancels()
    {
        Assert.Equal(254, ScopePolicy.Plan("192.0.2.0/24", true, []).Count);
        Assert.Throws<OperationCanceledException>(() => ScopePolicy.Plan("192.0.2.0/24", true, [], new CancellationToken(true)));
    }
}
