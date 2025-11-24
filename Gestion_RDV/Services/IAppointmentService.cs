using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gestion_RDV.Models;

namespace Gestion_RDV.Services
{
    public interface IAppointmentService
    {
        Task<bool> HasConflictAsync(RendezVous appointment);
        Task<List<RendezVous>> GetAppointmentsForDayAsync(DateTime day);
    }
}