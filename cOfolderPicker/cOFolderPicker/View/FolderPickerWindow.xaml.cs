using System;
using System.Windows;
using System.Windows.Input;
using cOFolderPicker.Helper;
using cOFolderPicker.Model;
using cOFolderPicker.ViewModel;

namespace cOFolderPicker.View
{
	public interface IFolderPickerWindow
	{
		bool? ShowDialog();
		IFolder GetSelectedFolder();
	}

	/// <summary>
	///     Interaction logic for FolderPickerWindow.xaml
	/// </summary>
	internal partial class FolderPickerWindow : IFolderPickerWindow
	{
		public FolderPickerWindow()
		{
			Action<object> closeAction = delegate
				{
					_historyDb.Add(new Entry {EntryName = FolderTreeViewModel.SelectedItem.FullName});
					DialogResult = true;
					Close();
				};
			_closeWindowCommand = new RelayCommand(closeAction);
			InitializeComponent();
		}

		public FolderPickerWindow(IFolder rootFolder, string dialogTitle, string folderToSelect,
		                          IDatabase historyDb, IDatabase favoritesDb)
			: this()
		{
			if (String.IsNullOrEmpty(dialogTitle))
				dialogTitle = "Please select a folder:";
			Title = dialogTitle;
			_historyDb = historyDb ?? new RegistryDatabaseHistory(rootFolder.GetType().Name);
			favoritesDb = favoritesDb ?? new RegistryDatabaseFavorites(String.Format("{0}_Favorite", rootFolder.GetType().Name));
			var folderTree = new FolderTreeViewModel(rootFolder, _historyDb, favoritesDb);
			FolderTreeViewModel.UserInteractionOn = true;
			FolderTreeHelper.FindFolder(folderToSelect, folderTree); //Search for folder to select
			DataContext = folderTree; // Let the UI bind to the view-model.
		}

		private readonly IDatabase _historyDb;
		private readonly ICommand _closeWindowCommand;

		public ICommand CloseWindowCommand
		{
			get { return _closeWindowCommand; }
		}

		public IFolder GetSelectedFolder()
		{
			return FolderTreeViewModel.SelectedItem;
		}
	}
}