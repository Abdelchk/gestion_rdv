using SQLite;

namespace Gestion_RDV.Models;

public class Appointment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int PatientId { get; set; }
    public int MedecinId { get; set; }

    public DateTime DateTime { get; set; }

    public AppointmentType Type { get; set; }
    public AppointmentStatus Status { get; set; }

    public string Notes { get; set; } = string.Empty;
}
