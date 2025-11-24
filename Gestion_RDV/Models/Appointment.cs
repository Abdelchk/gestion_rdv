using SQLite;

namespace Gestion_RDV.Models;

public enum AppointmentType
{
    Consultation,
    Controle,
    Urgence
}

public enum AppointmentStatus
{
    Normal,
    Urgent,
    Cancelled
}

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
    
    [Ignore]
    public string TypeLabel => Type switch
    {
        AppointmentType.Consultation => "Consultation",
        AppointmentType.Controle => "Contrôle",
        AppointmentType.Urgence => "Urgence",
        _ => "Consultation"
    };
    
    [Ignore]
    public string StatusLabel => Status switch
    {
        AppointmentStatus.Normal => "Normal",
        AppointmentStatus.Urgent => "Urgent",
        AppointmentStatus.Cancelled => "Annulé",
        _ => "Normal"
    };
}
