using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleChat.Avalonia;

public sealed partial class MainViewModel : ViewModel, IPresentationManager
{
	[ObservableProperty] public partial object? DataContext { get; set; }
}