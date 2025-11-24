using System;

namespace Gestion_RDV.Models
{
    public enum AppointmentStatus
    {
        Normal,
        Urgent,
        Cancelled
    }

    public class RendezVous
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Type { get; set; } = string.Empty; // consultation, controle, urgence...
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Normal;
        public string Notes { get; set; } = string.Empty;
    }
}