using Gestion_RDV.ViewModels.Medecins;

namespace Gestion_RDV.Views.Medecins;

public partial class MedecinListPage : ContentPage
{
    private readonly MedecinListViewModel _vm;

    public MedecinListPage(MedecinListViewModel vm)
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
}