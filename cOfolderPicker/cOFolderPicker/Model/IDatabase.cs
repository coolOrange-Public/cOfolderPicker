using System.Collections.Generic;

namespace cOFolderPicker.Model
{
	 public interface IDatabase
	 {
		  string Name { get; set; }
		  IEnumerable<IEntry> Get();
		  bool Add(IEntry entry);
		  bool Remove(IEntry entry);
	 }

	 public interface IEntry
	 {
		  string EntryName { get; set; }
	 }
}