using Gestion_RDV.Services;
using Gestion_RDV.ViewModels.Dashboard;
using Gestion_RDV.ViewModels.Patients;
using Gestion_RDV.ViewModels.Medecins;
using Gestion_RDV.ViewModels.RendezVous;
using Gestion_RDV.ViewModels.Authentication;
using Gestion_RDV.Views.Dashboard;
using Gestion_RDV.Views.Patients;
using Gestion_RDV.Views.Medecins;
using Gestion_RDV.Views.RendezVous;
using Gestion_RDV.Views.Authentication;

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
        builder.Services.AddSingleton<AuthenticationService>();

        // ViewModels
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<PatientListViewModel>();
        builder.Services.AddTransient<PatientFormViewModel>();
        builder.Services.AddTransient<MedecinListViewModel>();
        builder.Services.AddTransient<MedecinFormViewModel>();
        builder.Services.AddTransient<AppointmentFormViewModel>();
        builder.Services.AddTransient<AppointmentListViewModel>();
        builder.Services.AddTransient<LoginViewModel>();

        // Views
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<PatientListPage>();
        builder.Services.AddTransient<PatientFormPage>();
        builder.Services.AddTransient<MedecinListPage>();
        builder.Services.AddTransient<MedecinFormPage>();
        builder.Services.AddTransient<AppointmentFormPage>();
        builder.Services.AddTransient<AppointmentListPage>();
        builder.Services.AddTransient<LoginPage>();

        return builder.Build();
    }
}
