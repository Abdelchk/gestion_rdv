using System.Collections.ObjectModel;
using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Dashboard
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly DatabaseService _db;

        private int _totalPatients;
        public int TotalPatients
        {
            get => _totalPatients;
            set { _totalPatients = value; OnPropertyChanged(); }
        }

        private int _totalMedecins;
        public int TotalMedecins
        {
            get => _totalMedecins;
            set { _totalMedecins = value; OnPropertyChanged(); }
        }

        private int _appointmentsToday;
        public int AppointmentsToday
        {
            get => _appointmentsToday;
            set { _appointmentsToday = value; OnPropertyChanged(); }
        }

        private int _urgencesToday;
        public int UrgencesToday
        {
            get => _urgencesToday;
            set { _urgencesToday = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TodayAppointmentItem> TodayAppointments { get; set; } = new();

        public ICommand NewAppointmentCommand { get; }
        public ICommand NewPatientCommand { get; }
        public ICommand NewMedecinCommand { get; }
        public ICommand RefreshCommand { get; }

        public DashboardViewModel() : this(null!)
        {
        }

        public DashboardViewModel(DatabaseService db)
        {
            _db = db;
            
            NewAppointmentCommand = new Command(async () => await CreateNewAppointmentAsync());
            NewPatientCommand = new Command(async () => await CreateNewPatientAsync());
            NewMedecinCommand = new Command(async () => await CreateNewMedecinAsync());
            RefreshCommand = new Command(async () => await LoadDataAsync());
        }

        public async Task LoadDataAsync()
        {
            if (_db == null)
                return;

            IsBusy = true;

            try
            {
                TotalPatients = (await _db.GetPatientsAsync()).Count;
                TotalMedecins = (await _db.GetMedecinsAsync()).Count;

                var today = await _db.GetAppointmentsForDayAsync(DateTime.Today);

                AppointmentsToday = today.Count;
                UrgencesToday = today.Count(a => a.Status == AppointmentStatus.Urgent);

                TodayAppointments.Clear();
                
                var sortedAppointments = today.OrderBy(a => a.DateTime).ToList();
                
                foreach (var rdv in sortedAppointments)
                {
                    var pat = await _db.GetPatientAsync(rdv.PatientId);
                    var doc = await _db.GetMedecinAsync(rdv.MedecinId);

                    TodayAppointments.Add(new TodayAppointmentItem
                    {
                        AppointmentId = rdv.Id,
                        PatientName = pat?.FullName ?? "Inconnu",
                        MedecinName = doc?.FullName ?? "Médecin",
                        Type = rdv.TypeLabel,
                        Notes = rdv.Notes,
                        Time = rdv.DateTime.ToString("HH:mm"),
                        Status = rdv.Status,
                        StatusLabel = rdv.StatusLabel
                    });
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CreateNewAppointmentAsync()
        {
            await Shell.Current.GoToAsync("AppointmentFormPage");
        }

        private async Task CreateNewPatientAsync()
        {
            await Shell.Current.GoToAsync("PatientFormPage");
        }

        private async Task CreateNewMedecinAsync()
        {
            await Shell.Current.GoToAsync("MedecinFormPage");
        }
    }

    public class TodayAppointmentItem
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string MedecinName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public AppointmentStatus Status { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        
        public string BorderColor => Status switch
        {
            AppointmentStatus.Urgent => "#EF4444",
            AppointmentStatus.Cancelled => "#9CA3AF",
            _ => "#3B82F6"
        };
    }
}
