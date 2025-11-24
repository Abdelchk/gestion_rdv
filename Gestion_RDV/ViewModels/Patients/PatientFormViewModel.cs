using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Patients;

[QueryProperty(nameof(PatientId), "PatientId")]
public class PatientFormViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    private string _patientId;
    public string PatientId
    {
        get => _patientId;
        set
        {
            _patientId = value;
            OnPropertyChanged();
            // Charger les données du patient lorsque l'ID est défini
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

    private string _birthDateString;
    public string BirthDateString
    {
        get => _birthDateString;
        set { _birthDateString = value; OnPropertyChanged(); }
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

    private string _notes;
    public string Notes
    {
        get => _notes;
        set { _notes = value; OnPropertyChanged(); }
    }

    public ICommand SaveCommand { get; }

    public PatientFormViewModel(DatabaseService db)
    {
        _db = db;
        SaveCommand = new Command(async () => await SaveAsync());
    }

    public async Task LoadAsync()
    {
        if (string.IsNullOrWhiteSpace(PatientId))
            return;

        var patient = await _db.GetPatientAsync(int.Parse(PatientId));
        if (patient == null) return;

        FirstName = patient.FirstName;
        LastName = patient.LastName;
        Email = patient.Email;
        Phone = patient.Phone;
        Notes = patient.Notes;
        BirthDateString = patient.DateNaissance.ToString("dd/MM/yyyy");
    }

    private async Task SaveAsync()
    {
        DateTime birth = DateTime.Parse(BirthDateString);

        if (string.IsNullOrWhiteSpace(PatientId))
        {
            await _db.AddPatientAsync(new Patient
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Phone = Phone,
                DateNaissance = birth,
                Notes = Notes
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
                p.Notes = Notes;
                p.DateNaissance = birth;

                await _db.UpdatePatientAsync(p);
            }
        }

        await Shell.Current.GoToAsync("..");
    }
}
