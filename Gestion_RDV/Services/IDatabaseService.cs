using SQLite;
using System.Threading.Tasks;

namespace Gestion_RDV.Services
{
    public interface IDatabaseService
    {
        Task InitializeAsync();
        SQLiteAsyncConnection GetConnection();
    }
}