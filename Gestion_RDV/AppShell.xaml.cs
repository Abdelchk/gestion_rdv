using Gestion_RDV.Views.Patients;

namespace Gestion_RDV
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("PatientFormPage", typeof(Gestion_RDV.Views.Patients.PatientFormPage));
            Routing.RegisterRoute(nameof(PatientFormPage), typeof(PatientFormPage));

        }
    }
}