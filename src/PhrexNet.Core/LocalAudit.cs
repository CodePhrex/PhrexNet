using System.Text.Json;
namespace PhrexNet.Core;

public enum AuditOperation { ScopePreview, AdapterInventory }
public enum AuditOutcome { Completed, Rejected, Failed }
public sealed record AuditEntry(DateTimeOffset TimestampUtc, AuditOperation Operation, AuditOutcome Outcome);

public sealed class LocalAudit(string path, TimeProvider? clock = null)
{
    private static readonly object Gate = new();
    public void Record(AuditOperation operation, AuditOutcome outcome)
    {
        if (!Enum.IsDefined(operation) || !Enum.IsDefined(outcome))
            throw new ArgumentOutOfRangeException();
        var entry = new AuditEntry((clock ?? TimeProvider.System).GetUtcNow(), operation, outcome);
        lock (Gate)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
            File.AppendAllText(path, JsonSerializer.Serialize(entry) + Environment.NewLine);
        }
    }
}
