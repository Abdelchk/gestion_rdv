using System.Collections.ObjectModel;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Patients;

public class PatientListViewModel : BaseViewModel
{
    private readonly DatabaseService _database;

    public ObservableCollection<Patient> Patients { get; set; } = new();

    public PatientListViewModel(DatabaseService db)
    {
        _database = db;
    }

    public async Task LoadAsync()
    {
        Patients.Clear();
        var list = await _database.GetPatientsAsync();
        foreach (var p in list)
            Patients.Add(p);
    }

    public void FilterPatients(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            _ = LoadAsync();
            return;
        }

        var filtered = Patients
            .Where(p =>
                p.FullName.ToLower().Contains(text.ToLower()) ||
                p.Email.ToLower().Contains(text.ToLower()) ||
                p.Phone.ToLower().Contains(text.ToLower()))
            .ToList();

        Patients.Clear();
        foreach (var p in filtered)
            Patients.Add(p);
    }

    public async Task DeleteAsync(Patient p)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Supprimer",
            $"Supprimer {p.FullName} ?",
            "Oui", "Non");

        if (!confirm) return;

        await _database.DeletePatientAsync(p);
        await LoadAsync();
    }
}
