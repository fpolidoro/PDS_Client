﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;   //per dire ai server element che è stato premuto disconnect all

        //private Boolean IsDirty;
        private LinkedList<string> _serverAddressList;
        private Boolean _disconnectAll;
        public Boolean DisconnectAll
        {
            set
            {
                _disconnectAll = value;
                if (value)
                {
                    Debug.WriteLine("MAIN_WINDOW: disconnectAll changed to TRUE");
                    OnPropertyChanged("DisconnectAll");
                }
            }
        }

        public ObservableCollection<ServerElement> ServerList {
            get;
            set;
        }

        private int _cols;
        public int Cols {
            get { return _cols; }
        }
        
        public MainWindow()
        {
            ServerList = new ObservableCollection<ServerElement>();
            _serverAddressList = new LinkedList<string>();
            InitializeComponent();
            //ItemsSource = "{Binding Source=ServerList}"
            ic_serverElements.ItemsSource = ServerList;
            ServerList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChanged);
            DisconnectAll = false;
            _cols = 0;
        }

        private void btn_newConnection_Click(object sender, RoutedEventArgs e)
        {
            Server srv;
            Window newConnWin = new NewConnectionDialog(out srv, this, _serverAddressList);
            newConnWin.ShowDialog();//showDialog fa in modo che la finestra sia modale
            if (srv != null && srv.IsValid)
            {
                var serverWin = new ServerElement(srv);
                serverWin.SetParent(this);
                 //serverWin.Srv = srv;
                _serverAddressList.AddLast(srv.GetAddress());
                ServerList.Add(serverWin);
            }
        }

        private void btn_disconnectAll_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("disconnectAll is TRUE");
            DisconnectAll = true;
        }

        public void UpdateStatusBar(string status) {
            //lbl_status.Content = status;
        }

        //Gestore evento di chiusura [X] della MainWindow
        public void OnClose(object sender, CancelEventArgs e)
        {
            if(ServerList.Count != 0)
            {
                DisconnectAll = true;
                //qua credo che la finestra si chiuda, ma i thread dei server continuino
                //nella chiusura anche dopo che la finestra è diventata invisibile
            }
        }

        //PropertyChanged per l'evento click sul bottone disconnetti tutti.
        //Ciascun server element si registra qua e ascolta quando viene fatto click
        protected virtual void OnPropertyChanged(string propertyName)
        {
            Debug.WriteLine("MAIN_WINDOW: OnPropertyChanged");
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            DisconnectAll = false; //riporto la proprietà a false, perchè ormai l'evento è andato
        }


        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null) {
                _serverAddressList.RemoveLast();
            }
            /*if (ServerList.Count > 6) {
                _cols = 4;
                decimal val = ServerList.Count / 4;
                _rows = (int)Math.Ceiling(val);
            }
            else if (ServerList.Count >= 3 && ServerList.Count <= 6) {
                _cols = 3;
                _rows = 2;
            }
            else if (ServerList.Count <= 2)
            {
                _cols = 2;
                _rows = 1;
            }*/
        }

        //Evento scatenato quando si fa il resize della finestra principale
        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ServerList.Count != 0 && ServerList.Count > 3) {
                Debug.WriteLine("width = {0} srv.Count = {1}", stackp_serverGrid.ActualWidth, ServerList.Count);
                _cols = (int)Math.Floor(stackp_serverGrid.ActualWidth / (ServerList.Count*200));
                Debug.WriteLine("_cols = {0}", _cols);
            }
        }

        private void btn_sendKeyCombo_Click(object sender, RoutedEventArgs e)
        {
            tabItem_KeyCombos.Visibility = Visibility.Visible;
            tabControl.SelectedIndex = 2;
        }

        private void btn_sendKey_Click(object sender, RoutedEventArgs e) {

        }
    }

}
