using Persistence.Interfaces;
using System.Data;

namespace Persistence
{
    internal sealed class TransactionManager(IConnectionManager connectionManager) : ITransactionManager
    {
        private ITransaction? _currentTransaction;

        public IDbTransaction? CurrentTransaction => _currentTransaction?.Current;

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct)
        {
            if (_currentTransaction != null)
            {
                if (_currentTransaction.Current == null)
                    throw new InvalidOperationException(nameof(_currentTransaction.Current));

                _currentTransaction = new Transaction(_currentTransaction.Current, _currentTransaction);

                return _currentTransaction;
            }

            var connection = await connectionManager.GetConnectionAsync(ct);
            var transaction = connection.BeginTransaction();

            _currentTransaction = new Transaction(transaction, null);

            return _currentTransaction;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct)
        {
            if (_currentTransaction != null)
            {
                await action();
                return;
            }

            var connection = await connectionManager.GetConnectionAsync(ct);

            using var transaction = connection.BeginTransaction();

            _currentTransaction = new Transaction(transaction, null);

            try
            {
                await action();
                _currentTransaction.Commit();
            }
            catch
            {
                _currentTransaction.Rollback();
                throw;
            }
            finally
            {
                _currentTransaction = null;
            }
        }
    }
}