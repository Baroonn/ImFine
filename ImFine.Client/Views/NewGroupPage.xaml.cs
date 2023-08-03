using ImFine.Client.ViewModels;

namespace ImFine.Client.Views;

public partial class NewGroupPage : ContentPage
{
    public NewGroupPage()
    {
        InitializeComponent();
        BindingContext = new NewGroupViewModel();
    }
}