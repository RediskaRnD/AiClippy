using File = Models.Entities.File.File;

namespace Persistence.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task<File?> GetFileInfoByIdAsync(long id, CancellationToken ct);
    }
}