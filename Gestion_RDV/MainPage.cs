using Gestion_RDV.Services;
using Gestion_RDV.Models;

namespace Gestion_RDV
{
    public partial class MainPage : ContentPage
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public MainPage(IAppointmentRepository appointmentRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
        }

        private async void OnAjouterClicked(object sender, EventArgs e)
        {
            var rdv = new RendezVous
            {
                PatientId = 1,
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(1),
                Type = "Consultation",
                Status = AppointmentStatus.Normal,
                Notes = "Rendez-vous médical"
            };

            await _appointmentRepository.AddAsync(rdv);
            
            // Récupérer la liste
            var liste = await _appointmentRepository.GetAllAsync();
        }
    }
}
