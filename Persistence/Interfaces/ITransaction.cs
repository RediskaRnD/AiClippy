using System.Data;

namespace Persistence.Interfaces
{
    public interface ITransaction : IDisposable
    {
        IDbTransaction? Current { get; }

        void Commit();
        void Rollback();
    }
}