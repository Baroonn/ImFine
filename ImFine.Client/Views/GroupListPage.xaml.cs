using ImFine.Client.Models;
using ImFine.Client.ViewModels;

namespace ImFine.Client.Views;

public partial class GroupListPage : ContentPage
{
    private GroupListViewModel _viewModel;

    public GroupListPage()
    {
        InitializeComponent();
        _viewModel = new GroupListViewModel();
        BindingContext = _viewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        var group = args.SelectedItem as Group;
        await Shell.Current.GoToAsync($"{nameof(GroupMemberPage)}?{nameof(GroupMemberViewModel.GroupName)}={group.name}");
        //((ListView)sender).SelectedItem = null;
    }

    public void OnItemTapped(object sender, EventArgs args)
    {
        var group = sender as Group;
        Shell.Current.GoToAsync($"{nameof(GroupOwnerPage)}?{nameof(GroupOwnerViewModel.GroupName)}={group.name}");
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        //_viewModel.RefreshGroupsCommand.Execute(this);
        base.OnNavigatedTo(args);
    }
}