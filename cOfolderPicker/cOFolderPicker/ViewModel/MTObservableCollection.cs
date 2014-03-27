using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Threading;

namespace cOFolderPicker.ViewModel
{
	 [ExcludeFromCodeCoverage]
	 internal class MTObservableCollection<T> : ObservableCollection<T>
	 {
		  public override event NotifyCollectionChangedEventHandler CollectionChanged;
		  protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		  {
				var eh = CollectionChanged;
				if (eh != null)
				{
					 Dispatcher dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
													  let dpo = nh.Target as DispatcherObject
													  where dpo != null
													  select dpo.Dispatcher).FirstOrDefault();

					 if (dispatcher != null && dispatcher.CheckAccess() == false)
					 {
						  dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
					 }
					 else
					 {
						  foreach (NotifyCollectionChangedEventHandler nh in eh.GetInvocationList())
								nh.Invoke(this, e);
					 }
				}
		  }
	 }

}