using SQLite;
using Gestion_RDV.Models;

namespace Gestion_RDV.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "gestion_rdv.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<RendezVous>().Wait();
        }

        // Récupérer tous les rendez-vous
        public Task<List<RendezVous>> GetRendezVousAsync()
        {
            return _database.Table<RendezVous>().ToListAsync();
        }

        // Ajouter un rendez-vous
        public Task<int> SaveRendezVousAsync(RendezVous rdv)
        {
            if (rdv.Id != 0)
                return _database.UpdateAsync(rdv);
            else
                return _database.InsertAsync(rdv);
        }

        // Supprimer un rendez-vous
        public Task<int> DeleteRendezVousAsync(RendezVous rdv)
        {
            return _database.DeleteAsync(rdv);
        }
    }
}