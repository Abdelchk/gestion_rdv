using Gestion_RDV.ViewModels.Authentication;

namespace Gestion_RDV.Views.Authentication;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
