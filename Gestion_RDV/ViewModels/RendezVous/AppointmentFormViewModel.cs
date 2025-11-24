using System.Collections.ObjectModel;
using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.RendezVous;

[QueryProperty(nameof(AppointmentId), "AppointmentId")]
public class AppointmentFormViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    private string _appointmentId;
    public string AppointmentId
    {
        get => _appointmentId;
        set
        {
            _appointmentId = value;
            OnPropertyChanged();
            if (!string.IsNullOrWhiteSpace(value))
            {
                Task.Run(async () => await LoadAsync());
            }
        }
    }

    private Patient _selectedPatient;
    public Patient SelectedPatient
    {
        get => _selectedPatient;
        set { _selectedPatient = value; OnPropertyChanged(); }
    }

    private Medecin _selectedMedecin;
    public Medecin SelectedMedecin
    {
        get => _selectedMedecin;
        set { _selectedMedecin = value; OnPropertyChanged(); }
    }

    private DateTime _date = DateTime.Today;
    public DateTime Date
    {
        get => _date;
        set { _date = value; OnPropertyChanged(); }
    }

    private TimeSpan _time = DateTime.Now.TimeOfDay;
    public TimeSpan Time
    {
        get => _time;
        set { _time = value; OnPropertyChanged(); }
    }

    private AppointmentType _type = AppointmentType.Consultation;
    public AppointmentType Type
    {
        get => _type;
        set { _type = value; OnPropertyChanged(); }
    }

    private AppointmentStatus _status = AppointmentStatus.Normal;
    public AppointmentStatus Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    private string _notes;
    public string Notes
    {
        get => _notes;
        set { _notes = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Patient> Patients { get; } = new();
    public ObservableCollection<Medecin> Medecins { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public AppointmentFormViewModel() : this(null!)
    {
    }

    public AppointmentFormViewModel(DatabaseService db)
    {
        _db = db;
        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await CancelAsync());
        
        Task.Run(async () => await LoadDataAsync());
    }

    private async Task LoadDataAsync()
    {
        if (_db == null)
            return;

        var patients = await _db.GetPatientsAsync();
        var medecins = await _db.GetMedecinsAsync();

        Patients.Clear();
        Medecins.Clear();

        foreach (var p in patients)
            Patients.Add(p);

        foreach (var m in medecins)
            Medecins.Add(m);
    }

    public async Task LoadAsync()
    {
        if (string.IsNullOrWhiteSpace(AppointmentId) || _db == null)
            return;

        var appointment = await _db.GetAppointmentsAsync();
        var rdv = appointment.FirstOrDefault(a => a.Id == int.Parse(AppointmentId));

        if (rdv == null) return;

        SelectedPatient = Patients.FirstOrDefault(p => p.Id == rdv.PatientId);
        SelectedMedecin = Medecins.FirstOrDefault(m => m.Id == rdv.MedecinId);
        Date = rdv.DateTime.Date;
        Time = rdv.DateTime.TimeOfDay;
        Type = rdv.Type;
        Status = rdv.Status;
        Notes = rdv.Notes;
    }

    private async Task SaveAsync()
    {
        if (_db == null)
            return;

        // Validation
        if (SelectedPatient == null)
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un patient", "OK");
            return;
        }

        if (SelectedMedecin == null)
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un médecin", "OK");
            return;
        }

        IsBusy = true;

        try
        {
            var dateTime = Date.Date + Time;

            // Vérifier les collisions
            var hasCollision = await _db.HasCollisionAsync(dateTime, SelectedMedecin.Id);
            if (hasCollision && string.IsNullOrWhiteSpace(AppointmentId))
            {
                var result = await Shell.Current.DisplayAlert(
                    "Conflit de rendez-vous",
                    $"Le Dr. {SelectedMedecin.FullName} a déjà un rendez-vous à {Time:hh\\:mm}. Voulez-vous continuer?",
                    "Oui",
                    "Non");

                if (!result)
                    return;
            }

            if (string.IsNullOrWhiteSpace(AppointmentId))
            {
                // Création
                await _db.AddAppointmentAsync(new Appointment
                {
                    PatientId = SelectedPatient.Id,
                    MedecinId = SelectedMedecin.Id,
                    DateTime = dateTime,
                    Type = Type,
                    Status = Status,
                    Notes = Notes ?? string.Empty
                });
            }
            else
            {
                // Mise à jour
                var appointments = await _db.GetAppointmentsAsync();
                var rdv = appointments.FirstOrDefault(a => a.Id == int.Parse(AppointmentId));
                
                if (rdv != null)
                {
                    rdv.PatientId = SelectedPatient.Id;
                    rdv.MedecinId = SelectedMedecin.Id;
                    rdv.DateTime = dateTime;
                    rdv.Type = Type;
                    rdv.Status = Status;
                    rdv.Notes = Notes ?? string.Empty;

                    await _db.UpdateAppointmentAsync(rdv);
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
