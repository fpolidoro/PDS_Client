using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        private LinkedList<string> _serverAddressList;
        public ObservableCollection<ServerElement> ServerList {
            get;
            set;
        }
        
        public MainWindow()
        {
            ServerList = new ObservableCollection<ServerElement>();
            _serverAddressList = new LinkedList<string>();
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
            Window newConnWin = new NewConnectionDialog(out srv, this);
            newConnWin.ShowDialog();//showDialog fa in modo che la finestra sia modale
            if (srv.IsValid)
            {
                if (_serverAddressList.Contains(srv.GetAddress()))
                {
                    MessageBox.Show(MsgServerAlreadyExisting(srv.GetAddress()), "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else {
                    var serverWin = new ServerElement(srv);
                    serverWin.SetParent(this);
                    //serverWin.Srv = srv;
                    _serverAddressList.AddLast(srv.GetAddress());
                    ServerList.Add(serverWin);
                }
            }
        }

        public void UpdateStatusBar(string status) {
            lbl_status.Content = status;
        }

        
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null) {
                _serverAddressList.RemoveLast();
            }                   
        }
        /*
        private void ServerItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            IsDirty = true;
        }*/

        private string MsgServerAlreadyExisting(string ip)
        {
            return "Server " + ip + " already exists.";
        }
    }

}
