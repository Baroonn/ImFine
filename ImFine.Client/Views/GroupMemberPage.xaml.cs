using ImFine.Client.ViewModels;

namespace ImFine.Client.Views;

public partial class GroupMemberPage : ContentPage
{
	public GroupMemberPage()
	{
		InitializeComponent();
		BindingContext = new GroupMemberViewModel();
	}
}