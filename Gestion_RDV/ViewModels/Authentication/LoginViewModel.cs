using System.Windows.Input;
using Gestion_RDV.Models;
using Gestion_RDV.Services;

namespace Gestion_RDV.ViewModels.Authentication;

public class LoginViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly AuthenticationService _authService;

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
            UpdatePasswordStrength();
        }
    }

    private string _passwordStrengthMessage = string.Empty;
    public string PasswordStrengthMessage
    {
        get => _passwordStrengthMessage;
        set
        {
            _passwordStrengthMessage = value;
            OnPropertyChanged();
        }
    }

    private string _passwordStrengthColor = "#94A3B8";
    public string PasswordStrengthColor
    {
        get => _passwordStrengthColor;
        set
        {
            _passwordStrengthColor = value;
            OnPropertyChanged();
        }
    }

    private bool _isRegistering = false;
    public bool IsRegistering
    {
        get => _isRegistering;
        set
        {
            _isRegistering = value;
            OnPropertyChanged();
        }
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }

    public LoginViewModel() : this(null!, null!)
    {
    }

    public LoginViewModel(DatabaseService databaseService, AuthenticationService authService)
    {
        _databaseService = databaseService;
        _authService = authService;

        LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
        RegisterCommand = new Command(async () => await RegisterAsync(), () => !IsBusy);
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "⚠️ Veuillez remplir tous les champs";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var user = await _databaseService.GetUserByEmailAsync(Email.Trim().ToLower());

            if (user == null)
            {
                ErrorMessage = "❌ Email ou mot de passe incorrect";
                return;
            }

            if (!_authService.VerifyPassword(Password, user.PasswordHash))
            {
                ErrorMessage = "❌ Email ou mot de passe incorrect";
                return;
            }

            // Mettre à jour la dernière connexion
            user.LastLoginAt = DateTime.Now;
            await _databaseService.UpdateUserAsync(user);

            // Sauvegarder la session
            _authService.SaveCurrentUser(user.Id);

            // Rediriger vers le Dashboard
            Application.Current.MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"❌ Erreur: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "⚠️ Veuillez remplir tous les champs";
            return;
        }

        // Valider la force du mot de passe
        var strength = _authService.ValidatePasswordStrength(Password);
        if (strength == PasswordStrength.TooShort)
        {
            ErrorMessage = "⚠️ Le mot de passe doit contenir au moins 8 caractères";
            return;
        }

        if (strength == PasswordStrength.Weak)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "⚠️ Mot de passe faible",
                "Votre mot de passe est faible. Il est recommandé d'utiliser :\n" +
                "• Au moins 8 caractères\n" +
                "• Des majuscules et minuscules\n" +
                "• Des chiffres\n" +
                "• Des caractères spéciaux (!@#$%)\n\n" +
                "Voulez-vous continuer quand même ?",
                "Continuer",
                "Annuler");

            if (!confirm)
                return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            // Vérifier si l'email existe déjà
            var existingUser = await _databaseService.GetUserByEmailAsync(Email.Trim().ToLower());
            if (existingUser != null)
            {
                ErrorMessage = "⚠️ Cet email est déjà utilisé";
                return;
            }

            // Créer le nouvel utilisateur
            var newUser = new User
            {
                Email = Email.Trim().ToLower(),
                PasswordHash = _authService.HashPassword(Password),
                FullName = Email.Split('@')[0], // Utiliser la partie avant @ comme nom
                CreatedAt = DateTime.Now
            };

            await _databaseService.AddUserAsync(newUser);

            // Connexion automatique après inscription
            _authService.SaveCurrentUser(newUser.Id);

            await Application.Current.MainPage.DisplayAlert(
                "✅ Succès",
                "Votre compte a été créé avec succès !",
                "OK");

            // Rediriger vers le Dashboard
            Application.Current.MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"❌ Erreur: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdatePasswordStrength()
    {
        if (string.IsNullOrWhiteSpace(Password))
        {
            PasswordStrengthMessage = string.Empty;
            PasswordStrengthColor = "#94A3B8";
            return;
        }

        var strength = _authService.ValidatePasswordStrength(Password);
        PasswordStrengthMessage = _authService.GetPasswordStrengthMessage(strength);
        PasswordStrengthColor = _authService.GetPasswordStrengthColor(strength);
    }
}
