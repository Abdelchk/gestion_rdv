using Gestion_RDV.ViewModels.Patients;

namespace Gestion_RDV.Views.Patients
{
    public partial class PatientListPage : ContentPage
    {
        private readonly PatientListViewModel _vm;

        public PatientListPage(PatientListViewModel vm)
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
}