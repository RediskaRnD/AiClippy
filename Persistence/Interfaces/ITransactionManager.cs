using System.Data;

namespace Persistence.Interfaces
{
    public interface ITransactionManager
    {
        IDbTransaction? CurrentTransaction { get; }

        Task<ITransaction> BeginTransactionAsync(CancellationToken ct);
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct);
    }
}