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
		private static int first_index = 0;
		
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
		public static ObservableCollection<string> CanvasIDCollection { get { return canvasIDCollection; } set { canvasIDCollection = value; } }
		public static ObservableCollection<string> CanvasValueCollection { get { return canvasValueCollection; } set { canvasValueCollection = value; } }
		public static ObservableCollection<string> BorderBrushCollection { get { return borderBrushCollection; } set { borderBrushCollection = value; } }
		public static ObservableCollection<Entity> RTD_Entities { get; set; } = MainWindowViewModel.RTD_Entities;
		public static ObservableCollection<Entity> TermoSprega_Entities { get; set; } = MainWindowViewModel.TermoSprega_Entities;
		public static ObservableCollection<Entity> entities { get; set; } = MainWindowViewModel.Entities;
		public static ObservableCollection<Canvas> CanvasCollection { get; set; }

		public MyICommand<object> SelectionChanged_TreeView { get; set; }
		public MyICommand MouseLeftButtonUp_TreeView { get; set; }
		public MyICommand<object> DropEntityOnCanvas { get; set; }
		public MyICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
		public MyICommand MouseLeftButtonUpCanvas { get; set; }
		public MyICommand<object> RightMouseButtonUpOnCanvas { get; set; }
		public MyICommand<object> FreeUpCanvas { get; set; }
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

		private void OnMouseRightButtonUp(object entity)
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

			for (int row = 0; row <= 4; row++)
			{
				for (int col = 0; col <= 3; col++)
				{
					int currentIndex = row * 4 + col;

					if (canvasIndex == currentIndex)
					{
						x = 130 + (col * 277);
						y = 120 + (row * 268);
						break;
					}
				}
			}
			return new Point(x, y);
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

					// PREVLACENJE IZ DRUGOG CANVASA
					if (draggingSourceIndex != -1)
					{
						CanvasCollection[draggingSourceIndex].Background = Brushes.LightGray;
						CanvasCollection[draggingSourceIndex].Resources.Remove("taken");
						CanvasCollection[draggingSourceIndex].Resources.Remove("data");
						CanvasIDCollection[draggingSourceIndex] = "X";
						CanvasValueCollection[draggingSourceIndex] = "X";

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

				CanvasCollection[index].Background = Brushes.LightGray;
				CanvasCollection[index].Resources.Remove("taken");
				CanvasCollection[index].Resources.Remove("data");
			}
		}

	}
}
