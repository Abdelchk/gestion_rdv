using SQLite;

namespace Gestion_RDV.Models;

public class Medecin
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Specialite { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    [Ignore]
    public string FullName => $"Dr. {FirstName} {LastName}";
}
