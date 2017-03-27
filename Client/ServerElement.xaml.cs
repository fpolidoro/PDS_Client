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
    public partial class ServerElement : UserControl/*, INotifyPropertyChanged*/
    {
        private MainWindow _parent;
        private Server _srv;
        private Uri _uriRetrieving;
        private LinkedList<OpenWindow> _openWindows;
        //private static int _RECLIMIT = 3;
        //private int _reconnAttempts;
        private Thread _receiveThread;
        private object _lock;

        /*public Server Srv{
            get { return _srv; }
            set
            {
                _srv = value;
                _srv.SetGUIParentElement(this);
                txtb_serverAddress.Text = _srv.Name();
            }
        }*/

        public ObservableCollection<string> pendingJSONs;

        public ServerElement(Server srv)
        {
            InitializeComponent();
            _uriRetrieving = new Uri(@"pack://application:,,,/imgs/32x32_RetrievingList.gif");
            //qui devo avviare il task asincrono che gestisca la receive o comunque l'ascolto
            //_reconnAttempts = 0;

            //assegno Server a questo ServerElement
            _srv = srv;
            _srv.SetGUIParentElement(this);
            txtb_serverAddress.Text = _srv.Name();

            _openWindows = new LinkedList<OpenWindow>();
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
                        OpenWindow win = JsonConvert.DeserializeObject<OpenWindow>(pendingJSONs.First());
                        Debug.Assert(win != null, "win == NULL");
                        win.Initialize();
                        if (win.HasFocus())
                        {   //la finestra ha il focus, quindi la metto come prima della lista
                            _openWindows.AddFirst(win);
                            win.Highlight(true);
                            listBox_OpenWindows.Items.Insert(0, win);
                        }
                        else
                        {//DA RIVEDERE: se non è in focus, la devo inserire in base al tempo per cui è stata in focus e non al fondo
                            _openWindows.AddLast(win); //meglio avere una SortedList
                            listBox_OpenWindows.Items.Add(win);
                            Debug.WriteLine("aggiunto l'item win");
                        }

                        if (listBox_OpenWindows.Visibility == Visibility.Collapsed)
                        {
                            listBox_OpenWindows.Visibility = Visibility.Visible;
                            if (win.Status == "NewWindow")
                            {
                                _openWindows.AddLast(win);
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

        /** Property change listener per la ObservableCollection pendingJSONs: per ciascuna aggiunta/rimozione
         *  la UI viene aggiornata
        **/
        //event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        //{
        //    add
        //    {
        //        Debug.Write("La lista è stata modificata dal server.");
        //        //rimuovo il tooltip
        //        gif_retrievingList.Visibility = Visibility.Collapsed;
        //        stackp_WindowsList.ClearValue(ToolTipProperty);
        //        //estraggo il primo elemento della lista e lo processo (le aggiunte sono fatte in coda)
        //        OpenWindow win = JsonConvert.DeserializeObject<OpenWindow>(pendingJSONs.First());
        //        Debug.Assert(win != null, "win == NULL");
        //        win.Initialize();
        //        if (win.HasFocus())
        //        {   //la finestra ha il focus, quindi la metto come prima della lista
        //            _openWindows.AddFirst(win);
        //            listBox_OpenWindows.Items.Add(win);
        //        }
        //        else
        //        {//DA RIVEDERE: se non è in focus, la devo inserire in base al tempo per cui è stata in focus e non al fondo
        //            _openWindows.AddLast(win); //meglio avere una SortedList
        //            listBox_OpenWindows.Items.Add(win);
        //            Debug.WriteLine("aggiunto l'item win");
        //        }

        //        if (listBox_OpenWindows.Visibility == Visibility.Collapsed)
        //        {
        //            listBox_OpenWindows.Visibility = Visibility.Visible;
        //            if (win.Status == "NewWindow")
        //            {
        //                _openWindows.AddLast(win);
        //            }
        //        }
        //        //else if (listBox_OpenWindows.Visibility == Visibility.Visible && _openWindows.Count == 1 /*&& _openWindows.First*/)
        //        {
        //            //se ho un solo elemento nella lista che sta venendo chiuso, collasso la lista, ma va messo un msg tipo "no finestre aperte"
        //            listBox_OpenWindows.Visibility = Visibility.Collapsed;
        //        }
        //    }

        //    remove
        //    {
        //        //non capita nulla perchè man mano che le finestre vengono disegnate, non servono più in questa lista
        //        //throw new NotImplementedException();
        //    }
        //}


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

        /*
        private async void ParseJSONToWindow(string json)
        {
            Debug.WriteLine("siamo in ReadAndParseJSON");
            Debug.Assert(!json.Equals(""), "json == {}");
            Debug.Assert(json != null, "json == NULL");
            Debug.WriteLine(json);
            OpenWindow win = JsonConvert.DeserializeObject<OpenWindow>(json);
            Debug.Assert(win != null, "win == NULL");
            win.Initialize();
            if (win.HasFocus())
            {   //la finestra ha il focus, quindi la metto come prima della lista
                _openWindows.AddFirst(win);
                listBox_OpenWindows.Items.Add(win);
            }
            else
            {//DA RIVEDERE: se non è in focus, la devo inserire in base al tempo per cui è stata in focus e non al fondo
                _openWindows.AddLast(win); //meglio avere una SortedList
                listBox_OpenWindows.Items.Add(win);
                Debug.WriteLine("aggiunto l'item win");
            }

            if (listBox_OpenWindows.Visibility == Visibility.Collapsed)
            {
                listBox_OpenWindows.Visibility = Visibility.Visible;
                if (win.Status == "NewWindow")
                {
                    _openWindows.AddLast(win);
                }
            }
            //else if (listBox_OpenWindows.Visibility == Visibility.Visible && _openWindows.Count == 1 /*&& _openWindows.First*///)
                                                                                                                                /*{
                                                                                                                                    //se ho un solo elemento nella lista che sta venendo chiuso, collasso la lista, ma va messo un msg tipo "no finestre aperte"
                                                                                                                                    listBox_OpenWindows.Visibility = Visibility.Collapsed;
                                                                                                                                }*/
                                                                                                                                //}


        private string Msg_RetrievingList()
        {
            return "Downloading the list of windows currently open";
        }
    }
}
