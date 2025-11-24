using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using Gestion_RDV.Models;

namespace Gestion_RDV.Services
{
    public class SqliteDatabaseService : IDatabaseService
    {
        private readonly string _dbPath;
        private SQLiteAsyncConnection? _connection;

        public SqliteDatabaseService()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dbPath = Path.Combine(folder, "appdb.db3");
        }

        public async Task InitializeAsync()
        {
            _connection = new SQLiteAsyncConnection(_dbPath);
            await _connection.CreateTableAsync<Patient>();
            await _connection.CreateTableAsync<RendezVous>();
        }

        public SQLiteAsyncConnection GetConnection()
        {
            if (_connection == null)
                throw new InvalidOperationException("Database not initialized. Call InitializeAsync first.");
            return _connection;
        }
    }
}