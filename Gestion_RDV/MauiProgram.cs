using Gestion_RDV.Services;
using Gestion_RDV.ViewModels.Dashboard;
using Gestion_RDV.ViewModels.Patients;
using Gestion_RDV.Views.Dashboard;
using Gestion_RDV.Views.Patients;

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
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<DashboardPage>();


        // ViewModels
        builder.Services.AddTransient<PatientListViewModel>();
        builder.Services.AddTransient<PatientFormViewModel>();

        // Views
        builder.Services.AddTransient<PatientListPage>();
        builder.Services.AddTransient<PatientFormPage>();

        return builder.Build();
    }
}
