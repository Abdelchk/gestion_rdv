using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Patients;

[QueryProperty(nameof(PatientId), "PatientId")]
public class PatientFormViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    private string _patientId = string.Empty;
    public string PatientId
    {
        get => _patientId;
        set
        {
            _patientId = value;
            OnPropertyChanged();
            if (!string.IsNullOrWhiteSpace(value))
            {
                Task.Run(async () => await LoadAsync());
            }
        }
    }

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set { _firstName = value; OnPropertyChanged(); }
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set { _lastName = value; OnPropertyChanged(); }
    }

    private DateTime _birthDate = DateTime.Today.AddYears(-30);
    public DateTime BirthDate
    {
        get => _birthDate;
        set { _birthDate = value; OnPropertyChanged(); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set { _phone = value; OnPropertyChanged(); }
    }

    private string _notes = string.Empty;
    public string Notes
    {
        get => _notes;
        set { _notes = value; OnPropertyChanged(); }
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public PatientFormViewModel() : this(null!)
    {
    }

    public PatientFormViewModel(DatabaseService db)
    {
        _db = db;
        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await CancelAsync());
    }

    public async Task LoadAsync()
    {
        if (string.IsNullOrWhiteSpace(PatientId) || _db == null)
            return;

        var patient = await _db.GetPatientAsync(int.Parse(PatientId));
        if (patient == null) return;

        FirstName = patient.FirstName;
        LastName = patient.LastName;
        Email = patient.Email;
        Phone = patient.Phone;
        Notes = patient.Notes;
        BirthDate = patient.DateNaissance;
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

        if (string.IsNullOrWhiteSpace(Phone))
        {
            await Shell.Current.DisplayAlert("Erreur", "Le téléphone est obligatoire", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            await Shell.Current.DisplayAlert("Erreur", "L'email est obligatoire", "OK");
            return;
        }

        IsBusy = true;

        try
        {
            if (string.IsNullOrWhiteSpace(PatientId))
            {
                await _db.AddPatientAsync(new Patient
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Phone = Phone,
                    DateNaissance = BirthDate,
                    Notes = Notes ?? string.Empty
                });
            }
            else
            {
                var p = await _db.GetPatientAsync(int.Parse(PatientId));
                if (p != null)
                {
                    p.FirstName = FirstName;
                    p.LastName = LastName;
                    p.Email = Email;
                    p.Phone = Phone;
                    p.Notes = Notes ?? string.Empty;
                    p.DateNaissance = BirthDate;

                    await _db.UpdatePatientAsync(p);
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
