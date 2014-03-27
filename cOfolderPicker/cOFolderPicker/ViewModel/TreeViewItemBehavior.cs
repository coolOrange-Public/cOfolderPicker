using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace cOFolderPicker.ViewModel
{
	 [ExcludeFromCodeCoverage]
	 internal static class TreeViewItemBehavior
	 {
		  public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
			  DependencyProperty.RegisterAttached(
				  "IsBroughtIntoViewWhenSelected",
				  typeof(bool),
				  typeof(TreeViewItemBehavior),
				  new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

		  public static bool GetIsBroughtIntoViewWhenSelected(TreeViewItem treeViewItem)
		  {
				return (bool)treeViewItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);
		  }

		  public static void SetIsBroughtIntoViewWhenSelected(
			  TreeViewItem treeViewItem, bool value)
		  {
				treeViewItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
		  }

		  private static void OnIsBroughtIntoViewWhenSelectedChanged(
			  DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		  {
				var item = depObj as TreeViewItem;
				if (item == null)
					 return;

				if (e.NewValue is bool == false)
					 return;

				if ((bool)e.NewValue)
					 item.Selected += OnTreeViewItemSelected;
				else
					 item.Selected -= OnTreeViewItemSelected;
		  }

		  private static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
		  {
				if (!ReferenceEquals(sender, e.OriginalSource))
					 return;

				var item = e.OriginalSource as TreeViewItem;
				if (item != null)
				{
					 item.BringIntoView();
					 item.Focus();
				}
		  }
	 }
}