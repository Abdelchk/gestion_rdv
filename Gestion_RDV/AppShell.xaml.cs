using Gestion_RDV.Views.Patients;
using Gestion_RDV.Views.Patients;
using Gestion_RDV.Views.Medecins;
using Gestion_RDV.Views.RendezVous;

namespace Gestion_RDV
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("PatientFormPage", typeof(PatientFormPage));
            Routing.RegisterRoute("MedecinListPage", typeof(MedecinListPage));
            Routing.RegisterRoute("MedecinFormPage", typeof(MedecinFormPage));
            Routing.RegisterRoute("AppointmentFormPage", typeof(AppointmentFormPage));
            Routing.RegisterRoute("AppointmentListPage", typeof(AppointmentListPage));
        }
    }
}