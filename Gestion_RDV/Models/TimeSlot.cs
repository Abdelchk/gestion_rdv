namespace Gestion_RDV.Models;

/// <summary>
/// Représente un créneau horaire dans l'agenda
/// </summary>
public class TimeSlot
{
    public TimeSpan Time { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsPast { get; set; }
    public string DisplayTime => Time.ToString(@"hh\:mm");
    public string StatusIcon => IsPast ? "⏰" : (IsAvailable ? "✅" : "❌");

    public Color BackgroundColor
    {
        get
        {
            if (IsPast)
                return Color.FromArgb("#475569"); // Gris foncé pour le passé
            if (!IsAvailable)
                return Color.FromArgb("#EF4444"); // Rouge pour occupé
            return Color.FromArgb("#10B981"); // Vert pour disponible
        }
    }

    public Color TextColor => Colors.White;

    public bool IsEnabled => !IsPast && IsAvailable;
}
