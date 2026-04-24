using Gestion_RDV.Services;
using Gestion_RDV.Helpers;
using Gestion_RDV.Views.Authentication;

namespace Gestion_RDV
{
    public partial class App : Application
    {
        public static DatabaseService Database { get; private set; }
        private readonly AuthenticationService _authService;

        public App(DatabaseService databaseService, AuthenticationService authService)
        {
            InitializeComponent();

            Database = databaseService;
            _authService = authService;

            // Initialiser la base de données et ajouter des données de test
            Task.Run(async () =>
            {
                await Database.GetPatientsAsync(); // Initialise les tables
                await DataSeeder.SeedDataAsync(Database); // Ajoute des données d'exemple
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Vérifier si un utilisateur est déjà connecté
            if (_authService.IsUserLoggedIn())
            {
                return new Window(new AppShell());
            }
            else
            {
                return new Window(new NavigationPage(new LoginPage(
                    Handler.MauiContext.Services.GetService<ViewModels.Authentication.LoginViewModel>())));
            }
        }
    }
}