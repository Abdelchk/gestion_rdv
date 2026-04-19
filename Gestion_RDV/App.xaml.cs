using Gestion_RDV.Services;
using Gestion_RDV.Helpers;

namespace Gestion_RDV
{
    public partial class App : Application
    {
        public static DatabaseService Database { get; private set; }

        public App(DatabaseService databaseService)
        {
            InitializeComponent();

            Database = databaseService;

            // Initialiser la base de données et ajouter des données de test
            Task.Run(async () =>
            {
                await Database.GetPatientsAsync(); // Initialise les tables
                await DataSeeder.SeedDataAsync(Database); // Ajoute des données d'exemple
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}