using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
	public enum Type
	{
		RTD,
		TermoSprega
	}
	public class Entity : BindableBase
	{
		private int id;
		private string name;
		private Type type;
		private double _value;

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
		public double Value
		{
			get
			{
				if (_value < 250 || _value > 350)
				{
					return 0; //???
				}
				else
				{
					return _value;
				}
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
		public Entity() { }
		public Entity(int id, string name, Type type)
		{
			Id = id;
			Name = name;
			Type = type;
		}
		public override string ToString()
		{
			return $"Entity [Id={id}, Name={name}, Type={type}, Value={_value}]";
		}
	}
}
