using Gestion_RDV.ViewModels.Patients;

namespace Gestion_RDV.Views.Patients;

public partial class PatientFormPage : ContentPage
{
    public PatientFormPage(PatientFormViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
