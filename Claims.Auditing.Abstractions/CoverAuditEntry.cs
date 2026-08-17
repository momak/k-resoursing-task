namespace Claims.Auditing.Abstractions
{
    public sealed record CoverAuditEntry(string EntityId, string Action, DateTime Timestamp)
        : AuditEntry(EntityId, Action, Timestamp);
}
