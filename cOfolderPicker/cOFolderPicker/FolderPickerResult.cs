using cOFolderPicker.Model;

namespace cOFolderPicker
{
	 public struct FolderPickerResult
	 {
		  public FolderDialogResult DialogResult { get; set; }
		  public IFolder SelectedFolder { get; set; }
	 }

	 public enum FolderDialogResult
	 {
		  OK,
		  Cancel
	 }
}