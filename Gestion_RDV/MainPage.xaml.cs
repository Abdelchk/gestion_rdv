namespace Gestion_RDV
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void OnShowDbPathClicked(object? sender, EventArgs e)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "gestion_rdv.db");

            await DisplayAlert(
                "Chemin de la Base de Données",
                $"Fichier SQLite :\n\n{dbPath}\n\n" +
                $"Dossier :\n{FileSystem.AppDataDirectory}",
                "Copier le chemin",
                "Fermer");

            // Copier dans le presse-papier (optionnel)
            await Clipboard.Default.SetTextAsync(dbPath);
        }
    }
}
