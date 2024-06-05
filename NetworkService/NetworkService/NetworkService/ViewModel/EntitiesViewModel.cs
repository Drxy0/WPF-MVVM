using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMLight.Messaging;
using System.Windows.Media;
using Notification.Wpf;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Security.Cryptography;
using System.ComponentModel;



namespace NetworkService.ViewModel
{
	public class EntitiesViewModel : BindableBase
	{
		private string _idText;
		private string _nameText;
		private string _idErrorTextBlock;
		private string _nameErrorTextBlock;
		private string _searchTextBoxErrorLabel;
		private string _idBorderBrush;
		private string _nameBorderBrush;
		private bool _isNameChecked;
		private bool _isTypeChecked;
		private bool _rtdChecked;
		private bool _termoChecked;
		private string _searchTextBox;
		private ObservableCollection<Entity> _entities;
		private ObservableCollection<Entity> filter = new ObservableCollection<Entity>();
		private Entity currentEntity;
		public string SearchTextBox
		{
			get { return _searchTextBox; }
			set { _searchTextBox = value; OnPropertyChanged(nameof(SearchTextBox)); }
		}
		public string SearchTextBoxErrorLabel
		{
			get { return _searchTextBoxErrorLabel; }
			set { _searchTextBoxErrorLabel = value; OnPropertyChanged(nameof(SearchTextBoxErrorLabel)); }
		}
		public bool IsNameChecked
		{
			get { return _isNameChecked; }
			set
			{
				if (_isNameChecked != value)
				{
					_isNameChecked = value;
					OnPropertyChanged(nameof(IsNameChecked));
				}
			}
		}
		public bool IsTypeChecked
		{
			get { return _isTypeChecked; }
			set
			{
				if (_isTypeChecked != value)
				{
					_isTypeChecked = value;
					OnPropertyChanged(nameof(IsTypeChecked));
				}
			}
		}

		public bool RtdChecked
		{
			get { return _rtdChecked; }
			set
			{
				if (_rtdChecked != value)
				{
					_rtdChecked = value;
					OnPropertyChanged(nameof(RtdChecked));
				}
			}
		}

		public bool TermoChecked
		{
			get { return _termoChecked; }
			set
			{
				if (_termoChecked != value)
				{
					_termoChecked = value;
					OnPropertyChanged(nameof(TermoChecked));
				}
			}
		}

		public string IdText
		{
			get { return _idText; }
			set { _idText = value; OnPropertyChanged(nameof(IdText)); }
		}

		public string IDErrorTextBlock
		{
			get { return _idErrorTextBlock; }
			set { _idErrorTextBlock = value; OnPropertyChanged(nameof(IDErrorTextBlock)); }
		}
		public string NameErrorTextBlock
		{
			get { return _nameErrorTextBlock; }
			set { _nameErrorTextBlock = value; OnPropertyChanged(nameof(NameErrorTextBlock)); }
		}

		public string IdBorderBrush
		{
			get { return _idBorderBrush; }
			set { _idBorderBrush = value; OnPropertyChanged(nameof(IdBorderBrush)); }
		}

		public string NameBorderBrush
		{
			get { return _nameBorderBrush; }
			set { _nameBorderBrush = value; OnPropertyChanged(nameof(NameBorderBrush)); }
		}

		public string NameText
		{
			get { return _nameText; }
			set { _nameText = value; OnPropertyChanged(nameof(NameText)); }
		}

		public MyICommand AddEntityCommand { get; set; }
		public MyICommand DeleteEntityCommand { get; set; }
		public MyICommand SearchEntitiesCommand { get; set; }
		public MyICommand ClearSearchCommand { get; set; }
		public MyICommand ShowEntityCommand { get; set; }

		public ObservableCollection<Entity> _Entities
		{
			get { return _entities; }
			set { _entities = value; OnPropertyChanged(nameof(_Entities)); }
		}

		public Entity CurrentEntity
		{
			get { return currentEntity; }
			set
			{
				currentEntity = value;
				OnPropertyChanged(nameof(CurrentEntity));
			}
		}

		public EntitiesViewModel()
		{
			AddEntityCommand = new MyICommand(OnAdd);
			DeleteEntityCommand = new MyICommand(OnDelete);
			SearchEntitiesCommand = new MyICommand(OnSearch);
			ClearSearchCommand = new MyICommand(OnClear);
			_Entities = MainWindowViewModel.Entities;
			InitialiseFields();
		}

		private void InitialiseFields()
		{
			RtdChecked = true;
			IsNameChecked = true;
			SearchTextBox = string.Empty;
			NameText = string.Empty;
			IDErrorTextBlock = string.Empty;
			IdBorderBrush = "Gray";
			NameBorderBrush = "Gray";
		}

		public void OnAdd()
		{
			bool onAddReturn = false;
			if (!int.TryParse(IdText, out _))
			{
				//Show Toast Notification for id not int;
				IdText = string.Empty;
				IDErrorTextBlock = "Error: Id must be an integer!";
				IdBorderBrush = "Red";
				Toast_OnAddError("Id must be an integer!");
				onAddReturn = true;
			}
			else
			{
				IDErrorTextBlock = string.Empty;
				IdBorderBrush = "Gray";
			}

			if (MainWindowViewModel.Entities.Any(e => e.Id == int.Parse(IdText)))
			{
				//Show Toast Notification for same id;
				IdText = string.Empty;
				IDErrorTextBlock = "Error: Id already exists!";
				IdBorderBrush = "Red";
				Toast_OnAddError("Id already exists!");
				onAddReturn = true;
			}
			else if (!onAddReturn)
			{
				IDErrorTextBlock = string.Empty;
				IdBorderBrush = "Gray";
			}

			if (NameText == string.Empty)
			{
				//Show Toast Notification for no name;
				NameErrorTextBlock = "Error: Name cannot be left empty!";
				NameBorderBrush = "Red";
				Toast_OnAddError("Name cannot be left empty!");
				onAddReturn = true;
			}

			if (onAddReturn) return;

			Entity entity = new Entity
			{
				Id = int.Parse(IdText),
				Name = NameText,
				Id_name_treeview = $"{IdText} - {NameText}"
			};

			if (RtdChecked) 
			{
				entity.Type = new EntityType(Model.Type.RTD);
			}
			else
			{
				entity.Type = new EntityType(Model.Type.TermoSprega);
			}

			SaveState();
			MainWindowViewModel.Entities.Add(entity);
			
			if (RtdChecked)
			{
				MainWindowViewModel.RTD_Entities.Add(entity);
			}
			else
			{
				MainWindowViewModel.TermoSprega_Entities.Add(entity);
			}
			ToastNotify.RaiseToast(
						"Success",
						$"Entity created: {entity.Id}",
						Notification.Wpf.NotificationType.Success);
			ResetFields();
		}

		private void Toast_OnAddError(string error_msg)
		{
			ToastNotify.RaiseToast(
			"Error",
			error_msg,
			Notification.Wpf.NotificationType.Error);
		}

		private void ResetFields()
		{
			IdText = string.Empty;
			NameText = string.Empty;
			RtdChecked = true;
			TermoChecked = false;
			IDErrorTextBlock = string.Empty;
			NameErrorTextBlock = string.Empty;
			IdBorderBrush = "Gray";
			NameBorderBrush = "Gray";
		}

		public void OnDelete()
		{
			MessageBoxResult result = MessageBox.Show(
				"Are you sure you want to delete the selected items?",
				"Confirm Deletion",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning
			);

			if (result == MessageBoxResult.No)
				return;

			SaveState();
			var itemsToDelete = _Entities.Where(e => e.IsSelected).ToList();
			foreach (var item in itemsToDelete)
			{
				MainWindowViewModel.Entities.Remove(item);
				try { filter.Remove(item); } catch { }
			}
			if (SearchTextBox.Equals(string.Empty))
			{
				_Entities = MainWindowViewModel.Entities;
			}
			else
			{
				_Entities = filter;
			}

			ToastNotify.RaiseToast(
			"Success",
			"Items sucessfully deleted",
			Notification.Wpf.NotificationType.Success);
		}
		public void OnSearch()
		{
			filter = new ObservableCollection<Entity>();
			if (SearchTextBox.Equals(string.Empty))
			{
				SearchTextBoxErrorLabel = "Error: Please fill the search box";
			}
			else
			{
				SearchTextBoxErrorLabel = string.Empty;
			}

			if (IsNameChecked)
			{
				foreach (Entity e in MainWindowViewModel.Entities)
				{
					if (e.Name.Contains(SearchTextBox))
						filter.Add(e);
				}
			}
			else
			{
				foreach (Entity e in MainWindowViewModel.Entities)
				{
					if (e.Type.ToString().Contains(SearchTextBox))
						filter.Add(e);
				}
			}
			_Entities = filter;
		}

		public void OnClear()
		{
			SearchTextBox = string.Empty;
			filter.Clear();
			_Entities = MainWindowViewModel.Entities;
		}

		public int GetCanvasIndexForEntityId(int entityId)
		{
			for (int i = 0; i < DisplayViewModel.CanvasCollection.Count; i++)
			{
				Entity entity = (DisplayViewModel.CanvasCollection[i].Resources["data"]) as Entity;

				if ((entity != null) && (entity.Id == entityId))
				{
					return i;
				}
			}
			return -1;
		}

		private void SaveState()
		{
			MainWindowViewModel.Undo = new SaveState<CommandType, object> (CommandType.EntityManipulation, new ObservableCollection<Entity>(MainWindowViewModel.Entities));
		}
	}
}

