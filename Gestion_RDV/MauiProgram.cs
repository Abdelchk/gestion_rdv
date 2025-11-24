using Gestion_RDV.Services;
using Gestion_RDV.ViewModels.Dashboard;
using Gestion_RDV.ViewModels.Patients;
using Gestion_RDV.ViewModels.Medecins;
using Gestion_RDV.ViewModels.RendezVous;
using Gestion_RDV.Views.Dashboard;
using Gestion_RDV.Views.Patients;
using Gestion_RDV.Views.Medecins;
using Gestion_RDV.Views.RendezVous;

namespace Gestion_RDV;

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

        // Services
        builder.Services.AddSingleton<DatabaseService>();

        // ViewModels
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<PatientListViewModel>();
        builder.Services.AddTransient<PatientFormViewModel>();
        builder.Services.AddTransient<MedecinListViewModel>();
        builder.Services.AddTransient<MedecinFormViewModel>();
        builder.Services.AddTransient<AppointmentFormViewModel>();

        // Views
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<PatientListPage>();
        builder.Services.AddTransient<PatientFormPage>();
        builder.Services.AddTransient<MedecinListPage>();
        builder.Services.AddTransient<MedecinFormPage>();
        builder.Services.AddTransient<AppointmentFormPage>();
    
        return builder.Build();
    }
}
