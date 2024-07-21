using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Owl.ViewModels;

public partial class TabControlMenuViewModel : ViewModelBase
{
	[ObservableProperty] private ObservableCollection<TabControlItem> _menuItems;
	
	public TabControlMenuViewModel()
	{
		
	}
	
	[RelayCommand]
	private void SelectItem(TabControlItem item)
	{
		Console.WriteLine("Selected item: " + item.Text);
		foreach (var menuItem in MenuItems)
		{
			menuItem.IsSelected = menuItem == item;
		}
	}
}

public class TabControlItem(string text)
{
	public string Text { get; set; } = text;
	public bool IsSelected { get; set; } = false;
}
