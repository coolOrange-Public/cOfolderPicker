using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using cOFolderPicker.Helper;
using cOFolderPicker.Model;
using cOFolderPicker.Properties;

namespace cOFolderPicker.ViewModel
{
	internal class FolderTreeViewModel : ViewModelBase
	{
		#region Data

		private static readonly ImageSource IconLibraryFolder = Resources.libraryFolder.ToImageSource();
		private static readonly ImageSource IconRootFolder = Resources.rootFolder.ToImageSource();
		private static readonly ImageSource IconFolder = Resources.vaultFolder.ToImageSource();
		private static readonly ImageSource Star = Resources.Star.ToImageSource();
		private static readonly ImageSource StarEmpty = Resources.StarEmpty.ToImageSource();
		private static readonly FolderTreeViewModel DummyChild = new FolderTreeViewModel();
		private static MTObservableCollection<FolderTreeViewModel> _folderCollection;
		private static MTObservableCollection<string> _favoriteList;
		private static string _selectedFavoriteItem;
		private static IDatabase _databaseHistory;
		private static IDatabase _databaseFavorites;

		private MTObservableCollection<FolderTreeViewModel> _children;
		private ImageSource _icon;
		private string _historyItem;
		private bool _isExpanded;
		private bool _isSelected;
		private bool _isChecked;
		private Visibility _progressbarVisibility = Visibility.Hidden;
		private Thread _currentThread;
		private static FolderTreeViewModel _rootFolder;

		internal static bool UserInteractionOn = false;
		internal IFolder Folder;

		#endregion

		#region Constructors

		internal FolderTreeViewModel(IFolder folder, IDatabase historyDb, IDatabase favoritesDb)
		{
			_databaseHistory = historyDb;
			_databaseFavorites = favoritesDb;
			if (_databaseFavorites != null)
				FavoriteList = _databaseFavorites.Get().ToObservableCollection();

			Initialize(folder);
			_rootFolder = this;
			_folderCollection = new MTObservableCollection<FolderTreeViewModel> { _rootFolder };
			_icon = IconRootFolder;
			IsSelected = true;
		}

		internal FolderTreeViewModel(IFolder folder)
		{
			Initialize(folder);
		}

		private void Initialize(IFolder folder)
		{
			Folder = folder;
			_children = new MTObservableCollection<FolderTreeViewModel>();

			if (Folder.HasClds)
				_children.Add(DummyChild);
			if (FavoriteList != null && FavoriteList.Contains(Folder.FullName))
				_isChecked = true;
			_icon = Folder.IsLib ? IconLibraryFolder : IconFolder;
		}

		private FolderTreeViewModel()
		{
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return Folder.Name; }
		}

		public string FullName
		{
			get { return Folder.FullName; }
		}

		public bool HasDummyChild
		{
			get { return Children.Any() && Children[0] == DummyChild; }
		}

		public ImageSource Icon
		{
			get
			{
				_icon.Freeze();
				return _icon;
			}
		}

		public ImageSource StarIcon
		{
			get
			{
				Star.Freeze();
				return Star;
			}
		}

		public ImageSource StarEmptyIcon
		{
			get
			{
				StarEmpty.Freeze();
				return StarEmpty;
			}
		}

		public List<string> HistoryList
		{
			get { return _databaseHistory.Get().Select(entry => entry.EntryName).ToList(); }
		}

		public static IFolder SelectedItem { get; private set; }

		public MTObservableCollection<FolderTreeViewModel> RootFolder
		{
			get { return _folderCollection; }
			set
			{
				_folderCollection = value;
				OnPropertyChanged("RootFolder");
			}
		}

		public MTObservableCollection<FolderTreeViewModel> Children
		{
			get { return _children; }
			set
			{
				_children = value;
				OnPropertyChanged("Children");
			}
		}

		public MTObservableCollection<string> FavoriteList
		{
			get { return _favoriteList; }
			set
			{
				_favoriteList = value;
				OnPropertyChanged("FavoriteList");
			}
		}

		public string SelectedHistoryItem
		{
			get { return FullName; }
			set
			{
				_historyItem = value;
				OnPropertyChanged("SelectedHistoryItem");

				_rootFolder.IsExpanded = false;
				if (UserInteractionOn)
					FolderTreeHelper.FindFolder(_historyItem, _rootFolder);
			}
		}

		public string SelectedFavoriteItem
		{
			get { return _selectedFavoriteItem; }
			set
			{
				_selectedFavoriteItem = value;
				OnPropertyChanged("SelectedFavoriteItem");
			}
		}

		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");

				IsSelected = true;
				if (_databaseFavorites != null)
				{
					if (_isChecked)
						_databaseFavorites.Add(new Entry { EntryName = SelectedItem.FullName });
					else
						_databaseFavorites.Remove(new Entry { EntryName = SelectedItem.FullName });
					FavoriteList = _databaseFavorites.Get().ToObservableCollection();
				}
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged("IsSelected");
				//Save selected Folder
				if (_isSelected)
					SelectedItem = Folder;
			}
		}

		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				_isExpanded = value;
				OnPropertyChanged("IsExpanded");
				if (_isExpanded)
				{
					_children.Remove(DummyChild); //Remove DummyFile
					IsSelected = true;
					if (UserInteractionOn)
						DoBackgroundOperation();
				}
				else
				{
					_children.Clear(); //Clear list
					if (Folder.HasClds)
						_children.Add(DummyChild);
					if (UserInteractionOn)
						StopBackgroundOperation();
				}
			}
		}

		public ICommand GoToFolderCommand
		{
			get
			{
				Action<object> goToFolderAction = delegate
				{
					if (_selectedFavoriteItem != null)
						_rootFolder.IsExpanded = false;
					FolderTreeHelper.FindFolder(_selectedFavoriteItem, _rootFolder);
				};
				return new RelayCommand(goToFolderAction);
			}
		}


		public ICommand RemoveSelectedFavoriteCommand
		{
			get
			{
				Action<object> removeAction = delegate
				{
					if (_selectedFavoriteItem != null)
					{
						if (_rootFolder.FullName.Equals(_selectedFavoriteItem))
							_rootFolder.IsChecked = false;

						_databaseFavorites.Remove(new Entry { EntryName = _selectedFavoriteItem });
						FavoriteList = _databaseFavorites.Get().ToObservableCollection();

						_rootFolder.IsExpanded = false;
					}
				};
				return new RelayCommand(removeAction);
			}
		}

		public Visibility ProgressbarVisibility
		{
			get { return _progressbarVisibility; }
			set
			{
				_progressbarVisibility = value;
				OnPropertyChanged("ProgressbarVisibility");
			}
		}

		#endregion

		private void DoBackgroundOperation()
		{
			var dispatcher = Dispatcher.CurrentDispatcher;
			ThreadStart start = () => BackgroundStart(dispatcher);
			_currentThread = new Thread(start) { IsBackground = true };
			_currentThread.Start();
		}

		private void BackgroundStart(Dispatcher dispatcher)
		{
			ProgressbarVisibility = Visibility.Visible;
			foreach (IFolder folder in Folder.Children)
			{
				IFolder currentFolder = folder;
				Action del = () => Children.Add(new FolderTreeViewModel(currentFolder));
				dispatcher.Invoke(DispatcherPriority.Background, del);
			}
			ProgressbarVisibility = Visibility.Hidden;
		}

		private void StopBackgroundOperation()
		{
			if (_currentThread == null || !_currentThread.IsAlive)
				return;
			ProgressbarVisibility = Visibility.Hidden;
			_currentThread.Abort();
		}
	}
}