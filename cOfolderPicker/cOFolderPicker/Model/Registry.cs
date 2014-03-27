using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

namespace cOFolderPicker.Model
{

	 /// <summary>
	 /// A class to read/write/delete/Add registry keys
	 /// </summary>
	 [ExcludeFromCodeCoverage]
	 internal class Registry : IRegistry
	 {
		  private string _subKey = @"Software\coolOrange\cOFolderPicker";

		  private readonly RegistryKey _baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser,
																								RegistryView.Registry32);

		  public Registry(string subKey)
		  {
				_subKey = subKey;
		  }

		  public Registry()
		  {
		  }

		  public string SubKey
		  {
				get { return _subKey; }
				set { _subKey = value; }
		  }

		  public RegistryKey BaseKey
		  {
				get { return _baseKey; }
		  }

		  public string ReadKey(string keyName)
		  {
				try
				{
					 using (RegistryKey openedSubKey = BaseKey.OpenSubKey(SubKey))
					 {
						  if (openedSubKey == null)
								return String.Empty;
						  return (string)openedSubKey.GetValue(keyName.ToUpper());
					 }
				}
				catch (Exception e)
				{
					 return String.Empty;
				}
		  }

		  public bool DeleteKey(string keyName)
		  {
				try
				{
					 using (RegistryKey openedSubKey = BaseKey.CreateSubKey(SubKey))
					 {
						  if (openedSubKey == null)
								return true;
						  openedSubKey.DeleteValue(keyName);
						  return true;
					 }
				}
				catch (Exception e)
				{
					 return false;
				}
		  }

		  public bool WriteKey(string keyName, object value)
		  {
				try
				{
					 using (RegistryKey openedSubKey = BaseKey.CreateSubKey(SubKey))
					 {
						  if (openedSubKey == null)
								return false;
						  openedSubKey.SetValue(keyName.ToUpper(), value.ToString().Trim());
						  return true;
					 }
				}
				catch (Exception e)
				{
					 return false;
				}
		  }
	 }
}