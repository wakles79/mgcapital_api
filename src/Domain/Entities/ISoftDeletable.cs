namespace MGCap.Domain.Entities
{
    /// <summary>
    ///     This is a marker interface to 'archive' entities
    /// </summary>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
