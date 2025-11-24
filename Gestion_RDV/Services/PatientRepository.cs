using System.Collections.Generic;
using System.Threading.Tasks;
using Gestion_RDV.Models;
using SQLite;

namespace Gestion_RDV.Services
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IDatabaseService _databaseService;

        public PatientRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        private SQLiteAsyncConnection Conn => _databaseService.GetConnection();

        public Task<int> AddAsync(Patient patient) => Conn.InsertAsync(patient);
        public Task<int> DeleteAsync(int id) => Conn.DeleteAsync<Patient>(id);
        public Task<List<Patient>> GetAllAsync() => Conn.Table<Patient>().ToListAsync();
        public Task<Patient?> GetByIdAsync(int id) => Conn.FindAsync<Patient>(id).AsTask();
        public Task<int> UpdateAsync(Patient patient) => Conn.UpdateAsync(patient);
        public Task<List<Patient>> SearchAsync(string query)
        {
            query = $"%{query}%";
            return Conn.QueryAsync<Patient>("SELECT * FROM Patient WHERE LastName LIKE ? OR FirstName LIKE ?", query, query);
        }
    }
}