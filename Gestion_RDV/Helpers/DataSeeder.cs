using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.Helpers;

public static class DataSeeder
{
    public static async Task SeedDataAsync(DatabaseService db)
    {
        // Vérifier si des données existent déjà
        var existingPatients = await db.GetPatientsAsync();
        if (existingPatients.Any())
            return; // Ne pas réinsérer si des données existent

        // Ajouter des patients
        var patients = new List<Patient>
        {
            new Patient
            {
                FirstName = "Marie",
                LastName = "Dupont",
                Email = "marie.dupont@email.com",
                Phone = "06 12 34 56 78",
                DateNaissance = new DateTime(1985, 3, 15),
                Notes = "Allergie aux antibiotiques pénicilline"
            },
            new Patient
            {
                FirstName = "Pierre",
                LastName = "Martin",
                Email = "pierre.martin@email.com",
                Phone = "06 98 76 54 32",
                DateNaissance = new DateTime(1972, 11, 22),
                Notes = "Diabète type 2, suivi régulier nécessaire"
            },
            new Patient
            {
                FirstName = "Sophie",
                LastName = "Bernard",
                Email = "sophie.bernard@email.com",
                Phone = "07 11 22 33 44",
                DateNaissance = new DateTime(1990, 7, 8),
                Notes = "Tension artérielle à surveiller"
            }
        };

        foreach (var patient in patients)
            await db.AddPatientAsync(patient);

        // Ajouter des médecins
        var medecins = new List<Medecin>
        {
            new Medecin
            {
                FirstName = "Jean",
                LastName = "Leblanc",
                Specialite = "Médecin généraliste",
                Email = "jean.leblanc@cabinet.fr",
                Phone = "01 23 45 67 89"
            },
            new Medecin
            {
                FirstName = "Claire",
                LastName = "Rousseau",
                Specialite = "Cardiologue",
                Email = "claire.rousseau@cabinet.fr",
                Phone = "01 23 45 67 90"
            },
            new Medecin
            {
                FirstName = "Thomas",
                LastName = "Moreau",
                Specialite = "Pédiatre",
                Email = "thomas.moreau@cabinet.fr",
                Phone = "01 23 45 67 91"
            }
        };

        foreach (var medecin in medecins)
            await db.AddMedecinAsync(medecin);

        // Recharger pour avoir les IDs
        var patientsWithIds = await db.GetPatientsAsync();
        var medecinsWithIds = await db.GetMedecinsAsync();

        // Ajouter des rendez-vous
        var today = DateTime.Today;
        var appointments = new List<Appointment>
        {
            new Appointment
            {
                PatientId = patientsWithIds[2].Id, // Sophie Bernard
                MedecinId = medecinsWithIds[1].Id, // Dr. Claire Rousseau
                DateTime = today.AddHours(14),
                Type = AppointmentType.Urgence,
                Status = AppointmentStatus.Urgent,
                Notes = "Douleurs thoraciques"
            },
            new Appointment
            {
                PatientId = patientsWithIds[1].Id, // Pierre Martin
                MedecinId = medecinsWithIds[0].Id, // Dr. Jean Leblanc
                DateTime = today.AddHours(10).AddMinutes(30),
                Type = AppointmentType.Controle,
                Status = AppointmentStatus.Normal,
                Notes = "Contrôle glycémie"
            },
            new Appointment
            {
                PatientId = patientsWithIds[0].Id, // Marie Dupont
                MedecinId = medecinsWithIds[0].Id, // Dr. Jean Leblanc
                DateTime = today.AddHours(9),
                Type = AppointmentType.Consultation,
                Status = AppointmentStatus.Normal,
                Notes = "Consultation de suivi"
            }
        };

        foreach (var appointment in appointments)
            await db.AddAppointmentAsync(appointment);
    }
}
