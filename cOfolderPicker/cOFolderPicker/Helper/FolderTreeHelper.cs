using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using cOFolderPicker.Model;
using cOFolderPicker.ViewModel;

namespace cOFolderPicker.Helper
{
	 [ExcludeFromCodeCoverage]
	 internal static class FolderTreeHelper
	 {
		  internal static bool FindFolder(string searchText, FolderTreeViewModel folderNode)
		  {
				if (String.IsNullOrEmpty(searchText) || folderNode == null)
					 return false;

				var dispatcher = Dispatcher.CurrentDispatcher;
				ThreadStart start = () => PerformSearch(dispatcher, searchText, folderNode);
				var thread = new Thread(start) { IsBackground = true };
				thread.Start();

				return true;
		  }

		  private static FolderTreeViewModel PerformSearch(Dispatcher dispatcher, string searchText, FolderTreeViewModel folderNode)
		  {
				var folderToSearch = searchText.TrimStart(folderNode.FullName);
				var folders = folderToSearch.Split(new[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string folderName in folders)
				{
					 if (folderNode.Folder.HasClds && folderNode.Folder.Children.Any(f => f.Name.Equals(folderName)))
					 {
						  FolderTreeViewModel.UserInteractionOn = false;
						  folderNode.IsExpanded = true;
						  folderNode = SearchFolderNode(dispatcher, folderNode, folderName);
						  FolderTreeViewModel.UserInteractionOn = true;
					 }
				}
				return folderNode;
		  }

		  internal static FolderTreeViewModel SearchFolderNode(Dispatcher dispatcher, FolderTreeViewModel folderNode, string folderName)
		  {
				FolderTreeViewModel foundFolderNode = folderNode;
				folderNode.ProgressbarVisibility = Visibility.Visible;
				foreach (IFolder folder in folderNode.Folder.Children)
				{
					 IFolder currentFolder = folder;
					 Action del = () =>
						  {
								var tempFolderNode = new FolderTreeViewModel(currentFolder);
								folderNode.Children.Add(tempFolderNode);
								if (tempFolderNode.Name.Equals(folderName))
								{
									 foundFolderNode = tempFolderNode;
									 foundFolderNode.IsSelected = true;
								}
						  };
					 dispatcher.Invoke(DispatcherPriority.Background, del);
				}
				folderNode.ProgressbarVisibility = Visibility.Hidden;
				return foundFolderNode;
		  }

		  public static MTObservableCollection<string> ToObservableCollection(this IEnumerable<IEntry> enumerable)
		  {
				var mtObsCollectoion = new MTObservableCollection<string>();
				foreach (var cur in enumerable)
					 mtObsCollectoion.Add(cur.EntryName);
				return mtObsCollectoion;
		  }
	 }
}