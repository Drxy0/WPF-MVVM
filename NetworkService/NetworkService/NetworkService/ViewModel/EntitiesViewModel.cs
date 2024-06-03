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
			RtdChecked = true;
			IsNameChecked = true;
			SearchTextBox = string.Empty;
		}

		public void OnAdd()
		{
			if (!int.TryParse(_idText, out _))
			{
				//Show Toast Notification for id not int;
				IdText = string.Empty;
				return;
			}

			if (MainWindowViewModel.Entities.Any(e => e.Id == int.Parse(_idText)))
			{
				//Show Toast Notification for same id;
				IdText = string.Empty;
				return;
			}

			Entity entity = new Entity
			{
				Id = int.Parse(_idText),
				Name = _nameText,
			};

			if (RtdChecked) 
			{
				entity.Type = new EntityType(Model.Type.RTD);
			}
			else
			{
				entity.Type = new EntityType(Model.Type.TermoSprega);
			}

			MainWindowViewModel.Entities.Add(entity);
			
			if (RtdChecked)
			{
				MainWindowViewModel.RTD_Entities.Add(entity);
			}
			else
			{
				MainWindowViewModel.TermoSprega_Entities.Add(entity);
			}
			ResetFields();
		}

		private void ResetFields()
		{
			IdText = string.Empty;
			NameText = string.Empty;
			RtdChecked = true;
			TermoChecked = false;
		}

		public void OnDelete()
		{
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
		}
		public void OnSearch()
		{
			filter = new ObservableCollection<Entity>();
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

	}
}

