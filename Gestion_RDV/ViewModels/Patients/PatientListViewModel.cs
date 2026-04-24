using System.Collections.ObjectModel;
using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Patients;

public class PatientListViewModel : BaseViewModel
{
    private readonly DatabaseService _database;
    private bool _isLoading = false;

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterPatients();
        }
    }

    public ObservableCollection<Patient> Patients { get; } = new();
    public ObservableCollection<Patient> FilteredPatients { get; } = new();

    public ICommand AddPatientCommand { get; }
    public ICommand EditPatientCommand { get; }
    public ICommand DeletePatientCommand { get; }
    public ICommand RefreshCommand { get; }

    public PatientListViewModel() : this(null!)
    {
    }

    public PatientListViewModel(DatabaseService db)
    {
        _database = db;

        AddPatientCommand = new Command(async () => await AddPatientAsync());
        EditPatientCommand = new Command<Patient>(async (p) => await EditPatientAsync(p));
        DeletePatientCommand = new Command<Patient>(async (p) => await DeletePatientAsync(p));
        RefreshCommand = new Command(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        if (_database == null || _isLoading)
            return;

        _isLoading = true;
        IsBusy = true;

        try
        {
            var list = await _database.GetPatientsAsync();

            Patients.Clear();
            FilteredPatients.Clear();

            foreach (var p in list)
            {
                Patients.Add(p);
                FilteredPatients.Add(p);
            }
        }
        finally
        {
            IsBusy = false;
            _isLoading = false;
        }
    }

    private void FilterPatients()
    {
        FilteredPatients.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var p in Patients)
                FilteredPatients.Add(p);
        }
        else
        {
            var query = SearchText.ToLower();
            foreach (var p in Patients)
            {
                if (p.FullName.ToLower().Contains(query) ||
                    p.Email.ToLower().Contains(query) ||
                    p.Phone.Contains(query))
                {
                    FilteredPatients.Add(p);
                }
            }
        }
    }

    private async Task AddPatientAsync()
    {
        await Shell.Current.GoToAsync("PatientFormPage");
    }

    private async Task EditPatientAsync(Patient patient)
    {
        if (patient == null)
            return;

        await Shell.Current.GoToAsync($"PatientFormPage?PatientId={patient.Id}");
    }

    private async Task DeletePatientAsync(Patient patient)
    {
        if (patient == null)
            return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirmation",
            $"Voulez-vous vraiment supprimer {patient.FullName} ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        await _database.DeletePatientAsync(patient);
        await LoadAsync();
    }
}
