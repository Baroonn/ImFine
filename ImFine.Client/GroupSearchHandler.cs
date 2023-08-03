using ImFine.Client.Models;

namespace ImFine.Client
{
    public class GroupSearchHandler : SearchHandler
    {
        public IList<Group> Groups { get; set; }
        public Type SelectedItemNavigationTarget { get; set; }

        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);
            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                ItemsSource = Groups
                    .Where(g => g.name.ToLower().Contains(newValue.ToLower()))
                    .ToList<Group>();
            }
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);
        }
    }
}
