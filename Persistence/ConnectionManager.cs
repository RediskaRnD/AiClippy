using Persistence.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;

namespace Persistence
{
    internal sealed class ConnectionManager : IConnectionManager, IAsyncDisposable
    {
        private readonly string _connectionString;

        private DbConnection? _dbConnection;

        private static readonly SemaphoreSlim Semaphore = new(100);

        public ConnectionManager(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _connectionString = configuration.GetConnectionString("DB") 
                                ?? throw new ArgumentException("Connection string with name \"DB\" not found");
        }

        public async Task<IDbConnection> GetConnectionAsync(CancellationToken ct)
        {
            if (_dbConnection != null)
            {
                if (_dbConnection.State == ConnectionState.Open)
                    return _dbConnection;

                _dbConnection.Dispose();
            }

            await Semaphore.WaitAsync();

            _dbConnection = new SqliteConnection(_connectionString);

            await _dbConnection.OpenAsync(ct);

            return _dbConnection;
        }

        public async ValueTask DisposeAsync()
        {
            if (_dbConnection != null)
            {
                await _dbConnection.CloseAsync();
                await _dbConnection.DisposeAsync();
            }

            Semaphore.Release();

            GC.SuppressFinalize(this);
        }
    }
}