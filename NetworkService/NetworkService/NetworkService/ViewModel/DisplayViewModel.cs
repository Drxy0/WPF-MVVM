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

namespace NetworkService.ViewModel
{
	public class DisplayViewModel : BindableBase
	{
		public static ObservableCollection<Entity> RTD_Entities { get; set; } = MainWindowViewModel.RTD_Entities;
		public static ObservableCollection<Entity> TermoSprega_Entities { get; set; } = MainWindowViewModel.TermoSprega_Entities;
		public static ObservableCollection<Entity> entities { get; set; } = MainWindowViewModel.Entities;
		public ObservableCollection<Canvas> CanvasCollection { get; set; }

		public MyICommand<object> SelectionChanged_TreeView { get; set; }
		public MyICommand MouseLeftButtonUp_TreeView { get; set; }
		public MyICommand<object> DropEntityOnCanvas { get; set; }
		public MyICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
		public MyICommand MouseLeftButtonUpCanvas { get; set; }
		public MyICommand<object> RightMouseButtonDownOnCanvas { get; set; }
		public MyICommand<object> FreeUpCanvas { get; set; }

		private Entity selectedEntity;

		private Entity draggedItem = null;
		private bool dragging = false;
		public int draggingSourceIndex = -1;

		private bool isLineSourceSelected = false;
		private int sourceCanvasIndex = -1;
		private int destinationCanvasIndex = -1;

		public Entity SelectedEntity { get { return selectedEntity; } set { selectedEntity = value; OnPropertyChanged("SelectedEntity"); } }

		public DisplayViewModel()
		{
			InitializeCanvases();

			MouseLeftButtonUp_TreeView = new MyICommand(OnMouseLeftButtonUp);
			SelectionChanged_TreeView = new MyICommand<object>(OnSelectionChanged);
			DropEntityOnCanvas = new MyICommand<object>(OnDrop);
			MouseLeftButtonUpCanvas = new MyICommand(OnMouseLeftButtonUp);
			LeftMouseButtonDownOnCanvas = new MyICommand<object>(OnLeftMouseButtonDown);
			FreeUpCanvas = new MyICommand<object>(OnFreeUpCanvas);

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
			bool termoSprega = false;
			if (draggedItem != null)
			{
				int index = Convert.ToInt32(entity);

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
						termoSprega = true;
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
				//EntitiesInList.Add((Entity)CanvasCollection[index].Resources["data"]);
				CanvasCollection[index].Background = Brushes.LightGray;
				CanvasCollection[index].Resources.Remove("taken");
				CanvasCollection[index].Resources.Remove("data");
			}
		}

	}
}
