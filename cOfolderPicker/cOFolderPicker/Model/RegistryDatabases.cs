using System;
using System.Collections.Generic;
using System.Linq;
using cOFolderPicker.Helper;

namespace cOFolderPicker.Model
{
	 internal class Entry : IEntry
	 {
		  public string EntryName { get; set; }
	 }

	 internal abstract class RegistryDatabase : IDatabase
	 {
		  protected IRegistry Registry;
		  public string Name { get; set; }

		  protected RegistryDatabase(string dbName, IRegistry registry)
		  {
				Name = dbName;
				Registry = registry ?? new Registry();
		  }

		  public IEnumerable<IEntry> Get()
		  {
				var historyList = new List<IEntry>();
				var historyRegistryValues = Registry.ReadKey(Name);
				if (historyRegistryValues == null)
					 return new List<Entry>();
				if (historyRegistryValues.Contains(","))
					 historyRegistryValues.ToList().ForEach(a => historyList.Add(new Entry { EntryName = a }));
				else
					 historyList.Add(new Entry { EntryName = historyRegistryValues });
				return historyList;
		  }

		  public abstract bool Add(IEntry entry);
		  public abstract bool Remove(IEntry entry);
	 }

	 internal class RegistryDatabaseHistory : RegistryDatabase
	 {
		  public RegistryDatabaseHistory(string dbName, IRegistry registry = null)
				: base(dbName, registry)
		  {
		  }

		  public override bool Add(IEntry entry)
		  {
				if (entry == null)
					 return false;
				var historyRegistryValues = Registry.ReadKey(Name);
				if (historyRegistryValues == null)
					 return Registry.WriteKey(Name, entry.EntryName);

				var registryValuesList = RegistryListsHelper.CheckHistoryListCount(historyRegistryValues);
				if (registryValuesList.Contains(entry.EntryName))
				{
					 var result = RegistryListsHelper.SetValueOnListStart(registryValuesList, entry.EntryName);
					 return Registry.WriteKey(Name, result.ToRegistryString());
				}
				registryValuesList.Insert(0, entry.EntryName);
				return Registry.WriteKey(Name, registryValuesList.ToRegistryString());
		  }

		  public override bool Remove(IEntry entry)
		  {
				throw new NotImplementedException();
		  }
	 }

	 internal class RegistryDatabaseFavorites : RegistryDatabase
	 {
		  public RegistryDatabaseFavorites(string dbName, IRegistry registry = null)
				: base(dbName, registry)
		  {
		  }

		  public override bool Add(IEntry entry)
		  {
				if (entry == null)
					 return false;
				var historyRegistryValues = Registry.ReadKey(Name);
				if (historyRegistryValues == null)
					 return Registry.WriteKey(Name, entry.EntryName);

				var registryValuesList = historyRegistryValues.ToList();
				if (registryValuesList.Contains(entry.EntryName))
					 return true;

				registryValuesList.Add(entry.EntryName);
				return Registry.WriteKey(Name, registryValuesList.ToRegistryString());
		  }

		  public override bool Remove(IEntry entry)
		  {
				if (entry == null)
					 return false;
				var favoriteList = Get().Select(a => a.EntryName).ToList();
				if (!favoriteList.Contains(entry.EntryName))
					 return false;

				favoriteList.Remove(entry.EntryName);
				Registry.WriteKey(Name, favoriteList.ToRegistryString());
				return true;
		  }
	 }
}
