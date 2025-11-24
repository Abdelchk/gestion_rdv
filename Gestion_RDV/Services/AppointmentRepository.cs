using System.Collections.Generic;
using System.Threading.Tasks;
using Gestion_RDV.Models;
using SQLite;

namespace Gestion_RDV.Services
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IDatabaseService _databaseService;

        public AppointmentRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        private SQLiteAsyncConnection Conn => _databaseService.GetConnection();

        public Task<int> AddAsync(RendezVous rendezVous) => Conn.InsertAsync(rendezVous);
        public Task<int> DeleteAsync(int id) => Conn.DeleteAsync<RendezVous>(id);
        public Task<List<RendezVous>> GetAllAsync() => Conn.Table<RendezVous>().ToListAsync();
        public async Task<RendezVous?> GetByIdAsync(int id) => await Conn.FindAsync<RendezVous>(id);
        public Task<int> UpdateAsync(RendezVous rendezVous) => Conn.UpdateAsync(rendezVous);
        public Task<List<RendezVous>> GetByRangeAsync(System.DateTime start, System.DateTime end)
        {
            return Conn.QueryAsync<RendezVous>("SELECT * FROM RendezVous WHERE Start >= ? AND End <= ?", start, end);
        }
    }
}