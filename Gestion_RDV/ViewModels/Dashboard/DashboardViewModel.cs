using System.Collections.ObjectModel;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Dashboard
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        public int TotalPatients { get; set; }
        public int TotalMedecins { get; set; }
        public int AppointmentsToday { get; set; }
        public int UrgencesToday { get; set; }

        public ObservableCollection<TodayAppointmentItem> TodayAppointments { get; set; } = new();

        public DashboardViewModel()
        {
            LoadData();
        }

        private async void LoadData()
        {
            TotalPatients = (await _db.GetPatientsAsync()).Count;
            TotalMedecins = (await _db.GetMedecinsAsync()).Count;

            var today = await _db.GetAppointmentsForDayAsync(DateTime.Today);

            AppointmentsToday = today.Count;
            UrgencesToday = today.Count(a => a.Type == AppointmentType.Urgence);

            TodayAppointments.Clear();
            foreach (var rdv in today)
            {
                var pat = await _db.GetPatientAsync(rdv.PatientId);
                var doc = (await _db.GetMedecinsAsync()).FirstOrDefault(m => m.Id == rdv.MedecinId);

                TodayAppointments.Add(new TodayAppointmentItem
                {
                    PatientName = pat?.FullName ?? "Inconnu",
                    MedecinName = doc?.FullName ?? "Médecin",
                    Notes = rdv.Notes,
                    Time = rdv.DateTime.ToString("HH:mm")
                });
            }

            OnPropertyChanged(nameof(TotalPatients));
            OnPropertyChanged(nameof(TotalMedecins));
            OnPropertyChanged(nameof(AppointmentsToday));
            OnPropertyChanged(nameof(UrgencesToday));
            OnPropertyChanged(nameof(TodayAppointments));
        }
    }

    public class TodayAppointmentItem
    {
        public string PatientName { get; set; }
        public string MedecinName { get; set; }
        public string Notes { get; set; }
        public string Time { get; set; }
    }
}
