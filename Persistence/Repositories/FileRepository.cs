using Dapper;

using Models.Entities.File;
using Persistence.Interfaces;
using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories
{
    internal sealed class FileRepository(IConnectionManager connectionManager) : IFileRepository
    {
	    public async Task<Models.Entities.File.File?> GetFileInfoByIdAsync(long id, CancellationToken ct)
        {
            var sql = @$"
                select
	                f.id {nameof(Models.Entities.File.File.Id)},
	                f.name {nameof(Models.Entities.File.File.Name)},
                    f.created_at {nameof(Models.Entities.File.File.CreatedAt)},
                    f.edited_at {nameof(Models.Entities.File.File.UpdatedAt)}
                from
	                file f
                where
	                f.id = @{nameof(id)}
            ";

            var connection = await connectionManager.GetConnectionAsync(ct);
            var record = await connection.QuerySingleOrDefaultAsync<Models.Entities.File.File>(sql, new { id });

            return record;
        }
    }
}