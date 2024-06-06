using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Reflection;
using NetworkService.Views;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Media.Imaging;
//using System.Data;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
	{
		//private int count = 15; // Inicijalna vrednost broja objekata u sistemu
		// ######### ZAMENITI stvarnim brojem elemenata
		//           zavisno od broja entiteta u listi



		private bool idFound = false;
		private static int abs_id = -1;
		private int previousCanvasIndex_PutCommand = -1;

		private string entitiesIcon;
		private string displayIcon;
		private string graphIcon;
		private string terminalIcon;

		#region Teminal properties
		private string terminalDisplayVisible;
		private string terminalInputVisible;
		private string terminalLabelVisible;
		private static bool terminalVisible = false;
		private int caretIndexTerminal;
		private int terminalDisplayHeight;
		private static string help = @"
	COMMAND LIST

	Add entity into collection:					add (id: int) (name: string) (type: RTD or TermoSprega)
	Delete entity from collection:				del (id: int) ...
	Search entities by type or name:				search (option: string) (single word query: string)
	Put entity into display view canvas:				put (id: int) (canvasIndex: int)
	Remove entity from display view canvas:			pop (id: int) ...
	Connect two entites in display view:				connect (canvasIndex: int) (canvasIndex: int)
	Move an entity from one canvas position to another:	move (sourceCanvasIndex: int) (destinationCanvasIndex: int)
	Show graph for selected entity:				show (id: int)
					";
		public int CaretIndexTerminal
		{
			get { return caretIndexTerminal; }
			set
			{
				if (caretIndexTerminal != value)
				{
					caretIndexTerminal = value;
					OnPropertyChanged(nameof(CaretIndexTerminal));
				}
			}
		}
		public string TerminalDisplayVisible
		{
			get { return terminalDisplayVisible; }
			set
			{
				if (terminalDisplayVisible != value)
				{
					terminalDisplayVisible = value;
					OnPropertyChanged(nameof(TerminalDisplayVisible));
				}
			}
		}
		public string TerminalInputVisible
		{
			get { return terminalInputVisible; }
			set
			{
				if (terminalInputVisible != value)
				{
					terminalInputVisible = value;
					OnPropertyChanged(nameof(TerminalInputVisible));
				}
			}
		}
		public string TerminalLabelVisible
		{
			get { return terminalLabelVisible; }
			set
			{
				if (terminalLabelVisible != value)
				{
					terminalLabelVisible = value;
					OnPropertyChanged(nameof(TerminalLabelVisible));
				}
			}
		}
		public int TerminalDisplayHeight
		{
			get { return terminalDisplayHeight; }
			set
			{
				if (terminalDisplayHeight != value)
				{
					terminalDisplayHeight = value;
					OnPropertyChanged(nameof(TerminalDisplayHeight));
				}
			}
		}
		#endregion
		public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<Window> CloseWindowCommand { get; private set; }
		public MyICommand UndoCommand { get; set; }
		public MyICommand PreviousViewCommand { get; set; }

		public static ObservableCollection<Entity> Entities { get; set; } = new ObservableCollection<Entity>();
		public static ObservableCollection<Entity> RTD_Entities { get; set; } = new ObservableCollection<Entity>();
		public static ObservableCollection<Entity> TermoSprega_Entities { get; set; } = new ObservableCollection<Entity>();
		public static Dictionary<int, int> idIndex_Pair { get; set; } = new Dictionary<int, int>();
        public EntitiesViewModel entitiesViewModel;
        public DisplayViewModel displayViewModel;
        public GraphViewModel graphViewModel;
        private BindableBase currentViewModel;
		private NotificationManager notificationManager;

		private object _selectedContent;
		public object SelectedContent
		{
			get => _selectedContent;
			set
			{
				SetProperty(ref _selectedContent, value);
				UndoCommand.RaiseCanExecuteChanged();
			}
		}

		public static SaveState<CommandType, object> Undo { get; set; }
		public static SaveState<CommandType, object> PreviousView { get; set; }
		public string EntitiesIcon
		{
			get { return entitiesIcon; }
			set { entitiesIcon = value; OnPropertyChanged(nameof(EntitiesIcon)); }
		}
		public string DisplayIcon
		{
			get { return displayIcon; }
			set { displayIcon = value; OnPropertyChanged(nameof(DisplayIcon)); }
		}
		public string GraphIcon
		{
			get { return graphIcon; }
			set { graphIcon = value; OnPropertyChanged(nameof(GraphIcon)); }
		}
		public string TerminalIcon
		{
			get { return terminalIcon; }
			set { terminalIcon = value; OnPropertyChanged(nameof(TerminalIcon)); }
		}

		#region Commands
		public ICommand KeyPressedCommand { get; }
		public ICommand ShiftBacktickCommand { get; }
		public ICommand Shift1_Command { get; }
		public ICommand Shift2_Command { get; }
		public ICommand Shift3_Command { get; }
		public ICommand ShiftUp_Command {  get; }
		public ICommand ShiftDown_Command {  get; }
		public ICommand TerminalCommand { get; private set; }
		#endregion

		private string terminalInput;
		public string TerminalInput
		{
			get { return terminalInput; }
			set 
			{ 
				terminalInput = value; 
				OnPropertyChanged(nameof(TerminalInput));
			}
		}
		private string terminalDisplay;
		public string TerminalDisplay
		{
			get { return terminalDisplay; }
			set { terminalDisplay = value; OnPropertyChanged(nameof(TerminalDisplay)); }
		}

		private string terminalLabel;
		public string TerminalLabel
		{
			get { return terminalLabel; }
			set { terminalLabel = value; OnPropertyChanged(nameof(TerminalLabel)); }

		}
		public static Mutex Mutex { get; set; } = new Mutex();
		public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom

			notificationManager = new NotificationManager();
			Undo = new SaveState<CommandType, object>();

			NavCommand = new MyICommand<string>(OnNav);
			TerminalCommand = new RelayCommand(OnTerminalButton);
			CloseWindowCommand = new MyICommand<Window>(CloseWindow);
			ShiftBacktickCommand = new RelayCommand(OnShiftBacktickPressed);
			Shift1_Command = new RelayCommand(OnShift1);
			Shift2_Command = new RelayCommand(OnShift2);
			Shift3_Command = new RelayCommand(OnShift3);

			ShiftUp_Command = new RelayCommand(OnShiftUp);
			ShiftDown_Command = new RelayCommand(OnShiftDown);
			UndoCommand = new MyICommand(OnUndo);
			PreviousViewCommand = new MyICommand(OnPreviousView);
			KeyPressedCommand = new RelayCommand(OnKeyPressed);

			Entities = new ObservableCollection<Entity>();

			entitiesViewModel = new EntitiesViewModel();
            displayViewModel = new DisplayViewModel();
            graphViewModel = new GraphViewModel();
            CurrentViewModel = entitiesViewModel;

			PreviousView = new SaveState<CommandType, object>(CommandType.SwitchViews, CurrentViewModel.GetType());
			InitialiseFields();
			ShowHide_Terminal();
		}

		private void InitialiseFields()
		{
			TerminalInput = string.Empty;
			TerminalDisplay = string.Empty;
			TerminalLabel = "Terminal input: ";
			EntitiesIcon = "Assets/entities_white.png";
			DisplayIcon = "Assets/display.png";
			GraphIcon = "Assets/graph.png";
			TerminalIcon = "Assets/terminal.png";
			TerminalDisplayHeight = 200;
		}

		private void OnPreviousView()
		{
			SaveState<CommandType, object> saveState = PreviousView;
			if (saveState.CommandType == CommandType.SwitchViews)
			{
				System.Type viewType = saveState.SavedState as System.Type;

				if (viewType == typeof(EntitiesViewModel))
				{
					CurrentViewModel = entitiesViewModel;
					PreviousView = new SaveState<CommandType, object>(CommandType.SwitchViews, CurrentViewModel.GetType());
					NavEntities();
				}
				else if (viewType == typeof(DisplayViewModel))
				{
					CurrentViewModel = displayViewModel;
					PreviousView = new SaveState<CommandType, object>(CommandType.SwitchViews, CurrentViewModel.GetType());
					NavDisplay();
				}
				else if (viewType == typeof(GraphViewModel))
				{
					CurrentViewModel = graphViewModel;
					PreviousView = new SaveState<CommandType, object>(CommandType.SwitchViews, CurrentViewModel.GetType());
					NavGraph();
				}
			}
		}


		#region Terminal
		private void OnShiftUp(object e)
		{
			if (TerminalDisplayHeight < 400)
				TerminalDisplayHeight += 20;
		}
		private void OnShiftDown(object e)
		{
			if (TerminalDisplayHeight > 0)
				TerminalDisplayHeight -= 20;
		}
		private void OnTerminalButton(object parameter)
		{
			terminalVisible = !terminalVisible;
			ShowHide_Terminal();
		}
		private void OnShiftBacktickPressed(object parameter)
		{
			terminalVisible = !terminalVisible;
			ShowHide_Terminal();
		}
		private void ShowHide_Terminal()
		{
			if (terminalVisible == true)
			{
				TerminalDisplayVisible = "Hidden";
				TerminalInputVisible = "Hidden";
				TerminalLabelVisible = "Hidden";
				TerminalIcon = "Assets/terminal.png";
			}
			else
			{
				TerminalDisplayVisible = "Visible";
				TerminalInputVisible = "Visible";
				TerminalLabelVisible = "Visible";
				TerminalIcon = "Assets/terminal_white.png";
			}

		}
		private void OnShift1(object parameter)
		{
			OnNav("entities");
		}
		private void OnShift2(object parameter)
		{
			OnNav("display");
		}
		private void OnShift3(object parameter)
		{
			OnNav("graph");
		}

		private void OnKeyPressed(object parameter)
		{
			if (parameter is KeyEventArgs e)
			{
				if (e.Key == Key.Escape)
					TerminalInput = string.Empty;
				if (e.Key == Key.Tab)
				{
					e.Handled = true;
					AutoCompleteCommands();
				}


				if (e.Key == Key.Enter)
				{
					TerminalDisplay += '\n' + TerminalInput;

					if (TerminalLabel == "Entity input: ")
					{
						HandleAddEntity();
					}

					if (TerminalInput == string.Empty)
						return;

					if (TerminalInput.ToLower().StartsWith("add"))
					{
						HandleAddEntity();
					}
					else if (TerminalInput.ToLower().StartsWith("del"))
					{
						HandleDeleteEntity();
					}
					else if (TerminalInput.ToLower().StartsWith("search"))
					{
						HandleSearchEntity();
					}
					else if (TerminalInput.ToLower().StartsWith("put"))
					{
						HandlePutEntityOnCanvas();
					}
					else if (TerminalInput.ToLower().StartsWith("pop"))
					{
						HandlePopEntityFromCanvas();
					}
					else if (TerminalInput.ToLower().StartsWith("connect"))
					{
						HandleConnectEntitiesOnCanvas();
					}
					else if (TerminalInput.ToLower().StartsWith("move"))
					{
						HandleMoveEntityOnCanvas();
					}
					else if (TerminalInput.ToLower().StartsWith("show"))
					{
						HandleShowGraph();
					}
					else if (TerminalInput.ToLower() == "-h")
					{
						TerminalDisplay += '\n' + help;
					}
					else
					{
						TerminalDisplay += '\n' + $"Input error: Command not recognised. Type -h for command list.";
					}
					TerminalInput = string.Empty;
				}
			}
		}
		private void HandleShowGraph()
		{
			string[] parameters = TerminalInput.Split(' ');
			if (parameters.Length != 2)
			{
				TerminalDisplay += '\n' + "Input error: Invalid parameter nubmer. Correct syntax is: show (id: int)";
				return;
			}
			if (!int.TryParse(parameters[1], out int result))   //if not integer
			{
				TerminalDisplay += '\n' + "Input error: Parameter must be an integer. Correct syntax is: show (id: int)";
				return;
			}
			int entityId = int.Parse(parameters[1]);
			int entityIndexInCollection = -1;
			bool hasPair = idIndex_Pair.TryGetValue(entityId, out entityIndexInCollection);
			if (entityIndexInCollection != -1 && hasPair)
			{
				graphViewModel.SelectedEntity = Entities[entityIndexInCollection];
				graphViewModel.SelectedItemId = $"Displaying {entityId}"; 
			}
			else
			{
				TerminalDisplay += '\n' + $"Input error: Entity with id {entityId} not found.";
			}
		}
		private void HandleMoveEntityOnCanvas()
		{
			bool _return = false;
			string[] parameters = TerminalInput.Split(' ');

			if (parameters.Length != 3)
			{
				TerminalDisplay += '\n' + "Input error, correct syntax is: move (canvasIndex: int) (canvasIndex: int)";
				_return = true;
			}

			foreach (string integer in parameters.Skip(1))
			{
				if (!int.TryParse(integer, out int result))   //if not integer
				{
					TerminalDisplay += '\n' + "Input error, correct syntax is: move (canvasIndex: int) (canvasIndex: int)";
					_return = true;
				}
			}
			int sourceIndex = int.Parse(parameters[1]);
			int destinationIndex = int.Parse(parameters[2]);

			if (destinationIndex < 0 || destinationIndex > 11)
			{
				TerminalDisplay += '\n' + "Error: Incorrect canvas index value. Correct values are 0 to 11";
				_return = true;
			}

			if (_return) return;

			displayViewModel.OnLeftMouseButtonDown(sourceIndex);
			displayViewModel.OnSelectionChanged(sourceIndex);
			displayViewModel.OnDrop(destinationIndex);
			displayViewModel.OnMouseLeftButtonUp();

			TerminalInput = string.Empty;
		}
		private void HandlePutEntityOnCanvas()
		{
			bool _return = false;
			string[] parameters = TerminalInput.Split(' ');

			if (parameters.Length != 3)
			{
				TerminalDisplay += '\n' + "Input error, correct syntax is: put (id: int) (canvasIndex: int)";
				_return = true;
			}

			foreach (string integer in parameters.Skip(1))
			{
				if (!int.TryParse(integer, out int result))   //if not integer
				{
					TerminalDisplay += '\n' + "Input error, correct syntax is: put (id: int) (canvasIndex: int)";
					_return = true;
				}
			}
			int id = int.Parse(parameters[1]);
			int canvasIndex = int.Parse(parameters[2]);

			if (entitiesViewModel._Entities.Where(e => e.Id == id).ToList().Count == 0)	//If id not in list
			{
				TerminalDisplay += '\n' + "Error: Entity with id {result} not found.";
				_return = true;
			}
			if (canvasIndex < 0 || canvasIndex > 11)
			{
				TerminalDisplay += '\n' + "Error: Incorrect canvas index value. Correct values are 0 to 11";
				_return = true;
			}

			if (_return) return;
			int indexOfElement = -1;
			idIndex_Pair.TryGetValue(id, out indexOfElement);

			if (previousCanvasIndex_PutCommand != -1)
			{
				displayViewModel.InvokeOnFreeUpCanvas(previousCanvasIndex_PutCommand);
			}


			displayViewModel.OnSelectionChanged(Entities[indexOfElement]);
			displayViewModel.OnDrop(canvasIndex);

			previousCanvasIndex_PutCommand = canvasIndex;
			TerminalInput = string.Empty;
		}
		private void HandlePopEntityFromCanvas()
		{
			string[] parameters = TerminalInput.Split(' ');
			foreach (string id in parameters.Skip(1))
			{
				if (!int.TryParse(id, out int result))   //if not integer
				{
					TerminalDisplay += '\n' + $"Input error: {id} is not an integer. Correct syntax is: pop (id: int)...";
				}
				else if (result < 0 || result > 11)
				{
					TerminalDisplay += '\n' + "Error: Incorrect canvas index value. Correct values are 0 to 11";
					return;
				}
				else
				{
					displayViewModel.OnFreeUpCanvas(id);
					TerminalDisplay += '\n' + $"Success: Canvas {id} sucessfully popped";
				}
			}
		}
		private void HandleConnectEntitiesOnCanvas()
		{
			bool _return = false;
			string[] parameters = TerminalInput.Split(' ');

			if (parameters.Length != 3)
			{
				TerminalDisplay += '\n' + "Input error, correct syntax is: connect (canvasIndex: int) (canvasIndex: int)";
				_return = true;
			}

			foreach (string integer in parameters.Skip(1))
			{
				if (!int.TryParse(integer, out int result))   //if not integer
				{
					TerminalDisplay += '\n' + "Input error, correct syntax is: connect (canvasIndex: int) (canvasIndex: int)";
					_return = true;
				}
			}
			int sourceIndex = int.Parse(parameters[1]);
			int destinationIndex = int.Parse(parameters[2]);

			if (destinationIndex < 0 || destinationIndex > 11)
			{
				TerminalDisplay += '\n' + "Error: Incorrect canvas index value. Correct values are 0 to 11.";
				_return = true;
			}

			if (_return) return;
			displayViewModel.OnMouseRightButtonUp(sourceIndex);
			displayViewModel.OnMouseRightButtonUp(destinationIndex);
			//TerminalDisplay += '\n' + $"Success: Canvases {sourceIndex} and {destinationIndex} connected.";
		}
		private void AutoCompleteCommands()
		{
			if(IsSubsequence(TerminalInput, "add"))
			{
				TerminalInput = "add";
				CaretIndexTerminal = 3;
			}
			else if (IsSubsequence(TerminalInput, "del"))
			{
				TerminalInput = "del";
				CaretIndexTerminal = 3;
			}
			else if (IsSubsequence(TerminalInput, "search"))
			{
				TerminalInput = "search";
				CaretIndexTerminal = 6;
			}
			else if (IsSubsequence(TerminalInput, "put"))
			{
				TerminalInput = "put";
				CaretIndexTerminal = 3;
			}
			else if (IsSubsequence(TerminalInput, "pop"))
			{
				TerminalInput = "pop";
				CaretIndexTerminal = 3;
			}
			else if (IsSubsequence(TerminalInput, "connect"))
			{
				TerminalInput = "connect";
				CaretIndexTerminal = 7;
			}
			else if (IsSubsequence(TerminalInput, "move"))
			{
				TerminalInput = "move";
				CaretIndexTerminal = 4;
			}
			else if (IsSubsequence(TerminalInput, "show"))
			{
				TerminalInput = "show";
				CaretIndexTerminal = 4;
			}
		}
		private static bool IsSubsequence(string input, string target)
		{
			int inputIndex = 0;
			int targetIndex = 0;

			while (inputIndex < input.Length && targetIndex < target.Length)
			{
				if (input[inputIndex] == target[targetIndex])
				{
					inputIndex++;
				}
				targetIndex++;
			}

			return inputIndex == input.Length;
		}
		private void HandleAddEntity()
		{
			string[] parameters = TerminalInput.Split(' ');
			if (parameters.Length != 4)
			{
				TerminalDisplay += '\n' + "Input error, correct syntax is: add (id: int) (name: string) (type: RTD or TermoSprega)";
				TerminalInput = string.Empty;
				return;
			}
			if (parameters[3].ToLower() == "ts" || parameters[3].ToLower() == "termo")
			{
				parameters[3] = "termosprega";
			}
			if (parameters[3].ToLower() != "rtd" && parameters[3].ToLower() != "termosprega")
			{
				TerminalDisplay += '\n' + "Add Entity Error: avaivable types are RTD and TermoSprega";
				TerminalInput = string.Empty;
			}
			else
			{
				entitiesViewModel.IdText = parameters[1];
				entitiesViewModel.NameText = parameters[2];
				if (parameters[3].ToLower() == "rtd")
				{
					entitiesViewModel.RtdChecked = true;
					entitiesViewModel.TermoChecked = false;
				}
				else
				{
					entitiesViewModel.RtdChecked = false;
					entitiesViewModel.TermoChecked = true;
					
				}

				entitiesViewModel.OnAdd();
				TerminalDisplay += '\n' + $"Entity {parameters[1]} added successfully";
			}
		}
		private void HandleDeleteEntity()
		{
			string[] parameters = TerminalInput.Split(' ');
			idFound = false;
			foreach (string id in parameters.Skip(1))
			{
				if (!int.TryParse(id, out int result))   //if id is not int
				{
					TerminalDisplay += '\n' + "Input error. Correct syntax is: del (id: int)";
					TerminalInput = string.Empty;
				}
				else
				{
					foreach (var item in entitiesViewModel._Entities.Where(e => e.Id == result).ToList())
					{
						item.IsSelected = true;
						idFound = true;
					}

					if (idFound)
					{
						entitiesViewModel.OnDelete();
						TerminalDisplay += '\n' + $"Entity with id {result} deleted successfully.";
						TerminalInput = string.Empty;
					}
					else
					{
						TerminalDisplay += '\n' + $"Error: Entity with id {result} not found.";
						ToastNotify.RaiseToast(
							"Delete Error",
							$"Entity with id {result} not found.",
							Notification.Wpf.NotificationType.Error);
					}
				}
			}
		}
		private void HandleSearchEntity()
		{
			string[] parameters = TerminalInput.Split(' ');
			if (parameters.Length != 3)
			{
				TerminalDisplay += '\n' + "Input error, correct syntax is: search (option: string) (query: string)";
				TerminalInput = string.Empty;
			}
			if (parameters[1].ToLower() == "naziv" || parameters[1].ToLower() == "name")
			{

			}
			else if (parameters[1].ToLower() == "tip" || parameters[1].ToLower() == "type")
			{

			}
			else
			{
				TerminalDisplay += '\n' + "Input error, avaivable options are: naziv/name and tip/type";
				TerminalInput = string.Empty;
			}
		}
		#endregion



		private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            //Byte[] data = System.Text.Encoding.ASCII.GetBytes(count.ToString());
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(Entities.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

							//################ IMPLEMENTACIJA ####################
							// Obraditi poruku kako bi se dobile informacije o izmeni
							// Azuriranje potrebnih stvari u aplikaciji

							if (Entities.Count > 0)
							{
								var splited = incomming.Split(':');
								DateTime dt = DateTime.Now;
								using (StreamWriter sw = File.AppendText("Log.txt"))
								sw.WriteLine(dt + "; " + splited[0] + ", " + splited[1]);

								int id = Int32.Parse(splited[0].Split('_')[1]);
								abs_id = id;

								Entities[id].Value = Math.Round(Double.Parse(splited[1]), 2);
								AddValueToList(Entities[id]);
								int id_of_entity = Entities[id].Id;
								if (!idIndex_Pair.ContainsKey(id_of_entity))
									idIndex_Pair.Add(id_of_entity, id);

								UpdateCanvasValue(id_of_entity);
							}
						}
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
		private void AddValueToList(Entity entity)
		{
			if (entity.Last_5_Values.Count == 5)
			{
				entity.Last_5_Values.RemoveAt(0);
				entity.Last_5_Values.Add(new Pair<DateTime, double>(DateTime.Now, entity.Value));
			}
			else
			{
				entity.Last_5_Values.Add(new Pair<DateTime, double>(DateTime.Now, entity.Value));
			}
		}
		public BindableBase CurrentViewModel
		{
			get { return currentViewModel; }
			set
			{
				SetProperty(ref currentViewModel, value);
				OnPropertyChanged(nameof(CurrentViewModel));
			}
		}
		private void OnUndo()
		{
			SaveState<CommandType, object> saveState = Undo;

			if (saveState.CommandType == CommandType.EntityManipulation)
			{
				Entities = saveState.SavedState as ObservableCollection<Entity>;
				//refresh the view
				CurrentViewModel = entitiesViewModel;
			}
			else if (saveState.CommandType == CommandType.CanvasManipulation)
			{
				Mutex.WaitOne();

				List<object> state = saveState.SavedState as List<object>;
				Pair<int, Entity> entityState = state[0] as Pair<int, Entity>;
				int nuIndex = int.Parse(state[1] as string);

				if (entityState.Item1 == -2)
				{
					ToastNotify.RaiseToast("Error", "There is nothing to undo!", Notification.Wpf.NotificationType.Warning);
					return;
				}
				else if (entityState.Item1 == -1)
				{
					displayViewModel.InvokeOnFreeUpCanvas(nuIndex);
				}
				else
				{
					DisplayViewModel.CanvasIDCollection[entityState.Item1] = entityState.Item2.Id.ToString();
					DisplayViewModel.CanvasValueCollection[entityState.Item1] = entityState.Item2.Value.ToString();

					BitmapImage image = new BitmapImage();
					image.BeginInit();

					if (entityState.Item2.Type.ToString() == "RTD")
					{
						image.UriSource = new Uri("pack://application:,,,/NetworkService;component/Assets/RTD.png");
					}
					else
					{
						image.UriSource = new Uri("pack://application:,,,/NetworkService;component/Assets/TermoSprega.png");
					}

					image.EndInit();
					try
					{
						DisplayViewModel.CanvasCollection[entityState.Item1].Background = new ImageBrush(image);
						DisplayViewModel.CanvasCollection[entityState.Item1].Resources.Add("taken", true);
						DisplayViewModel.CanvasCollection[entityState.Item1].Resources.Add("data", entityState.Item2);
						displayViewModel.ResetCanvas(nuIndex);
					}
					catch { }
					DisplayViewModel.AddedToGrid = new Pair<int, Entity>(-2, new Entity());
				}
				DisplayViewModel.SaveState(nuIndex);
				Mutex.ReleaseMutex();
			}
			else if(saveState.CommandType == CommandType.LineManipulation)
			{
				Mutex.WaitOne();
				int lastIndex = int.Parse(saveState.SavedState as string);
				if (lastIndex >= 0)
				{
					DisplayViewModel.LineCollection.RemoveAt(lastIndex);
				}
				DisplayViewModel.SaveStateLine();
				Mutex.ReleaseMutex();
			}
		}

		#region Update SideBar Icons
		private void NavEntities()
		{
			EntitiesIcon = "Assets/entities_white.png";
			DisplayIcon = "Assets/display.png";
			GraphIcon = "Assets/graph.png";
		}
		private void NavDisplay()
		{
			EntitiesIcon = "Assets/entities.png";
			DisplayIcon = "Assets/display_white.png";
			GraphIcon = "Assets/graph.png";
		}
		private void NavGraph()
		{
			EntitiesIcon = "Assets/entities.png";
			DisplayIcon = "Assets/display.png";
			GraphIcon = "Assets/graph_white.png";
		}
		#endregion

		private void OnNav(string destination)
		{
			switch (destination)
			{
				case "entities":

					PreviousView = new SaveState<CommandType, object>(CommandType.SwitchViews, CurrentViewModel.GetType());
					CurrentViewModel = entitiesViewModel;
					NavEntities();
					break;
				case "display":
					PreviousView = new SaveState<CommandType, object>(CommandType.SwitchViews, CurrentViewModel.GetType());
					CurrentViewModel = displayViewModel;
					NavDisplay();
					break;
				case "graph":
					PreviousView = new SaveState<CommandType, object>(CommandType.SwitchViews, CurrentViewModel.GetType());
					CurrentViewModel = graphViewModel;
					NavGraph();
					break;
			}
		}
		private void ShowToastNotification(NotificationContent notificationContent)
		{
			notificationManager.Show(notificationContent, "WindowNotificationArea");
		}

		private void CloseWindow(Window MainWindow)
		{
			MainWindow.Close();
		}

		public void UpdateCanvasValue(int id)
		{
			foreach (string entityIndex in DisplayViewModel.CanvasIDCollection)
			{
				int canvasId_ofEntity = entitiesViewModel.GetCanvasIndexForEntityId(id);
				if (canvasId_ofEntity == -1) return;
				string k = DisplayViewModel.CanvasIDCollection[canvasId_ofEntity].ToString();
				if (entityIndex == k)
				{
					DisplayViewModel.CanvasValueCollection[canvasId_ofEntity] = Entities[abs_id].Value.ToString();
					double q = Entities[abs_id].Value;
					if (q > 350 || q < 250)
					{
						DisplayViewModel.BorderBrushCollection[canvasId_ofEntity] = "Red";
					}
					else
					{
						DisplayViewModel.BorderBrushCollection[canvasId_ofEntity] = "Black";
					}
				}
			}
		}

	}

}
