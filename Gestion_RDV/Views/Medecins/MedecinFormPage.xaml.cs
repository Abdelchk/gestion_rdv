using Gestion_RDV.ViewModels.Medecins;

namespace Gestion_RDV.Views.Medecins;

public partial class MedecinFormPage : ContentPage
{
    private readonly MedecinFormViewModel _vm;

    public MedecinFormPage(MedecinFormViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Charger les données du médecin si c'est une édition
        await _vm.LoadAsync();
        
        // Mettre à jour le titre en fonction du mode (création ou édition)
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        if (string.IsNullOrWhiteSpace(_vm.MedecinId))
        {
            TitleLabel.Text = "Nouveau Médecin";
        }
        else
        {
            TitleLabel.Text = "Modifier Médecin";
        }
    }
}
