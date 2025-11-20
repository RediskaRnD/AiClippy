using System.Data;

namespace Persistence.Interfaces
{
    public interface IConnectionManager
    {
        Task<IDbConnection> GetConnectionAsync(CancellationToken ct);
    }
}