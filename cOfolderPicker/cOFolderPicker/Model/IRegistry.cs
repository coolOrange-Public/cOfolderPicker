using Microsoft.Win32;

namespace cOFolderPicker.Model
{
	 public interface IRegistry
	 {
		  string SubKey { get; set; }
		  RegistryKey BaseKey { get; }

		  string ReadKey(string keyName);
		  bool DeleteKey(string keyName);
		  bool WriteKey(string keyName, object value);
	 }
}