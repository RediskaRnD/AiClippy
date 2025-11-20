namespace Models.Entities.File
{
    public sealed record File(
        long Id,
        string Name,
        // byte[] Data,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}