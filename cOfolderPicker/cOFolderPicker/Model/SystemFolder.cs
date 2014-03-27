using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace cOFolderPicker.Model
{
	 [ExcludeFromCodeCoverage]
	 public class SystemFolder : IFolder
	 {
		  private readonly DirectoryInfo _directory;
		  /// <summary>
		  /// Initializes a new instance of the <see cref="SystemFolder"/> class.
		  /// This is an example Class to show the system folder structure.
		  /// </summary>
		  /// <param name="directory">The DirectoryInfo object, from where the structure should start from.</param>
		  public SystemFolder(DirectoryInfo directory)
		  {
				if (directory == null)
					 return;

				_directory = directory;
				Name = directory.Name;
				FullName = directory.FullName;
		  }

		  public string Name { get; set; }
		  public string FullName { get; set; }
		  public string Id { get; set; }
		  public bool IsLib { get; set; }

		  public bool HasClds
		  {
				get { return Children.Any(); }
		  }

		  public IList<IFolder> Children
		  {
				get { return GetChildren(); }
		  }

		  private IList<IFolder> GetChildren()
		  {
				try
				{
					 var subDirs = _directory.GetDirectories();
					 return subDirs.Select(subDir => new SystemFolder(subDir)).Cast<IFolder>().ToList();
				}
				catch (Exception)
				{
					 return new List<IFolder>();
				}
		  }
	 }
}