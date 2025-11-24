using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Patients;

[QueryProperty(nameof(PatientId), "PatientId")]
public class PatientFormViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    public string PatientId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BirthDateString { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Notes { get; set; }

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

        OnPropertyChanged(null);
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
            p.FirstName = FirstName;
            p.LastName = LastName;
            p.Email = Email;
            p.Phone = Phone;
            p.Notes = Notes;
            p.DateNaissance = birth;

            await _db.UpdatePatientAsync(p);
        }

        await Shell.Current.GoToAsync("..");
    }
}
