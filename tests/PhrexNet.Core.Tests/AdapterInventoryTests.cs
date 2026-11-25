using PhrexNet.Core;
using Xunit;
public sealed class AdapterInventoryTests
{
    private sealed class Fixture(params AdapterSnapshot[] rows) : IAdapterSource { public IReadOnlyList<AdapterSnapshot> Read() => rows; }
    [Fact] public void OrdersAdaptersAndPreservesAddresses()
    {
        var a = new AdapterSnapshot("Ethernet", "Ethernet", "Up", [new("192.0.2.5",24),new("2001:db8::5",64)], ["192.0.2.1"], ["192.0.2.53"]);
        var b = new AdapterSnapshot("WiFi", "Wireless80211", "Down", [], [], []);
        var rows = new AdapterInventory(new Fixture(b,a)).Read();
        Assert.Equal(a, rows[0]);
        Assert.Contains("2001:db8::5/64", AdapterInventory.Describe(rows[0]));
        Assert.Contains("192.0.2.1", AdapterInventory.Describe(rows[0]));
        Assert.Contains("192.0.2.53", AdapterInventory.Describe(rows[0]));
        Assert.Contains("Not reported", AdapterInventory.Describe(rows[1]));
    }
    [Fact] public void EmptyInventoryIsValid() => Assert.Empty(new AdapterInventory(new Fixture()).Read());
}
