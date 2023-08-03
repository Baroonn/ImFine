using ImFine.Client.ViewModels;

namespace ImFine.Client.Views;

public partial class GroupOwnerPage : ContentPage
{
	private readonly GroupOwnerViewModel _viewModel;
	public GroupOwnerPage()
	{
		InitializeComponent();
		_viewModel = new GroupOwnerViewModel();
		BindingContext = _viewModel;
	}

	public async void OnToggled(object sender, ToggledEventArgs e)
	{
		//_viewModel.UpdateGroupState(e.Value ? "start" : "stop");
	}

}