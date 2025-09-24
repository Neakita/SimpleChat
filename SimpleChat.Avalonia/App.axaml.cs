using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AuthenticationViewModel = SimpleChat.Avalonia.Authentication.AuthenticationViewModel;

namespace SimpleChat.Avalonia;

public sealed partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			// Line below is needed to remove Avalonia data validation.
			// Without this line you will get duplicate validations from both Avalonia and CT
			BindingPlugins.DataValidators.RemoveAt(0);
			var mainViewModel = new MainViewModel();
			const string serverUri = "https://localhost:7210";
			var apiClient = new APIClient(serverUri);
			var hubsClient = new HubsClient(serverUri, apiClient);
			mainViewModel.DataContext = new AuthenticationViewModel(apiClient, hubsClient, mainViewModel);
			desktop.MainWindow = new MainWindow
			{
				DataContext = mainViewModel
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
}