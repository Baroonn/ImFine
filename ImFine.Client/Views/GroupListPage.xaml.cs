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

    //public async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    //{
    //    var group = args.SelectedItem as Group;
    //    await Shell.Current.GoToAsync($"{nameof(GroupMemberPage)}?{nameof(GroupMemberViewModel.GroupName)}={group.name}");
    //    //((ListView)sender).SelectedItem = null;
    //}

    //public async void OnItemTapped(object sender, EventArgs args)
    //{
    //    var group = sender as Group;
    //    await Shell.Current.GoToAsync($"{nameof(GroupOwnerPage)}?{nameof(GroupOwnerViewModel.GroupName)}={group.name}");
    //}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _viewModel.RefreshGroupsCommand.Execute(this);
    }

    public async void OnItemDeleted(object sender, EventArgs args)
    {
        MenuItem menuItem = sender as MenuItem;
        var contextItem = (Group)menuItem.BindingContext;
        _viewModel.DeleteGroupAsync(contextItem.name);
    }
}