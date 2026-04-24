using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Gestion_RDV.Services;

public class AuthenticationService
{
    private const string CurrentUserKey = "CurrentUserId";
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 100000; // Nombre d'itérations PBKDF2

    /// <summary>
    /// Hash un mot de passe avec PBKDF2 (sécurisé)
    /// Format: iterations.salt.hash (en Base64)
    /// </summary>
    public string HashPassword(string password)
    {
        // Générer un salt aléatoire
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        // Générer le hash avec PBKDF2
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        var hash = pbkdf2.GetBytes(HashSize);

        // Combiner iterations + salt + hash
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Vérifie si un mot de passe correspond au hash
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            var parts = passwordHash.Split('.');
            if (parts.Length != 3)
                return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            // Générer le hash du mot de passe saisi avec le même salt
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256);

            var testHash = pbkdf2.GetBytes(HashSize);

            // Comparaison sécurisée pour éviter les timing attacks
            return CryptographicOperations.FixedTimeEquals(hash, testHash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Valide la force d'un mot de passe
    /// </summary>
    public PasswordStrength ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return PasswordStrength.Empty;

        if (password.Length < 8)
            return PasswordStrength.TooShort;

        int score = 0;

        // Longueur
        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;
        if (password.Length >= 16) score++;

        // Complexité
        if (Regex.IsMatch(password, @"[a-z]")) score++; // Minuscules
        if (Regex.IsMatch(password, @"[A-Z]")) score++; // Majuscules
        if (Regex.IsMatch(password, @"[0-9]")) score++; // Chiffres
        if (Regex.IsMatch(password, @"[^a-zA-Z0-9]")) score++; // Caractères spéciaux

        return score switch
        {
            >= 7 => PasswordStrength.VeryStrong,
            >= 5 => PasswordStrength.Strong,
            >= 3 => PasswordStrength.Medium,
            _ => PasswordStrength.Weak
        };
    }

    /// <summary>
    /// Obtient un message descriptif pour la force du mot de passe
    /// </summary>
    public string GetPasswordStrengthMessage(PasswordStrength strength)
    {
        return strength switch
        {
            PasswordStrength.Empty => "⚠️ Le mot de passe est requis",
            PasswordStrength.TooShort => "❌ Le mot de passe doit contenir au moins 8 caractères",
            PasswordStrength.Weak => "🔴 Faible - Ajoutez des majuscules, chiffres et caractères spéciaux",
            PasswordStrength.Medium => "🟡 Moyen - Ajoutez plus de complexité",
            PasswordStrength.Strong => "🟢 Fort - Bon mot de passe",
            PasswordStrength.VeryStrong => "✅ Très fort - Excellent mot de passe !",
            _ => string.Empty
        };
    }

    /// <summary>
    /// Obtient la couleur associée à la force du mot de passe
    /// </summary>
    public string GetPasswordStrengthColor(PasswordStrength strength)
    {
        return strength switch
        {
            PasswordStrength.Empty => "#94A3B8",
            PasswordStrength.TooShort => "#EF4444",
            PasswordStrength.Weak => "#EF4444",
            PasswordStrength.Medium => "#F59E0B",
            PasswordStrength.Strong => "#10B981",
            PasswordStrength.VeryStrong => "#059669",
            _ => "#94A3B8"
        };
    }

    /// <summary>
    /// Sauvegarde l'ID de l'utilisateur connecté
    /// </summary>
    public void SaveCurrentUser(int userId)
    {
        Preferences.Set(CurrentUserKey, userId);
    }

    /// <summary>
    /// Récupère l'ID de l'utilisateur connecté
    /// </summary>
    public int? GetCurrentUserId()
    {
        if (Preferences.ContainsKey(CurrentUserKey))
        {
            return Preferences.Get(CurrentUserKey, -1);
        }
        return null;
    }

    /// <summary>
    /// Déconnecte l'utilisateur
    /// </summary>
    public void Logout()
    {
        Preferences.Remove(CurrentUserKey);
    }

    /// <summary>
    /// Vérifie si un utilisateur est connecté
    /// </summary>
    public bool IsUserLoggedIn()
    {
        return GetCurrentUserId() != null;
    }
}

/// <summary>
/// Énumération de la force d'un mot de passe
/// </summary>
public enum PasswordStrength
{
    Empty,
    TooShort,
    Weak,
    Medium,
    Strong,
    VeryStrong
}
