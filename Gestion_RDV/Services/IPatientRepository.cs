using System.Collections.Generic;
using System.Threading.Tasks;
using Gestion_RDV.Models;

namespace Gestion_RDV.Services
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<int> AddAsync(Patient patient);
        Task<int> UpdateAsync(Patient patient);
        Task<int> DeleteAsync(int id);
        Task<List<Patient>> SearchAsync(string query);
    }
}