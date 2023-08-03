using ImFine.Client.ViewModels;

namespace ImFine.Client.Views;

public partial class GroupSearchPage : ContentPage
{
    public GroupSearchPage()
    {
        InitializeComponent();
        BindingContext = new GroupSearchViewModel();

    }
}