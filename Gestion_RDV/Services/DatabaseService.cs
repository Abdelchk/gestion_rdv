using SQLite;
using Gestion_RDV.Models;

namespace Gestion_RDV.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection _db;

    private async Task InitAsync()
    {
        if (_db != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "gestion_rdv.db");
        _db = new SQLiteAsyncConnection(dbPath);

        // Afficher le chemin de la base de données dans la console de debug
        System.Diagnostics.Debug.WriteLine("===========================================");
        System.Diagnostics.Debug.WriteLine($"📂 DATABASE LOCATION:");
        System.Diagnostics.Debug.WriteLine($"   {dbPath}");
        System.Diagnostics.Debug.WriteLine("===========================================");

        await _db.CreateTableAsync<Patient>();
        await _db.CreateTableAsync<Medecin>();
        await _db.CreateTableAsync<Appointment>();
        await _db.CreateTableAsync<User>();
    }

    /// <summary>
    /// Obtient le chemin complet de la base de données
    /// </summary>
    public string GetDatabasePath()
    {
        return Path.Combine(FileSystem.AppDataDirectory, "gestion_rdv.db");
    }

    // ------------------------------------------------------
    // USERS (AUTHENTICATION)
    // ------------------------------------------------------
    public async Task<User> GetUserByEmailAsync(string email)
    {
        await InitAsync();
        return await _db.Table<User>()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        await InitAsync();
        return await _db.FindAsync<User>(id);
    }

    public async Task<int> AddUserAsync(User user)
    {
        await InitAsync();
        return await _db.InsertAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        await InitAsync();
        await _db.UpdateAsync(user);
    }

    public async Task<int> GetUserCountAsync()
    {
        await InitAsync();
        return await _db.Table<User>().CountAsync();
    }

    // ------------------------------------------------------
    // PATIENTS
    // ------------------------------------------------------
    public async Task<List<Patient>> GetPatientsAsync()
    {
        await InitAsync();
        return await _db.Table<Patient>().ToListAsync();
    }

    public async Task<Patient> GetPatientAsync(int id)
    {
        await InitAsync();
        return await _db.FindAsync<Patient>(id);
    }

    public async Task AddPatientAsync(Patient p)
    {
        await InitAsync();
        await _db.InsertAsync(p);
    }

    public async Task UpdatePatientAsync(Patient p)
    {
        await InitAsync();
        await _db.UpdateAsync(p);
    }

    public async Task DeletePatientAsync(Patient p)
    {
        await InitAsync();
        await _db.DeleteAsync(p);
    }

    // ------------------------------------------------------
    // MEDECINS
    // ------------------------------------------------------
    public async Task<List<Medecin>> GetMedecinsAsync()
    {
        await InitAsync();
        return await _db.Table<Medecin>().ToListAsync();
    }

    public async Task<Medecin> GetMedecinAsync(int id)
    {
        await InitAsync();
        return await _db.FindAsync<Medecin>(id);
    }

    public async Task AddMedecinAsync(Medecin m)
    {
        await InitAsync();
        await _db.InsertAsync(m);
    }

    public async Task UpdateMedecinAsync(Medecin m)
    {
        await InitAsync();
        await _db.UpdateAsync(m);
    }

    public async Task DeleteMedecinAsync(Medecin m)
    {
        await InitAsync();
        await _db.DeleteAsync(m);
    }

    // ------------------------------------------------------
    // APPOINTMENTS (RENDEZ-VOUS)
    // ------------------------------------------------------
    public async Task<List<Appointment>> GetAppointmentsAsync()
    {
        await InitAsync();
        return await _db.Table<Appointment>().ToListAsync();
    }

    public async Task AddAppointmentAsync(Appointment rdv)
    {
        await InitAsync();
        await _db.InsertAsync(rdv);
    }

    public async Task UpdateAppointmentAsync(Appointment rdv)
    {
        await InitAsync();
        await _db.UpdateAsync(rdv);
    }

    public async Task DeleteAppointmentAsync(Appointment rdv)
    {
        await InitAsync();
        await _db.DeleteAsync(rdv);
    }

    // ------------------------------------------------------
    // RDV DU JOUR
    // ------------------------------------------------------
    public async Task<List<Appointment>> GetAppointmentsForDayAsync(DateTime day)
    {
        await InitAsync();

        DateTime start = day.Date;
        DateTime end = day.Date.AddDays(1);

        return await _db.Table<Appointment>()
            .Where(a => a.DateTime >= start && a.DateTime < end)
            .ToListAsync();
    }

    // ------------------------------------------------------
    // RDV D’UN MÉDECIN
    // ------------------------------------------------------
    public async Task<List<Appointment>> GetAppointmentsByMedecinAsync(int medecinId)
    {
        await InitAsync();
        return await _db.Table<Appointment>()
            .Where(a => a.MedecinId == medecinId)
            .ToListAsync();
    }

    // ------------------------------------------------------
    // CHECK COLLISION (empêcher 2 RDV au même moment)
    // ------------------------------------------------------
    public async Task<bool> HasCollisionAsync(DateTime dateTime, int medecinId)
    {
        await InitAsync();

        return await _db.Table<Appointment>()
            .Where(a =>
                a.MedecinId == medecinId &&
                a.DateTime == dateTime
            )
            .FirstOrDefaultAsync() != null;
    }

    /// <summary>
    /// Vérifie les conflits de rendez-vous avec un intervalle de 10 minutes
    /// pour le médecin ET le patient
    /// </summary>
    public async Task<AppointmentConflict> CheckAppointmentConflictAsync(
        DateTime dateTime, 
        int patientId, 
        int medecinId, 
        int? currentAppointmentId = null)
    {
        await InitAsync();

        var startTime = dateTime.AddMinutes(-10);
        var endTime = dateTime.AddMinutes(10);

        // Récupérer tous les rendez-vous dans la plage horaire
        var allAppointments = await _db.Table<Appointment>().ToListAsync();

        // Filtrer en mémoire pour éviter les problèmes de compilation SQLite
        var conflictingAppointments = allAppointments
            .Where(a => a.DateTime >= startTime && a.DateTime <= endTime)
            .Where(a => currentAppointmentId == null || a.Id != currentAppointmentId.Value)
            .ToList();

        // Vérifier les conflits pour le médecin
        var medecinConflict = conflictingAppointments
            .FirstOrDefault(a => a.MedecinId == medecinId);

        if (medecinConflict != null)
        {
            var patient = await GetPatientAsync(medecinConflict.PatientId);
            var medecin = await GetMedecinAsync(medecinId);
            return new AppointmentConflict
            {
                HasConflict = true,
                ConflictType = ConflictType.Medecin,
                ConflictingAppointment = medecinConflict,
                Message = $"Le Dr. {medecin?.FullName} a déjà un rendez-vous avec {patient?.FullName} à {medecinConflict.DateTime:HH:mm}"
            };
        }

        // Vérifier les conflits pour le patient
        var patientConflict = conflictingAppointments
            .FirstOrDefault(a => a.PatientId == patientId);

        if (patientConflict != null)
        {
            var patient = await GetPatientAsync(patientId);
            var medecin = await GetMedecinAsync(patientConflict.MedecinId);
            return new AppointmentConflict
            {
                HasConflict = true,
                ConflictType = ConflictType.Patient,
                ConflictingAppointment = patientConflict,
                Message = $"{patient?.FullName} a déjà un rendez-vous avec le Dr. {medecin?.FullName} à {patientConflict.DateTime:HH:mm}"
            };
        }

        return new AppointmentConflict { HasConflict = false };
    }
}

public enum ConflictType
{
    None,
    Medecin,
    Patient
}

public class AppointmentConflict
{
    public bool HasConflict { get; set; }
    public ConflictType ConflictType { get; set; }
    public Appointment ConflictingAppointment { get; set; }
    public string Message { get; set; }
}
