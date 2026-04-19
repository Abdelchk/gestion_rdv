using System.Collections.ObjectModel;
using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.RendezVous;

public class AppointmentListViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterAppointments();
        }
    }

    private AppointmentType? _selectedType;
    public AppointmentType? SelectedType
    {
        get => _selectedType;
        set
        {
            _selectedType = value;
            OnPropertyChanged();
            FilterAppointments();
        }
    }

    private AppointmentStatus? _selectedStatus;
    public AppointmentStatus? SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value;
            OnPropertyChanged();
            FilterAppointments();
        }
    }

    private int? _selectedMedecinId;
    public int? SelectedMedecinId
    {
        get => _selectedMedecinId;
        set
        {
            _selectedMedecinId = value;
            OnPropertyChanged();
            FilterAppointments();
        }
    }

    private DateTime? _selectedDate;
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            _selectedDate = value;
            OnPropertyChanged();
            FilterAppointments();
        }
    }

    public ObservableCollection<Appointment> Appointments { get; } = new();
    public ObservableCollection<AppointmentDisplayItem> FilteredAppointments { get; } = new();
    public ObservableCollection<Medecin> Medecins { get; } = new();

    public ICommand AddAppointmentCommand { get; }
    public ICommand EditAppointmentCommand { get; }
    public ICommand DeleteAppointmentCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    public AppointmentListViewModel() : this(null!)
    {
    }

    public AppointmentListViewModel(DatabaseService db)
    {
        _db = db;

        AddAppointmentCommand = new Command(async () => await AddAppointmentAsync());
        EditAppointmentCommand = new Command<AppointmentDisplayItem>(async (a) => await EditAppointmentAsync(a));
        DeleteAppointmentCommand = new Command<AppointmentDisplayItem>(async (a) => await DeleteAppointmentAsync(a));
        RefreshCommand = new Command(async () => await LoadAsync());
        ClearFiltersCommand = new Command(ClearFilters);
    }

    public async Task LoadAsync()
    {
        if (_db == null)
            return;

        IsBusy = true;

        try
        {
            var appointments = await _db.GetAppointmentsAsync();
            var medecins = await _db.GetMedecinsAsync();

            Appointments.Clear();
            Medecins.Clear();

            foreach (var a in appointments)
                Appointments.Add(a);

            foreach (var m in medecins)
                Medecins.Add(m);

            await FilterAppointments();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task FilterAppointments()
    {
        if (_db == null)
            return;

        FilteredAppointments.Clear();

        var filtered = Appointments.AsEnumerable();

        // Filtre par type
        if (SelectedType.HasValue)
            filtered = filtered.Where(a => a.Type == SelectedType.Value);

        // Filtre par statut
        if (SelectedStatus.HasValue)
            filtered = filtered.Where(a => a.Status == SelectedStatus.Value);

        // Filtre par médecin
        if (SelectedMedecinId.HasValue && SelectedMedecinId.Value > 0)
            filtered = filtered.Where(a => a.MedecinId == SelectedMedecinId.Value);

        // Filtre par date
        if (SelectedDate.HasValue)
            filtered = filtered.Where(a => a.DateTime.Date == SelectedDate.Value.Date);

        // Filtre par recherche textuelle
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var query = SearchText.ToLower();
            filtered = filtered.Where(a =>
            {
                var patient = _db.GetPatientAsync(a.PatientId).Result;
                var medecin = _db.GetMedecinAsync(a.MedecinId).Result;
                return (patient?.FullName.ToLower().Contains(query) ?? false) ||
                       (medecin?.FullName.ToLower().Contains(query) ?? false) ||
                       a.Notes.ToLower().Contains(query);
            });
        }

        var sortedList = filtered.OrderBy(a => a.DateTime).ToList();

        foreach (var appointment in sortedList)
        {
            var patient = await _db.GetPatientAsync(appointment.PatientId);
            var medecin = await _db.GetMedecinAsync(appointment.MedecinId);

            FilteredAppointments.Add(new AppointmentDisplayItem
            {
                Appointment = appointment,
                PatientName = patient?.FullName ?? "Inconnu",
                MedecinName = medecin?.FullName ?? "Médecin non assigné",
                Date = appointment.DateTime.ToString("dddd dd MMMM yyyy", new System.Globalization.CultureInfo("fr-FR")),
                Time = appointment.DateTime.ToString("HH:mm"),
                TypeLabel = appointment.TypeLabel,
                StatusLabel = appointment.StatusLabel,
                Notes = appointment.Notes,
                BorderColor = appointment.Status switch
                {
                    AppointmentStatus.Urgent => "#EF4444",
                    AppointmentStatus.Cancelled => "#9CA3AF",
                    _ => "#3B82F6"
                }
            });
        }
    }

    private void ClearFilters()
    {
        SelectedType = null;
        SelectedStatus = null;
        SelectedMedecinId = null;
        SelectedDate = null;
        SearchText = string.Empty;
    }

    private async Task AddAppointmentAsync()
    {
        await Shell.Current.GoToAsync("AppointmentFormPage");
    }

    private async Task EditAppointmentAsync(AppointmentDisplayItem item)
    {
        if (item?.Appointment == null)
            return;

        await Shell.Current.GoToAsync($"AppointmentFormPage?AppointmentId={item.Appointment.Id}");
    }

    private async Task DeleteAppointmentAsync(AppointmentDisplayItem item)
    {
        if (item?.Appointment == null)
            return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirmation",
            $"Voulez-vous vraiment supprimer le rendez-vous de {item.PatientName} ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        await _db.DeleteAppointmentAsync(item.Appointment);
        await LoadAsync();
    }
}

public class AppointmentDisplayItem
{
    public Appointment Appointment { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string MedecinName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string TypeLabel { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string BorderColor { get; set; } = "#3B82F6";
}
