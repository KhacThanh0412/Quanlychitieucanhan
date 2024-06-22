using Quanlychitieu.Navigation;

namespace Quanlychitieu
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            NavGraph.RegisterRoute();
        }

        public async Task RemoveRootAsync()
        {
            List<Task> tasks = new();
            foreach (var item in this.CurrentItem.Items)
            {
                var viewModel = (item.CurrentItem as IShellContentController)?.Page?.BindingContext as BaseViewModel;

                if (viewModel != null)
                {
                    var task = viewModel?.ViewIsRemovedAsync();
                    if (task != null)
                        tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
