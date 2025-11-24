using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gestion_RDV.Models;

namespace Gestion_RDV.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> HasConflictAsync(RendezVous appointment)
        {
            var items = await _repo.GetByRangeAsync(appointment.Start.Date, appointment.End.Date.AddDays(1));
            // check overlap excluding itself
            return items.Any(a => a.Id != appointment.Id && (appointment.Start < a.End && a.Start < appointment.End));
        }

        public Task<List<RendezVous>> GetAppointmentsForDayAsync(DateTime day)
        {
            var start = day.Date;
            var end = start.AddDays(1).AddTicks(-1);
            return _repo.GetByRangeAsync(start, end);
        }
    }
}