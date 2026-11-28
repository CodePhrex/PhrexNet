using PhrexNet.Core;
using Xunit;
public sealed class ScopePreviewTests
{
    [Fact] public void ParsesExclusions() => Assert.Equal(new[] { "192.0.2.1", "192.0.2.4", "192.0.2.5", "192.0.2.6" }, ScopePreview.Build("192.0.2.0/29", "192.0.2.2,\n192.0.2.3", true).Select(x => x.ToString()));
    [Theory]
    [InlineData("invalid")]
    [InlineData("::1")]
    [InlineData("127.1")]
    public void RejectsInvalidExclusions(string value) => Assert.Throws<ArgumentException>(() => ScopePreview.Build("192.0.2.0/29", value, true));
    [Fact] public void RequiresConfirmation() => Assert.Throws<InvalidOperationException>(() => ScopePreview.Build("192.0.2.0/29", "", false));
    [Fact] public void Cancels() => Assert.Throws<OperationCanceledException>(() => ScopePreview.Build("192.0.2.0/29", "", true, new CancellationToken(true)));
}
