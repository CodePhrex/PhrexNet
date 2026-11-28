using System.Text.Json;
using PhrexNet.Core;
using Xunit;

public sealed class LocalAuditTests
{
    [Fact]
    public void AppendsStructuredEventsWithoutInputData()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var path = Path.Combine(directory, "audit.jsonl");
        try
        {
            var audit = new LocalAudit(path);
            audit.Record(AuditOperation.ScopePreview, AuditOutcome.Rejected);
            new LocalAudit(path).Record(AuditOperation.AdapterInventory, AuditOutcome.Completed);
            var rows = File.ReadAllLines(path).Select(x => JsonSerializer.Deserialize<AuditEntry>(x)!).ToArray();
            Assert.Equal(2, rows.Length);
            Assert.Equal(AuditOutcome.Rejected, rows[0].Outcome);
            Assert.Equal(AuditOperation.AdapterInventory, rows[1].Operation);
            Assert.All(rows, x => Assert.Equal(TimeSpan.Zero, x.TimestampUtc.Offset));
            using var json = JsonDocument.Parse(File.ReadAllLines(path)[0]);
            Assert.Equal(3, json.RootElement.EnumerateObject().Count());
        }
        finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
    }
    [Fact]
    public void RejectsUndefinedOperations()
    {
        var audit = new LocalAudit("unused.jsonl");
        Assert.Throws<ArgumentOutOfRangeException>(() => audit.Record((AuditOperation)99, AuditOutcome.Completed));
    }
}
