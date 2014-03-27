using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using cOFolderPicker.Model;
using cOFolderPicker.ViewModel;

namespace cOFolderPicker.Helper
{
	 internal static class RegistryListsHelper
	 {
		  public static List<string> CheckHistoryListCount(string historyRegistryValues)
		  {
				var registryValuesList = historyRegistryValues.ToList();
				if (registryValuesList.Count >= 10)
					 registryValuesList.RemoveAt(registryValuesList.Count - 1);
				return registryValuesList;
		  }

		  public static List<string> SetValueOnListStart(List<string> registryValuesList, string value)
		  {
				registryValuesList.Remove(value);
				registryValuesList.Insert(0, value);
				return registryValuesList;
		  }

		  public static string ToRegistryString(this List<string> registryValuesList)
		  {
				return String.Join(",", registryValuesList.ToArray());
		  }

		  public static List<string> ToList(this string registryListString)
		  {
				var registryValuesOnSplit = registryListString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				return registryValuesOnSplit.ToList();
		  }
	 }
}