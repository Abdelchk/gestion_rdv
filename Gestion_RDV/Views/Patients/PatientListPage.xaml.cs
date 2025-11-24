using Gestion_RDV.ViewModels.Patients;
using Gestion_RDV.Models;

namespace Gestion_RDV.Views.Patients;

public partial class PatientListPage : ContentPage
{
    private readonly PatientListViewModel _vm;

    public PatientListPage(PatientListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private async void OnAddPatientClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PatientFormPage));
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _vm.FilterPatients(e.NewTextValue);
    }

    private async void OnDeletePatientClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton btn && btn.CommandParameter is Patient patient)
            await _vm.DeleteAsync(patient);
    }

    private async void OnEditPatientClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton btn && btn.CommandParameter is Patient patient)
        {
            await Shell.Current.GoToAsync($"{nameof(PatientFormPage)}?PatientId={patient.Id}");
        }
    }
}
