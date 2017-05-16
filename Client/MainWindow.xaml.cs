#define DEBUG

using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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

        //collezione thread-safe, perchè anche i ServerElement accedono in scrittura
        public AsyncObservableCollection<ServerElement> ServerList
        {
            get;
            set;
        }


        //dizionario thread-safe, Key=nomeProcessoInFocus, value=lista dei server con quel processo attualmente in focus
        public ObservableConcurrentDictionary<string, ObservableCollection<ServerElement>> AllProcesses
        {
            get;
            set;
        }

        public ObservableCollection<ElementToShow> WindowsToShow
        {
            get;
            set;
        }

        private int _cols;


        public int Cols
        {
            get { return _cols; }
        }

        public MainWindow()
        {
            ServerList = new AsyncObservableCollection<ServerElement>();
            _serverAddressList = new LinkedList<string>();
            AllProcesses = new ObservableConcurrentDictionary<string, ObservableCollection<ServerElement>>();
            WindowsToShow = new ObservableCollection<ElementToShow>();
            InitializeComponent();
            ic_serverElements.ItemsSource = ServerList;
            ServerList.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            AllProcesses.CollectionChanged += new NotifyCollectionChangedEventHandler(AllProcessesCollectionChanged);
            DisconnectAll = false;
            _cols = 0;
            listBoxWButtons_activeProcesses.SetParent(this);
            cktxt_getKeyCombo.SetParent(this);
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

        public void UpdateStatusBar(string status)
        {
            //lbl_status.Content = status;
        }

        //Gestore evento di chiusura [X] della MainWindow
        public void OnClose(object sender, CancelEventArgs e)
        {
            if (ServerList.Count != 0)
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
            if (args.OldItems != null)
            {
                _serverAddressList.RemoveLast();
            }
        }

        /*  Dizionario accessibile a tutti i server element. Ciascuno aggiunge/rimuove se stesso dalla lista di server
         *  su cui è in esecuzione il processo il cui nome è usato come chiave
         */
        public void AllProcessesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<ServerElement> values = new ObservableCollection<ServerElement>();
            foreach (var v in AllProcesses.Keys)
            {
                if (AllProcesses.TryGetValue(v, out values))
                {
                    if (values.Count > 0)//se ho almeno due elementi, aggiungo il nome processo alla lista
                    {
                        //estraggo l'icona
                        var win = values[0].OpenWindowsValues.FirstOrDefault(p => p.ProcName.Equals(v));
                        Debug.Assert(win != null);
                        ElementToShow el = new ElementToShow(v, win.Icona);
                        if (!WindowsToShow.Any(p => p.ProcName.Equals(v)))
                        {
                            var le = new ElementToShow(v, win.Icona);    //necessario perchè non posso aggiungere lo stesso elemento wpf due volte (altrimenti System.InvalidOperationException - Element already has a logical parent)
                            WindowsToShow.Add(el);
                            listBoxWButtons_activeProcesses.listView_focusedProcesses.Items.Add(el);
                            listBoxWButtons_activeProcesses.listBox_showAllProcesses.Items.Add(le);
                        }
                    }
                    else
                    {
                        var el = WindowsToShow.FirstOrDefault(p => p.ProcName.Equals(v));
                        if (el != null)
                        {
                            WindowsToShow.Remove(el);
                            listBoxWButtons_activeProcesses.listView_focusedProcesses.Items.Remove(el);
                            foreach (ElementToShow ets in listBoxWButtons_activeProcesses.listBox_showAllProcesses.Items)
                                if (ets.ProcName.Equals(el.ProcName))
                                {
                                    listBoxWButtons_activeProcesses.listBox_showAllProcesses.Items.Remove(ets);
                                    break;
                                }
                        }
                    }
                }
            }
#if (DEBUG)
            Debug.WriteLine("WindowsToShow: " + listBoxWButtons_activeProcesses.listView_focusedProcesses.Items.Count);
            foreach (var v in WindowsToShow)
            {
                Debug.WriteLine(" -" + v.ProcName);
            }
#endif
        }


        //Evento scatenato quando si fa il resize della finestra principale
        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ServerList.Count != 0 && ServerList.Count > 3)
            {
#if(DEBUG)
                Debug.WriteLine("width = " + stackp_serverGrid.ActualWidth + " srv.Count = " + ServerList.Count);
#endif
                _cols = (int)Math.Floor(stackp_serverGrid.ActualWidth / (ServerList.Count * 200));
#if(DEBUG)
                Debug.WriteLine("_cols = {0}", _cols);
#endif
            }
        }

        public void Send(List<int> keys)
        {
            //ottenere il nome processo selezionato, ottenere la lista di ServerElement con quel processo
            //attivo
            string process = listBoxWButtons_activeProcesses.SelectedProcess;
            if (process != null)
            {
                var kmsg = new KeyMessage(process, keys.Count, keys.ToArray());
                string json = JsonConvert.SerializeObject(kmsg);

                //controllo che string != null, altrimenti mando un msgBox di errore
                //se string != null, per ciascun ServerElement chiamo la sendJson passandole questo Json
                if (json == null)
                {
                    MessageBox.Show("Error in serializing key combo into JSON", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ObservableCollection<ServerElement> servers;
                if (!AllProcesses.TryGetValue(process, out servers))
                {
                    MessageBox.Show("Error in retrieving list of servers", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Debug.Assert(servers != null, "list 'servers' is NULL!");
                foreach (ServerElement v in servers)
                {
                    v.SendKeyCombo(keys);
                }
            }
            else MessageBox.Show("Occorre selezionare un processo per poter inviare la combinazione di tasti", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);

            return;
        }

        private void btn_sendKeyCombo_Click(object sender, RoutedEventArgs e)
        {
            tabItem_KeyCombos.Visibility = Visibility.Visible;
            tabControl.SelectedIndex = 2;
        }

        private void btn_sendSpecialKey_Click(object sender, RoutedEventArgs e)
        {
            List<int> keyCodes;
            var dialog = new SpecialKeyComboDialog(out keyCodes);
            dialog.ShowDialog();
            if (keyCodes.Count == 0) return;    //ho chiuso la finestra con [X]
#if (DEBUG)
            Debug.WriteLine("MAIN - keys:");
            foreach (var v in keyCodes)
                Debug.WriteLine(v);
#endif
            Send(keyCodes);
        }

        private void btn_clean_Click(object sender, RoutedEventArgs e)
        {
            cktxt_getKeyCombo.CleanValue();
        }

        private void btn_sendALTF4_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add((int)Key.LeftAlt);
            keys.Add((int)Key.F4);

            Send(keys);
        }

        private void btn_sendAltTab_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add((int)Key.LeftAlt);
            keys.Add((int)Key.Tab);

            Send(keys);
        }

        private void btn_sendAltTabRight_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add((int)Key.LeftAlt);
            keys.Add((int)Key.Tab);
            keys.Add((int)Key.Right);

            Send(keys);
        }

        private void btn_sendAltTabLeft_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add((int)Key.LeftAlt);
            keys.Add((int)Key.Tab);
            keys.Add((int)Key.Left);

            Send(keys);
        }
    }
}
