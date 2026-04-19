using Gestion_RDV.Models;
using Gestion_RDV.ViewModels.RendezVous;

namespace Gestion_RDV.Views.RendezVous;

public partial class AppointmentListPage : ContentPage
{
    private readonly AppointmentListViewModel _vm;

    public AppointmentListPage(AppointmentListViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private void OnMedecinFilterChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker && picker.SelectedItem is Medecin medecin)
        {
            _vm.SelectedMedecinId = medecin.Id;
        }
        else
        {
            _vm.SelectedMedecinId = null;
        }
    }
}
