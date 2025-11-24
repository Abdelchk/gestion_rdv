using Microsoft.Extensions.Logging;
using Gestion_RDV.Services;
using Gestion_RDV.ViewModels.Patients;
using Gestion_RDV.ViewModels.RendezVous;
using Gestion_RDV.Views.Patients;
using Gestion_RDV.Views.RendezVous;

namespace Gestion_RDV
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Enregistrer les services
            builder.Services.AddSingleton<IDatabaseService, SqliteDatabaseService>();
            builder.Services.AddSingleton<IPatientRepository, PatientRepository>();
            builder.Services.AddSingleton<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddSingleton<IAppointmentService, AppointmentService>();
            // LIGNE 31 SUPPRIMÉE : builder.Services.AddSingleton<DatabaseService>();

            // Enregistrer les ViewModels
            builder.Services.AddTransient<PatientListViewModel>();
            builder.Services.AddTransient<AppointmentListViewModel>();

            // Enregistrer les Pages
            builder.Services.AddTransient<PatientListPage>();
            builder.Services.AddTransient<AppointmentListPage>();
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}