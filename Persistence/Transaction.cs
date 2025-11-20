using Persistence.Interfaces;
using System.Data;

namespace Persistence
{
    internal sealed class Transaction(IDbTransaction transaction, ITransaction? outerTransaction) : ITransaction
    {
        private IDbTransaction? _transaction = transaction;
        private ITransaction? _outerTransaction = outerTransaction;
        
        public IDbTransaction? Current => _transaction;

        public void Commit()
        {
            if (_outerTransaction != null)
                return;

            if (_transaction == null)
                throw new InvalidOperationException(nameof(_transaction));

            _transaction.Commit();
        }

        public void Rollback()
        {
            if (_transaction == null)
                throw new InvalidOperationException(nameof(_transaction));

            _transaction.Rollback();
            _outerTransaction = null;
        }

        public void Dispose()
        {
            if (_outerTransaction != null)
                return;

            _transaction?.Dispose();
            _transaction = null;
        }
    }
}