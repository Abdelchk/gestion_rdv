using System.Collections.ObjectModel;
using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Medecins;

public class MedecinListViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterMedecins();
        }
    }

    public ObservableCollection<Medecin> Medecins { get; } = new();
    public ObservableCollection<Medecin> FilteredMedecins { get; } = new();

    public ICommand AddMedecinCommand { get; }
    public ICommand EditMedecinCommand { get; }
    public ICommand DeleteMedecinCommand { get; }
    public ICommand RefreshCommand { get; }

    public MedecinListViewModel() : this(null!)
    {
    }

    public MedecinListViewModel(DatabaseService db)
    {
        _db = db;

        AddMedecinCommand = new Command(async () => await AddMedecinAsync());
        EditMedecinCommand = new Command<Medecin>(async (m) => await EditMedecinAsync(m));
        DeleteMedecinCommand = new Command<Medecin>(async (m) => await DeleteMedecinAsync(m));
        RefreshCommand = new Command(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        if (_db == null)
            return;

        IsBusy = true;

        try
        {
            var medecins = await _db.GetMedecinsAsync();

            Medecins.Clear();
            FilteredMedecins.Clear();

            foreach (var m in medecins)
            {
                Medecins.Add(m);
                FilteredMedecins.Add(m);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void FilterMedecins()
    {
        FilteredMedecins.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var m in Medecins)
                FilteredMedecins.Add(m);
        }
        else
        {
            var query = SearchText.ToLower();
            foreach (var m in Medecins)
            {
                if (m.FirstName.ToLower().Contains(query) ||
                    m.LastName.ToLower().Contains(query) ||
                    m.Specialite.ToLower().Contains(query) ||
                    m.Phone.Contains(query) ||
                    m.Email.ToLower().Contains(query))
                {
                    FilteredMedecins.Add(m);
                }
            }
        }
    }

    private async Task AddMedecinAsync()
    {
        await Shell.Current.GoToAsync("MedecinFormPage");
    }

    private async Task EditMedecinAsync(Medecin medecin)
    {
        if (medecin == null)
            return;

        await Shell.Current.GoToAsync($"MedecinFormPage?MedecinId={medecin.Id}");
    }

    private async Task DeleteMedecinAsync(Medecin medecin)
    {
        if (medecin == null)
            return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirmation",
            $"Voulez-vous vraiment supprimer le Dr. {medecin.FullName} ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        await _db.DeleteMedecinAsync(medecin);
        await LoadAsync();
    }

    public int GetAppointmentCount(Medecin medecin)
    {
        // Cette méthode pourrait être améliorée en chargeant les comptes depuis la BDD
        return 0;
    }
}
