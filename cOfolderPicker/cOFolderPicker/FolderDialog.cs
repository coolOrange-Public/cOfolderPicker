using System.Threading;
using cOFolderPicker.Model;
using cOFolderPicker.View;

namespace cOFolderPicker
{
	 public class FolderDialog
	 {
		  /// <summary>
		  /// Shows a Dialog Window with the folder structure of the passed folder object
		  /// </summary>
		  /// <param name="rootFolder">The folder object from where the structure starts.</param>
		  /// <returns>
		  /// The DialogResult: Ok or Cancel.
		  /// The selected Folder object, if nothing gets selected the selectedFolder object returns null
		  /// </returns>
		  public static FolderPickerResult Show(IFolder rootFolder)
		  {
				return Show(rootFolder, null, null, null, null);
		  }

		  /// <summary>
		  /// Shows a Dialog Window with the folder structure of the passed folder object
		  /// </summary>
		  /// <param name="rootFolder">The folder object from where the structure starts.</param>
		  /// <param name="dialogTitle">The dialog title.</param>
		  /// <returns>
		  /// The DialogResult: Ok or Cancel.
		  /// The selected Folder object, if nothing gets selected the selectedFolder object returns null
		  /// </returns>
		  public static FolderPickerResult Show(IFolder rootFolder, string dialogTitle)
		  {
				return Show(rootFolder, dialogTitle, null, null, null);
		  }

		  /// <summary>
		  /// Shows a Dialog Window with the folder structure of the passed folder object
		  /// </summary>
		  /// <param name="rootFolder">The folder object from where the structure starts.</param>
		  /// <param name="dialogTitle">The dialog title.</param>
		  /// <param name="folderToSelect">The full folder path, where the folder selection should jump to at start.</param>
		  /// <returns>
		  /// The DialogResult: Ok or Cancel.
		  /// The selected Folder object, if nothing gets selected the selectedFolder object returns null
		  /// </returns>
		  public static FolderPickerResult Show(IFolder rootFolder, string dialogTitle, string folderToSelect)
		  {
				return Show(rootFolder, dialogTitle, folderToSelect, null, null);
		  }

		  /// <summary>
		  /// Shows a Dialog Window with the folder structure of the passed folder object
		  /// </summary>
		  /// <param name="rootFolder">The folder object from where the structure starts.</param>
		  /// <param name="historyDb">The database where the HistoryList should be saved</param>
		  /// <param name="favoritesDb">The database where the FavoritesList should be saved<</param>
		  /// <returns>
		  /// The DialogResult: Ok or Cancel. 
		  /// The selected Folder object, if nothing gets selected the selectedFolder object returns null  
		  /// </returns>
		  public static FolderPickerResult Show(IFolder rootFolder, IDatabase historyDb,
															 IDatabase favoritesDb)
		  {
				return Show(rootFolder, null, null, historyDb, favoritesDb);
		  }

		  /// <summary>
		  /// Shows a Dialog Window with the folder structure of the passed folder object
		  /// </summary>
		  /// <param name="rootFolder">The folder object from where the structure starts.</param>
		  /// <param name="dialogTitle">The dialog title.</param>
		  /// <param name="folderToSelect">The full folder path, where the folder selection should jump to at start.</param>
		  /// <param name="historyDb">The database where the HistoryList should be saved</param>
		  /// <param name="favoritesDb">The database where the FavoritesList should be saved<</param>
		  /// <returns>
		  /// The DialogResult: Ok or Cancel. 
		  /// The selected Folder object, if nothing gets selected the selectedFolder object returns null  
		  /// </returns>
		  public static FolderPickerResult Show(IFolder rootFolder, string dialogTitle, string folderToSelect,
															 IDatabase historyDb,
															 IDatabase favoritesDb)
		  {
				return Show(rootFolder, dialogTitle, folderToSelect, historyDb, favoritesDb, null);
		  }

		  internal static FolderPickerResult Show(IFolder rootFolder, string dialogTitle,
																		  string folderToSelect, IDatabase historyDb, IDatabase favoritesDb,
																		  IFolderPickerWindow folderpicker)
		  {
				if (rootFolder == null)
					 return new FolderPickerResult();
				var dialogResult = FolderDialogResult.Cancel;
				IFolder selectedFolderNode = null;
				var thread = new Thread(() =>
					{
						 if (folderpicker == null)
							  folderpicker = new FolderPickerWindow(rootFolder, dialogTitle, folderToSelect, historyDb, favoritesDb);
						 var wpfDialogResult = folderpicker.ShowDialog();
						 if (wpfDialogResult == true)
						 {
							  dialogResult = FolderDialogResult.OK;
							  selectedFolderNode = folderpicker.GetSelectedFolder();
						 }
					});
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
				thread.Join();

				return new FolderPickerResult { DialogResult = dialogResult, SelectedFolder = selectedFolderNode };
		  }
	 }
}