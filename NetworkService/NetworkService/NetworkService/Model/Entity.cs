using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
	public class Entity : BindableBase
	{
		private int id;
		private string name;
		private EntityType type;
		private double _value;
		private bool _isSelected;
		List<Pair<DateTime, double>> _last_5_values;

		public int Id
		{
			get { return id; }
			set
			{
				if (id != value)
				{
					id = value;
					OnPropertyChanged("Id");
				}
			}
		}
		public string Name
		{
			get { return name; }
			set
			{
				if (name != value)
				{
					name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		public EntityType Type
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

		public double Value
		{
			get
			{
				/*if (_value < 250 || _value > 350)
				{
					return 0;
				}*/
				return _value;
			}
			set
			{
				if (_value != value)
				{
					_value = value;
					OnPropertyChanged("Value");
				}
			}
		}
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged(nameof(IsSelected));
				}
			}
		}

		public List<Pair<DateTime, double>> Last_5_Values
		{
			get
			{
				return _last_5_values;
			}
			set
			{
				if (_last_5_values != value)
				{
					_last_5_values = value;
					OnPropertyChanged(nameof(Last_5_Values));
				}
			}
		}

		public Entity()
		{
			Last_5_Values = new List<Pair<DateTime, double>>();
		}
		public Entity(int id, string name, Type type)
		{
			Id = id;
			Name = name;
			Type = new EntityType(type);
			Last_5_Values = new List<Pair<DateTime, double>>();

		}
		public override string ToString()
		{
			return $"Entity [Id={id}, Name={name}, Type={type}, Value={_value}]";
		}
	}
}
