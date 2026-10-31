using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pol.Data
{
    public class DatabaseContext : IDisposable
    {
        private readonly string _dbPath;
        private SqliteConnection _connection;

        public DatabaseContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        public async Task InitializeAsync()
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _connection = new SqliteConnection($"Data Source={_dbPath}");
            await _connection.OpenAsync();

            await CreateSchemaAsync();
        }

        private async Task CreateSchemaAsync()
        {
            // Create Messages table
            var createTableCmd = @"
                CREATE TABLE IF NOT EXISTS Messages (
                    Id TEXT PRIMARY KEY,
                    SenderId TEXT NOT NULL,
                    ReceiverId TEXT NOT NULL,
                    Topic TEXT,
                    TimestampUnix INTEGER NOT NULL,
                    Ciphertext BLOB NOT NULL,
                    Nonce BLOB NOT NULL,
                    Signature BLOB NOT NULL
                );";

            // Create Indexes for efficient sync and reading
            var createIndexCmd = @"
                CREATE INDEX IF NOT EXISTS idx_messages_timestamp ON Messages(TimestampUnix);
                CREATE INDEX IF NOT EXISTS idx_messages_receiver ON Messages(ReceiverId);
            ";

            using (var transaction = _connection.BeginTransaction())
            {
                await ExecuteCommandAsync(createTableCmd);
                await ExecuteCommandAsync(createIndexCmd);
                await transaction.CommitAsync();
            }
        }

        public SqliteConnection GetConnection()
        {
            if (_connection == null)
                throw new InvalidOperationException("Database not initialized.");
            return _connection;
        }

        private async Task ExecuteCommandAsync(string commandText)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = commandText;
            await cmd.ExecuteNonQueryAsync();
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}