using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;
using Microsoft.Maui.Controls;

namespace Gestion_RDV.ViewModels.RendezVous
{
    public class AppointmentListViewModel : BaseViewModel
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public ObservableCollection<RendezVous> Appointments { get; } = new ObservableCollection<RendezVous>();
        public ICommand LoadAppointmentsCommand { get; }

        public AppointmentListViewModel(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
            LoadAppointmentsCommand = new Command(async () => await LoadAsync());
        }

        public async Task LoadAsync()
        {
            Appointments.Clear();
            var items = await _appointmentRepository.GetAllAsync();
            foreach (var a in items)
                Appointments.Add(a);
        }
    }
}