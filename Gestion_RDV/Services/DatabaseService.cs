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

        _db = new SQLiteAsyncConnection(
            Path.Combine(FileSystem.AppDataDirectory, "gestion_rdv.db"));

        await _db.CreateTableAsync<Patient>();
        await _db.CreateTableAsync<Medecin>();
        await _db.CreateTableAsync<Appointment>();
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
}
