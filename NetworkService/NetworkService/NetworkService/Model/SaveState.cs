using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
	public enum CommandType { SwitchViews, EntityManipulation, CanvasManipulation, LineManipulation }
	public class SaveState<TFirst, TSecond>
	{
		public TFirst CommandType { get; set; }
		public TSecond SavedState { get; set; }
		public SaveState(TFirst commandType, TSecond state)
		{
			CommandType = commandType;
			SavedState = state;
		}
		public SaveState()
		{

		}

	}
}
