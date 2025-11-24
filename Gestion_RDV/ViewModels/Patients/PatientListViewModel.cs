using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;
using Microsoft.Maui.Controls;

namespace Gestion_RDV.ViewModels.Patients
{
    public class PatientListViewModel : BaseViewModel
    {
        private readonly IPatientRepository _patientRepository;

        public ObservableCollection<Patient> Patients { get; } = new ObservableCollection<Patient>();

        public ICommand LoadPatientsCommand { get; }

        // Constructeur sans paramètre pour le designer XAML
        public PatientListViewModel() : this(null!)
        {
        }

        public PatientListViewModel(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
            LoadPatientsCommand = new Command(async () => await LoadAsync());
        }

        public async Task LoadAsync()
        {
            if (_patientRepository == null)
                return;

            Patients.Clear();
            var items = await _patientRepository.GetAllAsync();
            foreach (var p in items)
                Patients.Add(p);
        }
    }
}