namespace Claims.Auditing.Abstractions
{
    /// <summary>
    /// A pending audit record waiting to be persisted.
    /// </summary>
    public abstract record AuditEntry(string EntityId, string Action, DateTime Timestamp);
}
