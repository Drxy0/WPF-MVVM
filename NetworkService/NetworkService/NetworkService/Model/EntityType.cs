using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetworkService.Model
{
	public enum Type
	{
		RTD,
		TermoSprega
	}
	public class EntityType : INotifyPropertyChanged
	{
		private Type type;

		public Uri PathToTypeImage { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		public Type Type
		{
			get { return type; }
			set
			{
				if (type != value)
				{
					type = value;
					OnPropertyChanged("Type");
				}
			}
		}

		public EntityType(Type type)
		{
			this.type = type;
			if (type == Type.RTD)
			{
				this.PathToTypeImage = new Uri("pack://application:,,,/NetworkService;component/Assets/RTD.png");
			}
			else
			{
				this.PathToTypeImage = new Uri("pack://application:,,,/NetworkService;component/Assets/TermoSprega.png");
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public override string ToString()
		{
			return Type.ToString();
		}

	}
}
