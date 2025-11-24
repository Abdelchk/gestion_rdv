using SQLite;

namespace Gestion_RDV.Models;

public class Patient
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public DateTime DateNaissance { get; set; }

    public string Notes { get; set; } = string.Empty;

    // ------ PROPRIÉTÉS UTILITAIRES ------

    [Ignore]
    public string FullName => $"{FirstName} {LastName}";

    [Ignore]
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateNaissance.Year;
            if (DateNaissance > today.AddYears(-age))
                age--;
            return age;
        }
    }
}
