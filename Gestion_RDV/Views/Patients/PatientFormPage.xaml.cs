using Gestion_RDV.ViewModels.Patients;

namespace Gestion_RDV.Views.Patients;

public partial class PatientFormPage : ContentPage
{
    private readonly PatientFormViewModel _vm;

    public PatientFormPage(PatientFormViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Charger les données du patient si c'est une édition
        await _vm.LoadAsync();
    }
}
