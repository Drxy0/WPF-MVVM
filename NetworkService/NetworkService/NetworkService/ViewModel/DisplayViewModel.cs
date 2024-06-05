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
using System.Windows.Documents;
using Line = NetworkService.Model.Line;

namespace NetworkService.ViewModel
{
	public class DisplayViewModel : BindableBase
	{
		private Entity draggedItem = null;
		private bool dragging = false;
		public int draggingSourceIndex = -1;

		private bool isLineSourceSelected = false;
		private int sourceCanvasIndex = -1;
		private int destinationCanvasIndex = -1;
		
		private Entity selectedEntity;

		private static ObservableCollection<string> canvasIDCollection = new ObservableCollection<string> { "X", "X", "X", "X", "X", "X", "X", "X", "X", "X", "X", "X"};
		private static ObservableCollection<string> canvasValueCollection = new ObservableCollection<string> { "X", "X", "X", "X", "X", "X", "X", "X", "X", "X", "X", "X" };
		private static ObservableCollection<string> borderBrushCollection= new ObservableCollection<string> { "Black", "Black", "Black", "Black", "Black", "Black", "Black", "Black", "Black", "Black", "Black", "Black"};

		public static ObservableCollection<Line> LineCollection { get; set; }
		public static ObservableCollection<Canvas> CanvasCollection { get; set; }
		public static ObservableCollection<string> CanvasIDCollection { get { return canvasIDCollection; } set { canvasIDCollection = value; } }
		public static ObservableCollection<string> CanvasValueCollection { get { return canvasValueCollection; } set { canvasValueCollection = value; } }
		public static ObservableCollection<string> BorderBrushCollection { get { return borderBrushCollection; } set { borderBrushCollection = value; } }
		public static ObservableCollection<Entity> RTD_Entities { get; set; } = MainWindowViewModel.RTD_Entities;
		public static ObservableCollection<Entity> TermoSprega_Entities { get; set; } = MainWindowViewModel.TermoSprega_Entities;

		#region Commands
		public MyICommand<object> SelectionChanged_TreeView { get; set; }
		public MyICommand MouseLeftButtonUp_TreeView { get; set; }
		public MyICommand<object> DropEntityOnCanvas { get; set; }
		public MyICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
		public MyICommand MouseLeftButtonUpCanvas { get; set; }
		public MyICommand<object> RightMouseButtonUpOnCanvas { get; set; }
		public MyICommand<object> FreeUpCanvas { get; set; }
		#endregion
		public Entity SelectedEntity { get { return selectedEntity; } set { selectedEntity = value; OnPropertyChanged("SelectedEntity"); } }
		
		private Line currentLine = new Line();
		private Point linePoint1 = new Point();
		private Point linePoint2 = new Point();
		
		public DisplayViewModel()
		{
			InitializeCanvases();

			LineCollection = new ObservableCollection<Line>();
			MouseLeftButtonUp_TreeView = new MyICommand(OnMouseLeftButtonUp);
			SelectionChanged_TreeView = new MyICommand<object>(OnSelectionChanged);
			DropEntityOnCanvas = new MyICommand<object>(OnDrop);
			MouseLeftButtonUpCanvas = new MyICommand(OnMouseLeftButtonUp);
			RightMouseButtonUpOnCanvas = new MyICommand<object>(OnMouseRightButtonUp);
			LeftMouseButtonDownOnCanvas = new MyICommand<object>(OnLeftMouseButtonDown);
			FreeUpCanvas = new MyICommand<object>(OnFreeUpCanvas);
		}


		public void OnMouseRightButtonUp(object entity)
		{
			int index = Convert.ToInt32(entity);

			if (CanvasCollection[index].Resources["taken"] != null)
			{
				if (!isLineSourceSelected)
				{
					sourceCanvasIndex = index;

					linePoint1 = GetPointForCanvasIndex(sourceCanvasIndex);

					currentLine.X1 = linePoint1.X;
					currentLine.Y1 = linePoint1.Y;
					currentLine.Source = sourceCanvasIndex;

					isLineSourceSelected = true;
				}
				else
				{
					destinationCanvasIndex = index;

					if ((sourceCanvasIndex != destinationCanvasIndex) && !DoesLineAlreadyExist(sourceCanvasIndex, destinationCanvasIndex))
					{
						linePoint2 = GetPointForCanvasIndex(destinationCanvasIndex);

						currentLine.X2 = linePoint2.X;
						currentLine.Y2 = linePoint2.Y;
						currentLine.Destination = destinationCanvasIndex;

						LineCollection.Add(new Line
						{
							X1 = currentLine.X1,
							Y1 = currentLine.Y1,
							X2 = currentLine.X2,
							Y2 = currentLine.Y2,
							Source = currentLine.Source,
							Destination = currentLine.Destination
						});

						isLineSourceSelected = false;

						linePoint1 = new Point();
						linePoint2 = new Point();
						currentLine = new Line();
					}
					else
					{
						// Pocetak i kraj linije su u istom canvasu

						isLineSourceSelected = false;

						linePoint1 = new Point();
						linePoint2 = new Point();
						currentLine = new Line();

						ToastNotify.RaiseToast(
							"Connect Error",
							$"Entites {sourceCanvasIndex} and {destinationCanvasIndex} are already connected",
							Notification.Wpf.NotificationType.Error);

					}
				}
			}
			else
			{
				// Canvas na koji se postavlja tacka nije zauzet

				isLineSourceSelected = false;

				linePoint1 = new Point();
				linePoint2 = new Point();
				currentLine = new Line();
			}
		}
		private bool DoesLineAlreadyExist(int source, int destination)
		{
			foreach (Line line in LineCollection)
			{
				if ((line.Source == source) && (line.Destination == destination))
				{
					return true;
				}
				if ((line.Source == destination) && (line.Destination == source))
				{
					return true;
				}
			}
			return false;
		}
		private Point GetPointForCanvasIndex(int canvasIndex)
		{
			double x = 0, y = 0;

			for (int row = 0; row < 3; row++)
			{
				for (int col = 0; col < 4; col++)
				{
					int currentIndex = row * 4 + col;

					if (canvasIndex == currentIndex)
					{
						switch(row)
						{
							case 0:
								y = 135;
								break;
							case 1:
								y = 385;
								break;
							case 2:
								y = 640;
								break;
						}

						switch(col)
						{
							case 0:
								x = 130;
								break;
							case 1:
								x = 410;
								break;
							case 2:
								x = 675;
								break;
							case 3:
								x = 950;
								break;
						}
						break;
					}
				}
			}
			return new Point(x, y);
		}

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

		public void OnMouseLeftButtonUp()
		{
			draggedItem = null;
			SelectedEntity = null;
			dragging = false;
			draggingSourceIndex = -1;
		}
		public void OnSelectionChanged(object selectedItem)
		{
			if (!dragging && selectedItem is Entity selectedEntity)
			{
				dragging = true;
				draggedItem = selectedEntity;
				DragDrop.DoDragDrop(Application.Current.MainWindow, draggedItem, DragDropEffects.Move | DragDropEffects.Copy);
			}
		}

		public void OnLeftMouseButtonDown(object entity)
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

		public void OnDrop(object e)
		{
			bool rtd = false;
			if (draggedItem != null)
			{
				int index = Convert.ToInt32(e);

				if (CanvasCollection[index].Resources["taken"] == null)
				{
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


					// Create and initialize list
					List<object> list = new List<object>();

					// Create deep copies of the collections
					ObservableCollection<string> mojCanvasIDCollection = DeepCopyObservableCollection(canvasIDCollection);
					ObservableCollection<string> mojCanvasValueCollection = DeepCopyObservableCollection(CanvasValueCollection);
					ObservableCollection<Canvas> mojCanvasCollection = DeepCopyObservableCollection(CanvasCollection);
					ObservableCollection<Entity> mojEntity = new ObservableCollection<Entity>();

					// Add copies to the list
					list.Add(mojCanvasIDCollection);
					list.Add(mojCanvasValueCollection);
					list.Add(mojCanvasCollection);

					// Add other elements
					if (rtd && draggingSourceIndex == -1)
					{
						list.Add("rtd");
						mojEntity = DeepCopyObservableCollection(RTD_Entities);
					}
					else
					{
						list.Add("termosprega");
						mojEntity = DeepCopyObservableCollection(TermoSprega_Entities);
					}
					list.Add(mojEntity);
					list.Add(index.ToString());

					// Save state
					MainWindowViewModel.Undo = new SaveState<CommandType, object>(CommandType.CanvasManipulation, list);

					// Modify collections
					CanvasIDCollection[index] = draggedItem.Id.ToString();
					CanvasValueCollection[index] = draggedItem.Value.ToString();
					CanvasCollection[index].Background = new ImageBrush(image);
					CanvasCollection[index].Resources.Add("taken", true);
					CanvasCollection[index].Resources.Add("data", draggedItem);

					// PREVLACENJE IZ DRUGOG CANVASA
					if (draggingSourceIndex != -1)
					{
						CanvasCollection[draggingSourceIndex].Background = Brushes.LightGray;
						CanvasCollection[draggingSourceIndex].Resources.Remove("taken");
						CanvasCollection[draggingSourceIndex].Resources.Remove("data");
						CanvasIDCollection[draggingSourceIndex] = "X";
						CanvasValueCollection[draggingSourceIndex] = "X";
						BorderBrushCollection[draggingSourceIndex] = "Black";
						UpdateLinesForCanvas(draggingSourceIndex, index);
						if (sourceCanvasIndex != -1)
						{
							isLineSourceSelected = false;
							sourceCanvasIndex = -1;
							linePoint1 = new Point();
							linePoint2 = new Point();
							currentLine = new Line();
						}
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
			dragging = false;
		}

		public static ObservableCollection<T> DeepCopyObservableCollection<T>(ObservableCollection<T> original)
		{
			return new ObservableCollection<T>(original.Select(item => item));
		}

		private void UpdateLinesForCanvas(int sourceCanvas, int destinationCanvas)
		{
			for (int i = 0; i < LineCollection.Count; i++)
			{
				if (LineCollection[i].Source == sourceCanvas)
				{
					Point newSourcePoint = GetPointForCanvasIndex(destinationCanvas);
					LineCollection[i].X1 = newSourcePoint.X;
					LineCollection[i].Y1 = newSourcePoint.Y;
					LineCollection[i].Source = destinationCanvas;
				}
				else if (LineCollection[i].Destination == sourceCanvas)
				{
					Point newDestinationPoint = GetPointForCanvasIndex(destinationCanvas);
					LineCollection[i].X2 = newDestinationPoint.X;
					LineCollection[i].Y2 = newDestinationPoint.Y;
					LineCollection[i].Destination = destinationCanvas;
				}
			}
		}

		private void DeleteLinesForCanvas(int canvasIndex)
		{
			List<Line> linesToDelete = new List<Line>();

			for (int i = 0; i < LineCollection.Count; i++)
			{
				if ((LineCollection[i].Source == canvasIndex) || (LineCollection[i].Destination == canvasIndex))
				{
					linesToDelete.Add(LineCollection[i]);
				}
			}

			foreach (Line line in linesToDelete)
			{
				LineCollection.Remove(line);
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
		public void OnFreeUpCanvas(object parameter)
		{
			int index = Convert.ToInt32(parameter);

			if (CanvasCollection[index].Resources["taken"] != null)
			{
				Entity tmpEntity = (Entity)CanvasCollection[index].Resources["data"];
				if (tmpEntity.Type.ToString().Equals("RTD"))
					RTD_Entities.Add(tmpEntity);
				else
					TermoSprega_Entities.Add(tmpEntity);

				CanvasCollection[index].Background = Brushes.LightGray;
				CanvasCollection[index].Resources.Remove("taken");
				CanvasCollection[index].Resources.Remove("data");
			}
		}

	}
}
