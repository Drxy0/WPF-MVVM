using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
	public class DisplayViewModel : BindableBase
	{
		private bool rightClickActive = false;
		private static int x1 = 0;
		private static int x2 = 0;
		private static int y1 = 0;
		private static int y2 = 0;
		
		private Entity draggedItem = null;
		private bool dragging = false;
		public int draggingSourceIndex = -1;

		private bool isLineSourceSelected = false;
		private int sourceCanvasIndex = -1;
		private int destinationCanvasIndex = -1;
		
		private Entity selectedEntity;

		private Dictionary<int, Pair<int, int>> CanvasCentres = new Dictionary<int, Pair<int, int>>();

		private static ObservableCollection<string> canvasIDCollection = new ObservableCollection<string> { "X", "X", "X", "X", "X", "X", "X", "X", "X", "X", "X", "X"};
		private static ObservableCollection<string> canvasValueCollection = new ObservableCollection<string> { "X", "X", "X", "X", "X", "X", "X", "X", "X", "X", "X", "X" };
		private static ObservableCollection<bool> canvasLines= new ObservableCollection<bool> { false, false, false, false, false, false, false, false, false, false, false, false };
		public static Dictionary<string, Line> Lines { get; set; } = new Dictionary<string, Line>();
		public ObservableCollection<Line> LinesToDisplay { get; set; }
		public static ObservableCollection<string> CanvasIDCollection { get { return canvasIDCollection; } set { canvasIDCollection = value; } }
		public static ObservableCollection<string> CanvasValueCollection { get { return canvasValueCollection; } set { canvasValueCollection = value; } }
		public static ObservableCollection<bool> CanvasLines { get { return canvasLines; } set { canvasLines = value; } }
		public static ObservableCollection<Entity> RTD_Entities { get; set; } = MainWindowViewModel.RTD_Entities;
		public static ObservableCollection<Entity> TermoSprega_Entities { get; set; } = MainWindowViewModel.TermoSprega_Entities;
		public static ObservableCollection<Entity> entities { get; set; } = MainWindowViewModel.Entities;
		public ObservableCollection<Canvas> CanvasCollection { get; set; }

		public MyICommand<object> SelectionChanged_TreeView { get; set; }
		public MyICommand MouseLeftButtonUp_TreeView { get; set; }
		public MyICommand<object> DropEntityOnCanvas { get; set; }
		public MyICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
		public MyICommand MouseLeftButtonUpCanvas { get; set; }
		public MyICommand<object> RightMouseButtonUpOnCanvas { get; set; }
		public MyICommand<object> FreeUpCanvas { get; set; }

		public Entity SelectedEntity { get { return selectedEntity; } set { selectedEntity = value; OnPropertyChanged("SelectedEntity"); } }

		public DisplayViewModel()
		{
			InitializeCanvases();
			InitialiseCentres();

			LinesToDisplay = new ObservableCollection<Line>();
			MouseLeftButtonUp_TreeView = new MyICommand(OnMouseLeftButtonUp);
			SelectionChanged_TreeView = new MyICommand<object>(OnSelectionChanged);
			DropEntityOnCanvas = new MyICommand<object>(OnDrop);
			MouseLeftButtonUpCanvas = new MyICommand(OnMouseLeftButtonUp);
			RightMouseButtonUpOnCanvas = new MyICommand<object>(OnMouseRightButtonUp);
			LeftMouseButtonDownOnCanvas = new MyICommand<object>(OnLeftMouseButtonDown);
			FreeUpCanvas = new MyICommand<object>(OnFreeUpCanvas);

		}

		private void InitialiseCentres()
		{
			CanvasCentres.Add(0, new Pair<int,int>(135, 130));
			CanvasCentres.Add(1, new Pair<int,int>(410, 130));
			CanvasCentres.Add(2, new Pair<int,int>(675, 130));
			CanvasCentres.Add(3, new Pair<int,int>(950, 130));

			CanvasCentres.Add(4, new Pair<int, int>(135, 385));
			CanvasCentres.Add(5, new Pair<int, int>(410, 385));
			CanvasCentres.Add(6, new Pair<int, int>(675, 385));
			CanvasCentres.Add(7, new Pair<int, int>(950, 385));

			CanvasCentres.Add(8, new Pair<int, int>(135, 640));
			CanvasCentres.Add(9, new Pair<int, int>(410, 640));
			CanvasCentres.Add(10,new Pair<int, int>(675, 640));
			CanvasCentres.Add(11,new Pair<int, int>(950, 640));
		}
		private void OnMouseRightButtonUp(object e)
		{
			int index = int.Parse(e.ToString());
			//needs more work//
			if (!rightClickActive)
			{
				x1 = CanvasCentres[index].Item1;
				y1 = CanvasCentres[index].Item2;

				foreach (var line in LinesToDisplay)
				{
					if ((line.X1 == x1 && line.Y1 == y1))     //Line already connected
					{
						MessageBox.Show("Line already connected");
						return;
					}
				}

			}
			else
			{
				x2 = CanvasCentres[index].Item1;
				y2 = CanvasCentres[index].Item2;

				if (x1 == x2 && y1 == y2)
					return;

				foreach (var line in LinesToDisplay)
				{
					if ((line.X1 == x1 && line.Y1 == y1) || (line.X2 == x2 && line.Y2 == y2))     //Line already connected
					{
						MessageBox.Show("Line already connected");
						return;
					}
				}


				Line newLine = new Line
				{
					X1 = x1,
					X2 = x2,
					Y1 = y1,
					Y2 = y2,
					Stroke = Brushes.Black,
					StrokeThickness = 5
				};

				if (!LinesToDisplay.Contains(newLine))
					LinesToDisplay.Add(newLine);

			}

			rightClickActive = !rightClickActive;

			//MessageBox.Show(index.ToString());


			//index 0 X1 - 130, Y1 - 135
			//index 1 X1 - 410, Y1 - 135
			//index 2 X1 - 675, Y1 - 135
			//index 3 X1 - 950, Y1 - 135

			//index 4 X1 - 130, Y1 - 385
			//index 5 X1 - 675, Y1 - 385
			//index 6 X1 - 675, Y1 - 385
			//index 7 X1 - 950, Y1 - 385

			//index 8 X1 - 130, Y1 - 640
			//index 9 X1 - 675, Y1 - 640
			//index 10 X1 - 675, Y1 - 640
			//index 11 X1 - 950, Y1 - 640


			/*if (CanvasCollection[index].Resources.Contains("taken"))
			{
				//draw a line, if it's not already drawn
				if (IsLineAlreadyDrawn(_draggingSourceIndex, index) == 0)
				{
					//save state to undo stack before anything happens
					//SaveState();

					int source = Math.Min(draggingSourceIndex, index);
					int destination = Math.Max(draggingSourceIndex, index);

					Lines.Add($"{source},{destination}", CreateNewLine(source, destination));

					var linesArray = Lines.Values.ToArray();

					for (int i = 0; i < linesArray.Length; i++)
					{
						try
						{
							LinesToDisplay.Add(linesArray[i]);
						}
						catch (ArgumentOutOfRangeException ex)
						{
							Console.WriteLine("[ERROR]: " + ex.Message);
							Console.WriteLine(linesArray[i].ToString());
							Console.WriteLine(linesArray[i].Uid);
							Console.WriteLine($"{linesArray[i].X1},{linesArray[i].Y1}");
							Console.WriteLine($"{linesArray[i].X2},{linesArray[i].Y2}");
						}

					}
					//DrawExistingLines();
				}

				draggingSourceIndex = -1;
				draggedItem = null;
				dragging = false;
			}*/
		}

		private double FindCorrectX(int index) 
		{ 
			return 0; 
		}
		private double FindCorrectY(int index) 
		{ 
			return 0; 
		}
		private Line CreateNewLine(int sourceIndex, int destinationIndex)
		{
			Line newLine = new Line
			{
				X1 = Math.Abs(sourceIndex),
				Y1 = Math.Abs(sourceIndex),
				X2 = Math.Abs(destinationIndex),
				Y2 = Math.Abs(destinationIndex),
				Stroke = Brushes.Teal,
				StrokeThickness = 3,
				StrokeStartLineCap = PenLineCap.Round,
				StrokeEndLineCap = PenLineCap.Round
			};
			return newLine;
		}

		private void OnMouseLeftButtonUp()
		{
			draggedItem = null;
			SelectedEntity = null;
			dragging = false;
			draggingSourceIndex = -1;
		}
		private void OnSelectionChanged(object selectedItem)
		{
			if (!dragging && selectedItem is Entity selectedEntity)
			{
				dragging = true;
				draggedItem = selectedEntity;
				DragDrop.DoDragDrop(Application.Current.MainWindow, draggedItem, DragDropEffects.Move | DragDropEffects.Copy);
			}
		}

		private void OnLeftMouseButtonDown(object entity)
		{
			if (!dragging)
			{
				int index = Convert.ToInt32(entity);

				if (CanvasCollection[index].Resources["taken"] != null)
				{
					dragging = true;
					draggedItem = (Entity)(CanvasCollection[index].Resources["data"]);
					draggingSourceIndex = index;
					DragDrop.DoDragDrop(CanvasCollection[index], draggedItem, DragDropEffects.Move);
				}
			}
		}

		private void OnDrop(object entity)
		{
			bool rtd = false;
			if (draggedItem != null)
			{
				int index = Convert.ToInt32(entity);

				if (CanvasCollection[index].Resources["taken"] == null)
				{
					CanvasIDCollection[index] = draggedItem.Id.ToString();
					CanvasValueCollection[index] = draggedItem.Value.ToString();

					BitmapImage image = new BitmapImage();
					image.BeginInit();

					if (draggedItem.Type.ToString() == "RTD")
					{
						image.UriSource = new Uri("pack://application:,,,/NetworkService;component/Assets/RTD.png");
						rtd = true;
					}
					else
					{
						image.UriSource = new Uri("pack://application:,,,/NetworkService;component/Assets/TermoSprega.png");
					}

					image.EndInit();

					CanvasCollection[index].Background = new ImageBrush(image);
					CanvasCollection[index].Resources.Add("taken", true);
					CanvasCollection[index].Resources.Add("data", draggedItem);

					if (draggingSourceIndex != -1)
					{
						CanvasCollection[draggingSourceIndex].Background = Brushes.LightGray;
						CanvasCollection[draggingSourceIndex].Resources.Remove("taken");
						CanvasCollection[draggingSourceIndex].Resources.Remove("data");

						draggingSourceIndex = -1;
					}
					else
					{
						if (rtd)
						{
							RTD_Entities.Remove(draggedItem);
						}
						else
						{
							TermoSprega_Entities.Remove(draggedItem);
						}
					}
				}
			}
		}

		private void InitializeCanvases()
		{
			CanvasCollection = new ObservableCollection<Canvas>();
			for (int i = 0; i < 12; i++)
			{
				CanvasCollection.Add(new Canvas
				{
					Background = Brushes.LightGray,
					AllowDrop = true
				});
			}
		}

		public void DeleteEntityFromCanvas(Entity e)
		{
			int canvasIndex = GetCanvasIndexForEntityId(e.Id);

			if (canvasIndex != -1)
			{
				CanvasCollection[canvasIndex].Background = Brushes.LightGray;
				CanvasCollection[canvasIndex].Resources.Remove("taken");
				CanvasCollection[canvasIndex].Resources.Remove("data");

			}
		}

		private int GetCanvasIndexForEntityId(int id)
		{
			for (int i = 0; i < CanvasCollection.Count; i++)
			{
				Entity entity = (CanvasCollection[i].Resources["data"]) as Entity;

				if ((entity != null) && entity.Id == id)
				{
					return i;
				}
			}
			return -1;
		}
		private void OnFreeUpCanvas(object parameter)
		{
			int index = Convert.ToInt32(parameter);

			if (CanvasCollection[index].Resources["taken"] != null)
			{
				Entity tmpEntity = (Entity)CanvasCollection[index].Resources["data"];
				if (tmpEntity.Type.ToString().Equals("RTD"))
					RTD_Entities.Add(tmpEntity);
				else
					TermoSprega_Entities.Add(tmpEntity);

				//EntitiesInList.Add((Entity)CanvasCollection[index].Resources["data"]);
				CanvasCollection[index].Background = Brushes.LightGray;
				CanvasCollection[index].Resources.Remove("taken");
				CanvasCollection[index].Resources.Remove("data");
			}
		}

	}
}
