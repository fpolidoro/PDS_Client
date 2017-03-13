using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private LinkedList<OpenWindow> _openWindows;
        private static int _RECLIMIT = 3;
        private int _reconnAttempts;

        public Server Srv{
            get { return _srv; }
            set
            {
                _srv = value;
                txtb_serverAddress.Text = _srv.Name();
            }
        }
        
        public ServerElement()
        {
            InitializeComponent();
            _uriRetrieving = new Uri(@"pack://application:,,,/imgs/32x32_RetrievingList.gif");
            //qui devo avviare il task asincrono che gestisca la receive o comunque l'ascolto
            _reconnAttempts = 0;
            _openWindows = new LinkedList<OpenWindow>();
            RetrievingData(_srv);
        }

        public void SetParent(MainWindow parent)
        {
            _parent = parent;
        }

        private void mitem_disconnect_Click(object sender, RoutedEventArgs e)
        {
            _srv.Close();
            //rimuove il server element dalla lista dei server
            //anche se meglio avere un listener nel main
            _parent.ServerList.Remove(this);
        }

        private async void RetrievingData(Server srv)
        {
            //while (true) { //se decommentato, win diventa null dopo il primo ciclo
                if (_openWindows.Count == 0)
                {
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
                else
                {
                    stackp_WindowsList.ClearValue(StackPanel.ToolTipProperty);  //toglie il tooltip
                }
                //creo json e lo passo by reference ad un task a parte che farà la receive
                StringBuilder json = null;
                bool opRes = await Task.Run(() => _srv.Receive(out json));
                Console.WriteLine("uscito da await");
                //controllo il retrieve, se true, allora posso leggere json e creare le entry della lista
                //altrimenti c'è stato un problema, metto grigio questo serverElement ed eventualmente riprovo a connettermi

                if (opRes)
                {
                    //Task.Run(() => ParseJSONToWindow(json.ToString()));
                    gif_retrievingList.Visibility = Visibility.Collapsed;
                    stackp_WindowsList.ClearValue(StackPanel.ToolTipProperty);
                    Console.WriteLine("siamo in opRes");
                    Debug.Assert(!json.Equals(""), "json == {}");
                    Debug.Assert(json != null, "json == NULL");
                    //Console.WriteLine(json); //se decommentata blocca il programma
                    OpenWindow win = JsonConvert.DeserializeObject<OpenWindow>(json.ToString());
                    Debug.Assert(win != null, "win == NULL");
                    if (win != null)
                    {
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
                            Console.WriteLine("aggiunto l'item win");
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
                }
                else
                {   //qui devo verificare che la connessione non sia caduta, se lo è metto in grigio la finestra
                    /*if(_reconnAttempts != _RECLIMIT)
                    {
                        do
                        {
                            //riconnetto e verifico che la connessione sia andata a buon fine, altrimenti metto la finestra in grigio
                        }
                    }*/
                    Console.WriteLine("else");
                }
            //}
        }
        /*
        private async void ParseJSONToWindow(string json)
        {
            Console.WriteLine("siamo in ReadAndParseJSON");
            Debug.Assert(!json.Equals(""), "json == {}");
            Debug.Assert(json != null, "json == NULL");
            Console.WriteLine(json);
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
                Console.WriteLine("aggiunto l'item win");
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
