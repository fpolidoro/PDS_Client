using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;
using System.Collections.Specialized;

namespace Client
{
    /// <summary>
    /// Logica di interazione per ServerElement.xaml
    /// </summary>
    public partial class ServerElement : UserControl
    {
        private MainWindow _parent;
        private Server _srv;
        private Uri _uriRetrieving;
        private Uri _uriConnected;
        private Uri _uriDisconnected;
        private Dictionary<int, OpenWindow> _openWindows;
        //private static int _RECLIMIT = 3;
        //private int _reconnAttempts;
        private Thread _receiveThread;
        private object _lock;
        private OpenWindow _currentlyOnFocus;

        public ObservableCollection<string> pendingJSONs;

        public ServerElement(Server srv)
        {
            InitializeComponent();
            _uriRetrieving = new Uri(@"pack://application:,,,/imgs/32x32_RetrievingList.gif");
            _uriDisconnected = new Uri("pack://application:,,,/imgs/16x16_disconnected.png");
            _uriConnected = new Uri("pack://application:,,,/imgs/16x16_Connected.png");
            //qui devo avviare il task asincrono che gestisca la receive o comunque l'ascolto
            //_reconnAttempts = 0;

            //assegno Server a questo ServerElement
            _srv = srv;
            _srv.SetGUIParentElement(this);
            txtb_serverAddress.Text = _srv.Name();

            _openWindows = new Dictionary<int, OpenWindow>();
            pendingJSONs = new ObservableCollection<string>();
            _lock = new object();
            Debug.Assert(pendingJSONs != null, "pendingJSONs == NULL");
            Debug.Assert(_lock != null, "_lock is NULL");
            Debug.Assert(_srv != null, "_srv is NULL");
            BindingOperations.EnableCollectionSynchronization(pendingJSONs, _lock);
            pendingJSONs.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChanged);
            _receiveThread = new Thread(_srv.Receive);
            _receiveThread.Start();
            try
            {
                ImageBehavior.SetAnimatedSource(gif_retrievingList, new BitmapImage(_uriRetrieving));
            }
            catch (System.IO.IOException)
            {   //se non riesce a caricare l'img, l'img viene collassata
                gif_retrievingList.Visibility = Visibility.Collapsed;
            }
            stackp_WindowsList.ToolTip = Msg_RetrievingList();
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //aggiunto un item
            if (e.NewItems != null)
            {
                Debug.WriteLine("Items added: ");
                foreach (var item in e.NewItems)
                {
                    Debug.WriteLine(item);
                    //rimuovo il tooltip
                    gif_retrievingList.Visibility = Visibility.Collapsed;
                    stackp_WindowsList.ClearValue(ToolTipProperty);
                    //estraggo il primo elemento della lista e lo processo (le aggiunte sono fatte in coda)
                    string element = pendingJSONs.First();
                    try
                    {
                        var win = JsonConvert.DeserializeObject<OpenWindow>(pendingJSONs.First());
                        Debug.Assert(win != null, "win == NULL");
                        var winID = Convert.ToInt32(win.WindowID);
                        if (_openWindows.ContainsKey(winID))   //la finestra era già stata aperta in precedenza, ora cambia solo stato
                        {
                            OpenWindow winToUpdate;
                            if(_openWindows.TryGetValue(winID, out winToUpdate))
                            {   //estraggo la OpenWindow e le assegno il nuovo stato
                                winToUpdate.Status = win.Status;
                            }
                            if (winToUpdate.HasFocus()) {
                                Debug.WriteLine(" has focus now", winToUpdate.ProcName);
                                if (_currentlyOnFocus != null) {
                                    Debug.WriteLine(" has lost focus", _currentlyOnFocus.ProcName);
                                    _currentlyOnFocus.Status = "NotOnFocus";
                                    //la finestra non più in focus va messa in lista in base al suo tempo di permanenza in focus
                                }
                                _currentlyOnFocus = winToUpdate;
                                listBox_OpenWindows.Items.Remove(winToUpdate);  //rimuovo dalla posizione attuale
                                listBox_OpenWindows.Items.Insert(0, winToUpdate);   //reinserisco in cima
                            }
                            if (winToUpdate.Status.Equals("Closed"))
                            {
                                listBox_OpenWindows.Items.Remove(winToUpdate);
                            }
                        }
                        else {  //la finestra non esisteva ancora, quindi creo l'oggetto
                            if (win.Status.Equals("NewWindow"))
                            {
                                win.Initialize();
                                _openWindows.Add(win.ID, win);
                                listBox_OpenWindows.Items.Add(win);
                                Debug.WriteLine("aggiunto l'item win");
                            }
                            else
                            {
                                MessageBox.Show("Error in CollectionChanged: the item to be added is not a NewWindow event.", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }

                        if (listBox_OpenWindows.Visibility == Visibility.Collapsed)
                        {
                            listBox_OpenWindows.Visibility = Visibility.Visible;
                            if (win.Status == "NewWindow")
                            {
                                _openWindows.Add(win.ID, win);
                            }
                        }
                        //else if (listBox_OpenWindows.Visibility == Visibility.Visible && _openWindows.Count == 1 /*&& _openWindows.First*/)
                        /*{
                            //se ho un solo elemento nella lista che sta venendo chiuso, collasso la lista, ma va messo un msg tipo "no finestre aperte"
                            listBox_OpenWindows.Visibility = Visibility.Collapsed;
                        }*/
                    }
                    catch (JsonException jsone)
                    {
                        Debug.WriteLine("{0}: l'elemento non sarà visualizzato", jsone.Message);
                    }
                    catch (Exception ex)
                    {
                        //errore nella deserializzazione del json, quindi rimuovo l'elemento dalla lista
                        Debug.WriteLine("{0}: l'elemento non sarà visualizzato", ex.Message);
                    }
                    finally
                    {
                        pendingJSONs.Remove(element);
                    }
                    
                }
            }

            //rimosso un item
            if (e.OldItems != null)
            {
                Debug.WriteLine("Items removed: ");
                foreach (var item in e.OldItems)
                {
                    Debug.WriteLine(item);
                }
            }
        }

        public void SocketStatusChanged(string value)
        {
            if (value.Equals("ObjectDisposedException") || value.Equals("IOException"))
            {
                //il socket si è chiuso inaspettatamente
                img_ConnectionStatus.Source = new BitmapImage(_uriDisconnected);
            }
            else if (value.Equals("SocketGentlyDisposed"))
            {
                //il socket ha chiuso la connessione in modo corretto
                //dovrei trasformare le finestre da colori in grayscale per far capire che la connessione è andata
                img_ConnectionStatus.Source = new BitmapImage(_uriDisconnected);
            }
            else if (value.Equals("Connected"))
            {
                img_ConnectionStatus.Source = new BitmapImage(_uriConnected);
            }
        }


        public void SetParent(MainWindow parent)
        {
            _parent = parent;
        }

        private void mitem_disconnect_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Receive stopped.");
            _srv.StopReceive();
            Debug.WriteLine("Thread joined.");
            _srv.Close();
            Debug.WriteLine("_srv closed.");
            //rimuove il server element dalla lista dei server
            //anche se meglio avere un listener nel main
            _parent.ServerList.Remove(this);
        }

        private string Msg_RetrievingList()
        {
            return "Downloading the list of windows currently open";
        }
    }
}
