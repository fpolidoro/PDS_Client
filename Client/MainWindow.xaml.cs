using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        //private Boolean IsDirty;
        public ObservableCollection<ServerElement> ServerList {
            get;
            set;
        }
        
        public MainWindow()
        {
            ServerList = new ObservableCollection<ServerElement>();
            InitializeComponent();
            //ItemsSource = "{Binding Source=ServerList}"
            ic_serverElements.ItemsSource = ServerList;
            ServerList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChanged);
        }

        //per modificare il comportamento del tasto X della finestra principale
        /*protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {

        }*/

        private void btn_newConnection_Click(object sender, RoutedEventArgs e)
        {
            Server srv;
            Window newConnWin = new NewConnectionDialog(out srv);
            newConnWin.ShowDialog();//showDialog fa in modo che la finestra sia modale
            if (srv.IsValid)
            {
                var serverWin = new ServerElement(srv);
                serverWin.SetParent(this);
                //serverWin.Srv = srv;
                ServerList.Add(serverWin);
            }
        }

        
        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
          /*  if (args.OldItems != null)
                foreach (var oldItem in args.OldItems)
                    (oldItem as INotifyPropertyChanged).PropertyChanged -= ServerItem_PropertyChanged;

            if (args.NewItems != null)
                foreach (var newItem in args.NewItems)
                    (newItem as INotifyPropertyChanged).PropertyChanged += ServerItem_PropertyChanged;
        */    
        }
        /*
        private void ServerItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            IsDirty = true;
        }*/
    }
}
