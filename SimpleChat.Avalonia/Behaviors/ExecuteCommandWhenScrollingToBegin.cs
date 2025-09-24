using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace SimpleChat.Avalonia.Behaviors;

public sealed class ExecuteCommandWhenScrollingToBegin : Behavior<ScrollViewer>
{
	public static readonly StyledProperty<ICommand?> CommandProperty =
		AvaloniaProperty.Register<ExecuteCommandWhenScrollingToBegin, ICommand?>(nameof(Command));

	public static readonly StyledProperty<double> ThresholdProperty =
		AvaloniaProperty.Register<ExecuteCommandWhenScrollingToBegin, double>(nameof(Threshold));

	public ICommand? Command
	{
		get => GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	public double Threshold
	{
		get => GetValue(ThresholdProperty);
		set => SetValue(ThresholdProperty, value);
	}

	protected override void OnAttached()
	{
		if (AssociatedObject == null)
			throw new NullReferenceException("AssociatedObject is not set");
		AssociatedObject.ScrollChanged += OnAssociatedObjectScrollChanged;
		AssociatedObject.Loaded += OnAssociatedObjectLoaded;
	}

	protected override void OnDetaching()
	{
		if (AssociatedObject == null)
			throw new NullReferenceException("AssociatedObject is not set");
		AssociatedObject.ScrollChanged -= OnAssociatedObjectScrollChanged;
		AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == CommandProperty || change.Property == ThresholdProperty)
			ExecuteCommandIfNecessary();
		base.OnPropertyChanged(change);
	}

	private void OnAssociatedObjectLoaded(object? sender, RoutedEventArgs e)
	{
		ExecuteCommandIfNecessary();
	}

	private void OnAssociatedObjectScrollChanged(object? sender, ScrollChangedEventArgs e)
	{
		ExecuteCommandIfNecessary();
	}

	private void ExecuteCommandIfNecessary()
	{
		if (AssociatedObject == null)
			return;
		if (AssociatedObject.Offset.Y > Threshold)
			return;
		if (Command == null)
			return;
		if (!Command.CanExecute(null))
			return;
		Command.Execute(null);
	}
}