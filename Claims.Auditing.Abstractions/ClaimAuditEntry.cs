namespace Claims.Auditing.Abstractions
{
    public sealed record ClaimAuditEntry(string EntityId, string Action, DateTime Timestamp)
        : AuditEntry(EntityId, Action, Timestamp);
}
