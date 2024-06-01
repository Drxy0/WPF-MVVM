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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
	{
        private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<Window> CloseWindowCommand { get; private set; }
		public static ObservableCollection<Entity> Entities { get; set; } = new ObservableCollection<Entity>();
		public static ObservableCollection<Entity> RTD_Entities { get; set; } = new ObservableCollection<Entity>();
		public static ObservableCollection<Entity> TermoSprega_Entities { get; set; } = new ObservableCollection<Entity>();
        public EntitiesViewModel entitiesViewModel;
        public DisplayViewModel displayViewModel;
        public GraphViewModel graphViewModel;
        private BindableBase currentViewModel;
		private NotificationManager notificationManager;

		public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom

			notificationManager = new NotificationManager();

			NavCommand = new MyICommand<string>(OnNav);
			CloseWindowCommand = new MyICommand<Window>(CloseWindow);

			Entities = new ObservableCollection<Entity>();

			entitiesViewModel = new EntitiesViewModel();
            displayViewModel = new DisplayViewModel();
            graphViewModel = new GraphViewModel();
            CurrentViewModel = entitiesViewModel;

			Messenger.Default.Register<Entity>(this, AddToList);
			Messenger.Default.Register<NotificationContent>(this, ShowToastNotification);

		}

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
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

		public BindableBase CurrentViewModel
		{
			get { return currentViewModel; }
			set
			{
				SetProperty(ref currentViewModel, value);
			}
		}
		private void OnNav(string destination)
		{
			switch (destination)
			{
				case "entities":
					CurrentViewModel = entitiesViewModel;
					break;
				case "display":
					CurrentViewModel = displayViewModel;
					break;
				case "graph":
					CurrentViewModel = graphViewModel;
					break;
			}
		}
		private void ShowToastNotification(NotificationContent notificationContent)
		{
			notificationManager.Show(notificationContent, "WindowNotificationArea");
		}

		private void AddToList(Entity entity)
		{
			Entities.Insert(0, entity);
		}
		private void CloseWindow(Window MainWindow)
		{
			MainWindow.Close();
		}
	}
}
