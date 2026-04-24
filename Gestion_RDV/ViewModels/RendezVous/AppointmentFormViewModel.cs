using System.Collections.ObjectModel;
using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.RendezVous;

[QueryProperty(nameof(AppointmentId), "AppointmentId")]
public class AppointmentFormViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private bool _isSaving = false;

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
        set 
        { 
            _selectedMedecin = value; 
            OnPropertyChanged();
            // Recharger les créneaux disponibles quand le médecin change
            Task.Run(async () => await LoadAvailableTimeSlotsAsync());
        }
    }

    private DateTime _date = DateTime.Today;
    public DateTime Date
    {
        get => _date;
        set 
        { 
            _date = value; 
            OnPropertyChanged();
            // Recharger les créneaux disponibles quand la date change
            Task.Run(async () => await LoadAvailableTimeSlotsAsync());
        }
    }

    private TimeSlot _selectedTimeSlot;
    public TimeSlot SelectedTimeSlot
    {
        get => _selectedTimeSlot;
        set
        {
            _selectedTimeSlot = value;
            OnPropertyChanged();
            if (value != null)
            {
                Time = value.Time;
            }
        }
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
    public ObservableCollection<TimeSlot> AvailableTimeSlots { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SelectTimeSlotCommand { get; }

    public AppointmentFormViewModel() : this(null!)
    {
    }

    public AppointmentFormViewModel(DatabaseService db)
    {
        _db = db;
        SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
        CancelCommand = new Command(async () => await CancelAsync());
        SelectTimeSlotCommand = new Command<TimeSlot>(SelectTimeSlot);

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

        await LoadAvailableTimeSlotsAsync();
    }

    /// <summary>
    /// Charge les créneaux horaires disponibles pour le médecin sélectionné et la date sélectionnée
    /// </summary>
    private async Task LoadAvailableTimeSlotsAsync()
    {
        if (_db == null || SelectedMedecin == null)
        {
            AvailableTimeSlots.Clear();
            return;
        }

        try
        {
            // Générer les créneaux de 8h à 18h par intervalles de 15 minutes
            var slots = new List<TimeSlot>();
            var startHour = 8;
            var endHour = 18;
            var intervalMinutes = 15;

            var selectedDateTime = Date;
            var now = DateTime.Now;

            // Récupérer tous les rendez-vous du médecin pour la date sélectionnée
            var existingAppointments = await _db.GetAppointmentsForDayAsync(selectedDateTime);
            var medecinAppointments = existingAppointments
                .Where(a => a.MedecinId == SelectedMedecin.Id)
                .ToList();

            // Exclure le rendez-vous en cours de modification
            if (!string.IsNullOrWhiteSpace(AppointmentId))
            {
                var currentId = int.Parse(AppointmentId);
                medecinAppointments = medecinAppointments
                    .Where(a => a.Id != currentId)
                    .ToList();
            }

            for (int hour = startHour; hour < endHour; hour++)
            {
                for (int minute = 0; minute < 60; minute += intervalMinutes)
                {
                    var timeSlot = new TimeSpan(hour, minute, 0);
                    var slotDateTime = selectedDateTime.Date + timeSlot;

                    // Vérifier si c'est dans le passé
                    bool isPast = slotDateTime < now;

                    // Vérifier si le créneau est occupé (avec intervalle de 10 minutes)
                    bool isOccupied = medecinAppointments.Any(a =>
                    {
                        var timeDiff = Math.Abs((a.DateTime - slotDateTime).TotalMinutes);
                        return timeDiff < 10;
                    });

                    slots.Add(new TimeSlot
                    {
                        Time = timeSlot,
                        IsAvailable = !isOccupied,
                        IsPast = isPast
                    });
                }
            }

            // Mettre à jour la collection sur le thread UI
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AvailableTimeSlots.Clear();
                foreach (var slot in slots)
                {
                    AvailableTimeSlots.Add(slot);
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des créneaux : {ex.Message}");
        }
    }

    private void SelectTimeSlot(TimeSlot slot)
    {
        if (slot == null || !slot.IsEnabled)
            return;

        SelectedTimeSlot = slot;
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
        if (_db == null || IsBusy || _isSaving)
            return;

        _isSaving = true;

        // Validation
        if (SelectedPatient == null)
        {
            _isSaving = false;
            await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un patient", "OK");
            return;
        }

        if (SelectedMedecin == null)
        {
            _isSaving = false;
            await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un médecin", "OK");
            return;
        }

        // Vérifier qu'un créneau a été sélectionné
        if (SelectedTimeSlot == null || !SelectedTimeSlot.IsEnabled)
        {
            _isSaving = false;
            await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un créneau horaire disponible", "OK");
            return;
        }

        var dateTime = Date.Date + Time;

        // Vérifier que ce n'est pas dans le passé
        if (dateTime < DateTime.Now)
        {
            _isSaving = false;
            await Shell.Current.DisplayAlert("⚠️ Date invalide", "Vous ne pouvez pas créer un rendez-vous dans le passé.", "OK");
            return;
        }

        IsBusy = true;

        try
        {
            // Vérifier les conflits avec intervalle de 10 minutes
            int? currentId = string.IsNullOrWhiteSpace(AppointmentId) ? null : int.Parse(AppointmentId);
            var conflict = await _db.CheckAppointmentConflictAsync(
                dateTime, 
                SelectedPatient.Id, 
                SelectedMedecin.Id, 
                currentId);

            if (conflict.HasConflict)
            {
                // Bloquer la création (pas de possibilité de forcer)
                await Shell.Current.DisplayAlert(
                    "⚠️ Conflit de rendez-vous",
                    $"{conflict.Message}\n\nIl doit y avoir au moins 10 minutes d'intervalle entre les rendez-vous.\n\nVeuillez choisir une autre heure.",
                    "OK");

                IsBusy = false;
                _isSaving = false;
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
            _isSaving = false;
        }
    }

    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
