using System.Collections.Generic;
using System.Threading.Tasks;
using Gestion_RDV.Models;

namespace Gestion_RDV.Services
{
    public interface IAppointmentRepository
    {
        Task<List<RendezVous>> GetAllAsync();
        Task<RendezVous?> GetByIdAsync(int id);
        Task<int> AddAsync(RendezVous rendezVous);
        Task<int> UpdateAsync(RendezVous rendezVous);
        Task<int> DeleteAsync(int id);
        Task<List<RendezVous>> GetByRangeAsync(System.DateTime start, System.DateTime end);
    }
}