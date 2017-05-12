using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Logica di interazione per ServerElement.xaml
    /// </summary>
    public partial class ServerElement : UserControl, INotifyPropertyChanged
    {
        private MainWindow _parent;
        private Server _srv;
        private Uri _uriRetrieving;
        private Uri _uriConnected;
        private Uri _uriDisconnected;
        private Uri _uriDisconnectedBadly;
        private Uri _uriDisconnectedByUs;
        private Uri _uriLostConnection;
        private Dictionary<int, OpenWindow> _openWindows;   //(int)windowID, OpenWindow
        private List<OpenWindow> _sortedWindowList; //lista da cui prenderò gli elementi da visualizzare nella listbox
        private Thread _receiveThread;
        private object _lock;
        private OpenWindow _currentlyOnFocus;
        private OpenWindow _hasLostFocus;
        private DateTime _lastFocusUpdate;
        private DispatcherTimer _timer;
        private Stopwatch _stopWatch;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> pendingJSONs;

        public OpenWindow CurrentlyOnFocus{
            get { return _currentlyOnFocus; }
            private set { _currentlyOnFocus = value;
                OnPropertyChanged("CurrentlyOnFocus");
            }
        }
        public List<OpenWindow> OpenWindowsValues {
            get { return new List<OpenWindow>(_openWindows.Values); }
        }

        public ServerElement(Server srv)
        {
            InitializeComponent();
            _uriRetrieving = new Uri(@"pack://application:,,,/imgs/32x32_RetrievingList.gif");
            _uriDisconnected = new Uri("pack://application:,,,/imgs/disconnectedGently16x16.png");
            _uriLostConnection = new Uri("pack://application:,,,/imgs/lostConnection16x16.png");
            _uriDisconnectedBadly = new Uri("pack://application:,,,/imgs/disconnectedBadly16x16.png");
            _uriConnected = new Uri("pack://application:,,,/imgs/connected16x16.png");
            _uriDisconnectedByUs = new Uri("pack://application:,,,/imgs/disconnectedByUs16x16.png");

            //assegno Server a questo ServerElement
            _srv = srv;
            _srv.SetGUIParentElement(this);
            txtb_serverAddress.Text = _srv.Name();



            _lastFocusUpdate = _srv.ConnectionTime;
            _hasLostFocus = null;
            _currentlyOnFocus = null;
            _openWindows = new Dictionary<int, OpenWindow>();
            _sortedWindowList = new List<OpenWindow>();
            pendingJSONs = new ObservableCollection<string>();
            _lock = new object();
            Debug.Assert(pendingJSONs != null, "pendingJSONs == NULL");
            Debug.Assert(_lock != null, "_lock is NULL");
            Debug.Assert(_srv != null, "_srv is NULL");
            BindingOperations.EnableCollectionSynchronization(pendingJSONs, _lock);
            pendingJSONs.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            //questi sono i timer per calcolare le percentuali di focus
            _stopWatch = new Stopwatch();   //per calcolare il tempo dalla connessione del server
            _stopWatch.Start();
            _timer = new DispatcherTimer(); //a intervalli regolari aggiorna le percentuali (se esiste focus)
            _timer.Tick += new EventHandler(UpdatePercentages_Tick);
            _timer.Interval = new TimeSpan(0, 0, 2);

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
                //Debug.WriteLine("Items added: ");
                foreach (var item in e.NewItems)
                {
                    //Debug.WriteLine(item);
                    //rimuovo il tooltip
                    if (gif_retrievingList.Visibility == Visibility.Visible)
                    {
                        gif_retrievingList.Visibility = Visibility.Collapsed;
                        listBox_OpenWindows.Visibility = Visibility.Visible;
                        stackp_WindowsList.ClearValue(ToolTipProperty);
                    }
                    //estraggo il primo elemento della lista e lo processo (le aggiunte sono fatte in coda)
                    string element = pendingJSONs.First();
                    try
                    {
                        var win = JsonConvert.DeserializeObject<OpenWindow>(pendingJSONs.First());
                        Debug.Assert(win != null, "win == NULL");
                        var winID = win.WindowID;
                        if (_openWindows.ContainsKey(winID))   //la finestra era già stata aperta in precedenza, ora cambia solo stato
                        {
                            OpenWindow winToUpdate;
                            if(_openWindows.TryGetValue(winID, out winToUpdate))
                            {   //estraggo la OpenWindow e le assegno il nuovo stato
                                winToUpdate.Status = win.Status;
                            }
                            if (winToUpdate.HasFocus()) {
                                Debug.WriteLine(" has focus now", winToUpdate.WinName);
                                if (CurrentlyOnFocus != null) {
                                    Debug.WriteLine(" has lost focus", CurrentlyOnFocus.WinName);
                                    CurrentlyOnFocus.Status = OpenWindow.UpdateType.HasLostFocus;
                                    _hasLostFocus = _currentlyOnFocus;
                                    //tolgo questo serverElement dalla lista di serverElement con WinName in focus
                                }
                                CurrentlyOnFocus = winToUpdate;
                            }
                            if (winToUpdate.Status == OpenWindow.UpdateType.Closed)
                            {
                                listBox_OpenWindows.Items.Remove(winToUpdate);
                                _openWindows.Remove(winToUpdate.WindowID);
                                if (CurrentlyOnFocus.WindowID == winToUpdate.WindowID) {  //la finestra che sto chiudendo era currently on focus
                                    CurrentlyOnFocus = null;
                                }
                                //la rimuovo dalla lista generale dei processi attivi
                                RemoveFromAllProcessesList(winToUpdate.ProcName);
                            }
                            if (winToUpdate.Status == OpenWindow.UpdateType.HasLostFocus) {  //La finestra è stata minimizzata, ad esempio, o l'utente è sul desktop
                                CurrentlyOnFocus = null;
                                _hasLostFocus = winToUpdate;
                                
                            }
                        }
                        else {  //la finestra non esisteva ancora, quindi creo l'oggetto
                            if (win.Status == OpenWindow.UpdateType.NewWindow)
                            {
                                win.Initialize();
                                _openWindows.Add(win.WindowID, win);
                                listBox_OpenWindows.Items.Add(win);
                                //Debug.WriteLine("aggiunto l'item win");
                                AddToAllProcessesList(win.ProcName);
                            }
                            else
                            {
                                MessageBox.Show("Error in CollectionChanged: the item to be added is not a NewWindow event.", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }

                        if (listBox_OpenWindows.Visibility == Visibility.Collapsed)
                        {
                            listBox_OpenWindows.Visibility = Visibility.Visible;
                            if (win.Status == OpenWindow.UpdateType.NewWindow)
                            {
                                _openWindows.Add(win.WindowID, win);
                            }
                        }
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
            {/*
                Debug.WriteLine("Items removed: ");
                foreach (var item in e.OldItems)
                {
                    Debug.WriteLine(item);
                }*/
            }
        }

        public void SocketStatusChanged(string value)
        {
            if (value.Equals("ObjectDisposedException") || value.Equals("IOException"))
            {
                //il socket si è chiuso inaspettatamente
                img_ConnectionStatus.Source = new BitmapImage(_uriLostConnection);
                img_ConnectionStatus.ToolTip = "The connection was lost unespectedly.";
                foreach (var openWin in _openWindows.Values)
                {
                    openWin.ConvertToGrayscale();
                    stackp_WindowsList.ToolTip = "The connection was lost unespectedly.";
                }
                mitem_disconnect.Visibility = Visibility.Collapsed;
                if(mitem_reconnect.IsEnabled)
                    mitem_reconnect.IsEnabled = false;
                mitem_close.Visibility = Visibility.Visible;
                _stopWatch.Stop();
                _timer.Stop();
                if (_openWindows.Values.Count != 0)
                {
                    //tolgo questo ServerElement dalle liste di ciascun processo, nel dizionario del main
                    foreach (var v in _openWindows.Values)
                        RemoveFromAllProcessesList(v.ProcName);
                }
            }
            else if (value.Equals("SocketGentlyDisposed"))
            {
                //il socket ha chiuso la connessione in modo corretto
                //trasformo le finestre da colori in grayscale per far capire che la connessione è andata
                img_ConnectionStatus.Source = new BitmapImage(_uriDisconnected);
                img_ConnectionStatus.ToolTip = "The counterpart closed the connection (gently).";
                foreach (var openWin in _openWindows.Values)
                {
                    openWin.ConvertToGrayscale();
                }
                stackp_WindowsList.ToolTip = "The counterpart closed the connection (gently).";
                mitem_disconnect.Visibility = Visibility.Collapsed;
                if (mitem_reconnect.IsEnabled)
                    mitem_reconnect.IsEnabled = false;
                mitem_close.Visibility = Visibility.Visible;
                _stopWatch.Stop();
                _timer.Stop();
                if (_openWindows.Values.Count != 0)
                {
                    //tolgo questo ServerElement dalle liste di ciascun processo, nel dizionario del main
                    foreach (var v in _openWindows.Values)
                        RemoveFromAllProcessesList(v.ProcName);
                }
            }
            else if (value.Equals("Connected"))
            {
                img_ConnectionStatus.Source = new BitmapImage(_uriConnected);
                if (_openWindows.Count != 0)
                {
                    _openWindows.Clear();
                    listBox_OpenWindows.Items.Clear();
                }
                if(mitem_disconnect.Visibility == Visibility.Collapsed)
                    mitem_disconnect.Visibility = Visibility.Visible;
                if (mitem_reconnect.IsEnabled)
                    mitem_reconnect.IsEnabled = false;
                if(mitem_close.Visibility == Visibility.Visible)
                    mitem_close.Visibility = Visibility.Collapsed;
                _stopWatch.Start(); //faccio ripartire il timer del tempo
                _timer.Start();
            }
            else if (value.Equals("ReconnectionLimitExceeded"))
            {
                //il # di riconnessioni è stato superato
                img_ConnectionStatus.Source = new BitmapImage(_uriDisconnectedBadly);
                img_ConnectionStatus.ToolTip = "Reconnection limit exceeded: no answer from the counterpart.";
                foreach (var openWin in _openWindows.Values)
                {
                    openWin.ConvertToGrayscale();
                    stackp_WindowsList.ToolTip = "Reconnection limit exceeded: no answer from the counterpart.";
                }
                mitem_disconnect.Visibility = Visibility.Collapsed;
                mitem_reconnect.IsEnabled = true;
                mitem_close.Visibility = Visibility.Visible;
                _stopWatch.Stop();
                _timer.Stop();
                if (_openWindows.Values.Count != 0)
                {
                    //tolgo questo ServerElement dalle liste di ciascun processo, nel dizionario del main
                    foreach (var v in _openWindows.Values)
                        RemoveFromAllProcessesList(v.ProcName);
                }
            }
            else if (value.Equals("SocketClosedByUs") || value.Equals("GenericException")) {
                //abbiamo ricevuto un json con dimensione brutta, chiuso il socket
                //mettiamo una icona con ! e lasciamo all'utente la possibilità di riconnettersi
                img_ConnectionStatus.Source = new BitmapImage(_uriDisconnectedByUs);
                mitem_disconnect.Visibility = Visibility.Collapsed;
                mitem_reconnect.IsEnabled = true;
                mitem_close.Visibility = Visibility.Visible;
                _stopWatch.Stop(); //fermo il timer del tempo di connessione
                _timer.Stop();
                if (_openWindows.Values.Count != 0)
                {
                    //tolgo questo ServerElement dalle liste di ciascun processo, nel dizionario del main
                    foreach (var v in _openWindows.Values)
                    {
                        RemoveFromAllProcessesList(v.ProcName);
                        v.ConvertToGrayscale();
                        if(value.Equals("SocketClosedByUs"))
                            stackp_WindowsList.ToolTip = "Socket was closed due to an exception on the json length.";
                        else
                            stackp_WindowsList.ToolTip = "Socket was closed due to a generic exception. See debug for more details.";
                    }
                }
            }
        }

        public void RemoveFromAllProcessesList(string procName)
        {
            //tolgo questo serverElement dalla lista di serverElement corrispondente alla coppia <K,V> con K == procName
            if (_parent.AllProcesses.ContainsKey(procName))
            {
                ObservableCollection<ServerElement> values;
                if (_parent.AllProcesses.TryGetValue(procName, out values))
                {/*
                    if (values.Count == 1) //questo era l'ultimo elemento della catena, quindi elimino proprio la voce
                    {
                        //tolgo il listener per le modifiche alla lista
                        values.CollectionChanged -= _parent.AllProcessesCollectionChanged;
                        _parent.AllProcesses.Remove(procName);
                    }
                    else*/
                    if(values.Contains(this)) values.Remove(this);
                }
            }
        }

        public void AddToAllProcessesList(string procName)
        {
            ObservableCollection<ServerElement> values;
            if (_parent.AllProcesses.ContainsKey(procName))
            {   // è già presente almeno 1 ServerElement con questo processo in esecuzione
                if (_parent.AllProcesses.TryGetValue(procName, out values))
                {   //aggiungo questo server alla lista solo se non è già presente (nel caso ci fossero due finestre che si riferiscono
                    //allo stesso processo
                    Debug.Assert(values != null);
                    if(!values.Contains(this))
                        values.Add(this);
                }
            }
            else //non c'era ancora nessun ServeElement con questo processo in esecuzione, quindi lo aggiungo al dizionario
            {
                values = new ObservableCollection<ServerElement>();
                values.Add(this);
                values.CollectionChanged += new NotifyCollectionChangedEventHandler(_parent.AllProcessesCollectionChanged);
                _parent.AllProcesses.Add(procName, values);
            }
        }

        public void SetParent(MainWindow parent)
        {
            _parent = parent;
            _parent.PropertyChanged += this.OnPropertyChanged;
            //Debug.WriteLine("SERVER_ELEMENT: subscribed to MAIN_WINDOW's property changed");
        }

        private void UpdatePercentages_Tick(object sender, EventArgs e)
        {
            RecomputeFocusPercentage();
            if (_openWindows.Count != 0)
            {
                listBox_OpenWindows.Items.Clear();
                int i = 0;
                if (_openWindows.Count > 1)
                {
                    if (CurrentlyOnFocus != null)
                    {
                        listBox_OpenWindows.Items.Add(_currentlyOnFocus);
                        _openWindows.Remove(_currentlyOnFocus.WindowID);
                        i = 1;
                        //Debug.WriteLine(i + ": " + CurrentlyOnFocus.WindowID + " " + _currentlyOnFocus.FocusTime.Milliseconds);
                    }
                    _sortedWindowList = _openWindows.Values.OrderByDescending(x => x.FocusTime).ToList(); //ordino per focustime
                    if (CurrentlyOnFocus != null) _openWindows.Add(CurrentlyOnFocus.WindowID, CurrentlyOnFocus);
                }
                else _sortedWindowList = _openWindows.Values.ToList();  //ho un solo elemento, è inutile fare il sort

                //Debug.WriteLine("listBox.Count = " + listBox_OpenWindows.Items.Count);

                foreach (var w in _sortedWindowList)
                {
                    //Debug.WriteLine(i + ": " + w.WindowID + " " + w.FocusTime.Milliseconds);
                    listBox_OpenWindows.Items.Insert(i, w);
                    i++;
                }
            }
        }

        private void RecomputeFocusPercentage() {
            //DateTime now = DateTime.Now;  //ottengo l'ora attuale
            TimeSpan focusTimeOfAll = TimeSpan.Zero;
            
            if (_hasLostFocus != null) {
                //ottengo TimeSpan da Now-TempoDiConnessione, con l'uguale in realtà aggiungo
                //_hasLostFocus.FocusTime = now.Subtract(_lastFocusUpdate);
                //Debug.WriteLine("_hasLostFocus.FocusTime: {0}", _hasLostFocus.FocusTime.ToString(@"ss\.fff"));
            }
            //_lastFocusUpdate = DateTime.Now;

            if (_openWindows.Count > 0)
            {
                //ricalcolo tutte le percentuali
                foreach (var item in _openWindows.Values)
                {
                    focusTimeOfAll = focusTimeOfAll.Add(item.FocusTime);
                }
                foreach (var item in _openWindows.Values) {
                    item.ComputeFocusPercentage(focusTimeOfAll);
                }
            }
        }

        //Collegato al metodo OnPropertyChanged della MainWindow per ascoltare quando viene premuto disconnectAll
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("DisconnectAll"))
            {   //bottone premuto, disconnetto questo server
                Debug.WriteLine("SERVER_ELEMENT: OnPropertyChanged stopping the servers");
                _srv.StopReceive();
                _srv.Close();
                _parent.ServerList.Remove(this);
            }
        }

        /* Quando una finestra acquisisce il focus o una lo perde, vengono fermati i rispettivi timer che accumulano
         * il tempo di permanenza in focus.
         * Su questo evento si registra anche la MainWindow per aggiornare la lista di processi in focus
         */
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName.Equals("CurrentlyOnFocus"))
            {
                if (CurrentlyOnFocus == null)
                {
                    _timer.Stop();
#if(DEBUG)
                    Debug.WriteLine("_timer has been stopped.");
#endif
                }
                else {
                    _timer.Start();
#if(DEBUG)
                    Debug.WriteLine("_timer has been restarted.");
#endif
                }
            }
        }

        public void SendKeyCombo(string json)
        {
            Task.Run(() => _srv.Send(json));
            //le eventuali eccezioni scatenate dalla send vengono notificate e gestite tramite SocketStatusChanged e Server.cs
        }

        private void mitem_disconnect_Click(object sender, RoutedEventArgs e)
        {
            if(_timer.IsEnabled)
                _timer.Stop();
            if (_stopWatch.IsRunning)
                _stopWatch.Stop();
            if (_openWindows.Values.Count != 0)
            {
                //tolgo questo ServerElement dalle liste di ciascun processo, nel dizionario del main
                foreach (var v in _openWindows.Values)
                    RemoveFromAllProcessesList(v.ProcName);
            }
            Debug.WriteLine("Receive stopped.");
            _srv.StopReceive();
            Debug.WriteLine("Thread joined.");
            _srv.Close();
            Debug.WriteLine("_srv closed.");
            //rimuove il server element dalla lista dei server
            //anche se meglio avere un listener nel main
            _parent.ServerList.Remove(this);
        }

        private async void mitem_reconnect_Click(object sender, RoutedEventArgs e)
        {
            listBox_OpenWindows.Visibility = Visibility.Collapsed;
            gif_retrievingList.Visibility = Visibility.Visible;
            stackp_WindowsList.ToolTip = Msg_RetrievingList(); 
            try
            {
                ImageBehavior.SetAnimatedSource(gif_retrievingList, new BitmapImage(_uriRetrieving));
            }
            catch (System.IO.IOException)
            {   //se non riesce a caricare l'img, l'img viene collassata
                gif_retrievingList.Visibility = Visibility.Collapsed;
            }
            String result = await Task.Run(() => _srv.Startup());
            if (result.Equals("Connected"))
            {
                _receiveThread = new Thread(_srv.Receive);
                _receiveThread.Start();
                SocketStatusChanged("Connected");
            }
        }

        private void mitem_close_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled) _timer.Stop();
            if (_stopWatch.IsRunning) _stopWatch.Stop();
            _parent.ServerList.Remove(this);
        }

        private string Msg_RetrievingList()
        {
            return "Downloading the list of windows currently open";
        }
    }
}
