using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.Connectivity.WebServices;
using cOFolderPicker.Model;

namespace cOFolderPickerUtils.Vault2016
{
	 public class VaultFolder : IFolder
	 {
		  private static DocumentService _docSvc;
		  private readonly Folder _vaultFolder;

		  public VaultFolder(DocumentService docSvc, Folder vaultFolder)
		  {
				_docSvc = docSvc;
				_vaultFolder = vaultFolder;
				Name = vaultFolder.Name;
				FullName = vaultFolder.FullName;
				Id = vaultFolder.Id.ToString(CultureInfo.InvariantCulture);
				IsLib = vaultFolder.IsLib;
		  }

		  public string Name { get; set; }
		  public string FullName { get; set; }
		  public string Id { get; set; }
		  public bool IsLib { get; set; }

		  public bool HasClds
		  {
				get { return GetChildren().Any(); }
		  }

		  public IList<IFolder> Children
		  {
				get { return GetChildren(); }
		  }

		  private IList<IFolder> GetChildren()
		  {
				var children = new List<IFolder>();
				var folders = _docSvc.GetFoldersByParentId(_vaultFolder.Id, false);
				if (folders == null)
					 return children;
				children.AddRange(folders.Select(folder => new VaultFolder(_docSvc, folder)));
				return children;
		  }
	 }
}