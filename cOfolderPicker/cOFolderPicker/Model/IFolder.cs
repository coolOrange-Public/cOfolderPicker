using System.Collections.Generic;

namespace cOFolderPicker.Model
{
	 /// <summary>
	 /// This interfaces defines which functions your FolderClass has to implement to 
	 /// call the dialog Window.
	 /// </summary>
	 public interface IFolder
	 {
		  string Name { get; set; }
		  string FullName { get; set; }
		  string Id { get; set; }
		  bool IsLib { get; set; }
		  bool HasClds { get; }

		  IList<IFolder> Children { get; }
	 }
}