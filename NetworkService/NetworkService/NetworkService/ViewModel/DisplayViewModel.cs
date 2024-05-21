using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
	public class DisplayViewModel : BindableBase
	{
		private ObservableCollection<Entity> _entities;
		public ObservableCollection<Entity> _Entities
		{
			get { return _entities; }
			set { _entities = value; OnPropertyChanged(nameof(_Entities)); }
		}

		public DisplayViewModel()
		{
			_Entities = MainWindowViewModel.Entities;
		}
	}
}
