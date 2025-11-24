using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Medecins;

[QueryProperty(nameof(MedecinId), "MedecinId")]
public class MedecinFormViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    private string _medecinId;
    public string MedecinId
    {
        get => _medecinId;
        set
        {
            _medecinId = value;
            OnPropertyChanged();
            // Charger les données du médecin lorsque l'ID est défini
            if (!string.IsNullOrWhiteSpace(value))
            {
                Task.Run(async () => await LoadAsync());
            }
        }
    }

    private string _firstName;
    public string FirstName
    {
        get => _firstName;
        set { _firstName = value; OnPropertyChanged(); }
    }

    private string _lastName;
    public string LastName
    {
        get => _lastName;
        set { _lastName = value; OnPropertyChanged(); }
    }

    private string _specialite;
    public string Specialite
    {
        get => _specialite;
        set { _specialite = value; OnPropertyChanged(); }
    }

    private string _email;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    private string _phone;
    public string Phone
    {
        get => _phone;
        set { _phone = value; OnPropertyChanged(); }
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public MedecinFormViewModel() : this(null!)
    {
    }

    public MedecinFormViewModel(DatabaseService db)
    {
        _db = db;
        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await CancelAsync());
    }

    public async Task LoadAsync()
    {
        if (string.IsNullOrWhiteSpace(MedecinId) || _db == null)
            return;

        var medecin = await _db.GetMedecinAsync(int.Parse(MedecinId));
        if (medecin == null) return;

        FirstName = medecin.FirstName;
        LastName = medecin.LastName;
        Specialite = medecin.Specialite;
        Email = medecin.Email;
        Phone = medecin.Phone;
    }

    private async Task SaveAsync()
    {
        if (_db == null)
            return;

        // Validation
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            await Shell.Current.DisplayAlert("Erreur", "Le nom et le prénom sont obligatoires", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Specialite))
        {
            await Shell.Current.DisplayAlert("Erreur", "La spécialité est obligatoire", "OK");
            return;
        }

        IsBusy = true;

        try
        {
            if (string.IsNullOrWhiteSpace(MedecinId))
            {
                // Création d'un nouveau médecin
                await _db.AddMedecinAsync(new Medecin
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Specialite = Specialite,
                    Email = Email ?? string.Empty,
                    Phone = Phone ?? string.Empty
                });
            }
            else
            {
                // Mise à jour d'un médecin existant
                var m = await _db.GetMedecinAsync(int.Parse(MedecinId));
                if (m != null)
                {
                    m.FirstName = FirstName;
                    m.LastName = LastName;
                    m.Specialite = Specialite;
                    m.Email = Email ?? string.Empty;
                    m.Phone = Phone ?? string.Empty;

                    await _db.UpdateMedecinAsync(m);
                }
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
