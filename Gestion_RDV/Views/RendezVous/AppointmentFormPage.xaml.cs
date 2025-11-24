using Gestion_RDV.ViewModels.RendezVous;

namespace Gestion_RDV.Views.RendezVous;

public partial class AppointmentFormPage : ContentPage
{
    private readonly AppointmentFormViewModel _vm;

    public AppointmentFormPage(AppointmentFormViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
        
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        if (string.IsNullOrWhiteSpace(_vm.AppointmentId))
        {
            TitleLabel.Text = "Nouveau Rendez-vous";
        }
        else
        {
            TitleLabel.Text = "Modifier Rendez-vous";
        }
    }
}
