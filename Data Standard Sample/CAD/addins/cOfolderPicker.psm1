

#cls
$loadedAssemblies = [AppDomain]::CurrentDomain.GetAssemblies()
$webserviceDLL = $loadedAssemblies | Where-Object {  $_.Location -ne $null -and $_.Location.ToLower().EndsWith("autodesk.connectivity.webservices.dll") }

$dotNetFramework = [System.Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()
$references = @()
$references += $dotNetFramework+"System.Collections.dll"
$references += $dotNetFramework+"System.Web.Services.dll"
$references += $dotNetFramework+"System.dll"
$references += $webserviceDLL.Location
$references += "C:\Program Files (x86)\Microsoft WSE\v3.0\Microsoft.Web.Services3.dll"
$references += "C:\ProgramData\Autodesk\Vault 2015\Extensions\DataStandard\CAD\addins\cOFolderPicker.dll"

Add-Type -Path $references
$cpar = New-Object System.CodeDom.Compiler.CompilerParameters
$cpar.ReferencedAssemblies.AddRange($references)

Add-Type @'
using System.Collections.Generic;
using Autodesk.Connectivity.WebServices;
using cOFolderPicker.Model;

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
        Id = vaultFolder.Id.ToString();
        IsLib = vaultFolder.IsLib;
    }

    public string Name { get; set; }
    public string FullName { get; set; }
    public string Id { get; set; }
    public bool IsLib { get; set; }

    public bool HasClds
    {
        get { return GetChildren().Count==0?false:true; }
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
        foreach (var folder in folders)
        {
            children.Add(new VaultFolder(_docSvc, folder));
        }
        return children;
    }
}
'@ -CompilerParameters $cpar 


function SelectVaultFolder
{
  $dsDiag.Trace(">> SelectVaultFolder")
  $workingPath = $Prop["_VaultVirtualPath"].Value + $Prop["_WorkspacePath"].Value
  $workingPath = $workingPath.Replace("\","/")
  $dsDiag.Trace(" Vault working folder: "+$workingPath)
  
  $folder=$vault.DocumentService.GetFolderByPath($workingPath)
  $dsDiag.Trace(" start folder: "+$folder.FullName)
  $iFolder = New-Object VaultFolder($vault.DocumentService,$folder)
  $currPath = $workingPath + "/" + $Prop['Folder'].Value
  $selectedFolder = [cOFolderPicker.FolderDialog]::Show($iFolder,"Select a folder!",$currPath)
  $dsDiag.Trace(" selected folder: "+$selectedFolder.SelectedFolder.FullName)
  if($selectedFolder.SelectedFolder -eq $null) { return }
  
  $path = $selectedFolder.SelectedFolder.FullName
  $path = $path.Replace($workingPath + "/","") #remove root folders
  $path = $path.Replace("/","\") #twist / with \ 
  
  $dsDiag.Trace(" path = "+$path)
  $Prop['Folder'].Value = $path
  $dsDiag.Trace("<< SelectVaultFolder")
}