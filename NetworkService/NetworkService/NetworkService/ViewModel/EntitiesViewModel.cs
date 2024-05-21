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
		private bool _rtdChecked;
		private bool _termoChecked;
		private ObservableCollection<Entity> _entities;
		private string _selectedEntity;
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
		public string IdText { get { return _idText; } set { _idText = value; OnPropertyChanged(nameof(IdText)); } }
		public string NameText { get { return _nameText; } set { _nameText = value; OnPropertyChanged(nameof(NameText)); } }
		public string SelectedEntity { get { return _selectedEntity; } set { _selectedEntity = value; OnPropertyChanged(nameof(SelectedEntity)); } }

		public MyICommand AddEntityCommand { get; set; }
		private Entity currentEntity = new Entity();
		public MyICommand ShowEntityCommand { get; set; }
		public ObservableCollection<Entity> _Entities
		{
			get { return _entities; }
			set { _entities = value; OnPropertyChanged(nameof(_Entities)); }
		}
		public EntitiesViewModel()
		{
			AddEntityCommand = new MyICommand(OnAdd);
			_Entities = MainWindowViewModel.Entities;
		}

		public Entity CurrentEntity
		{
			get { return currentEntity; }
			set
			{
				currentEntity = value;
				OnPropertyChanged("CurrentEntity");
			}
		}

		public void OnAdd()
		{
			if (RtdChecked)
			{
				MainWindowViewModel.Entities.Add(new Entity(int.Parse(_idText), _nameText, Model.Type.RTD));
			}
			else
			{
				MainWindowViewModel.Entities.Add(new Entity(int.Parse(_idText), _nameText, Model.Type.TermoSprega));
			}
		}
	}
}
