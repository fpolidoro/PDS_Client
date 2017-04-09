using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Diagnostics;
using WpfAnimatedGif;

namespace Client
{
    /// <summary>
    /// Logica di interazione per Window1.xaml
    /// </summary>
    public partial class NewConnectionDialog : Window, INotifyPropertyChanged
    {
        private MainWindow _parentWindow;
        private Boolean _addressIsValid;    //indirizzo
        private Boolean _portIsValid;    //porta
        private bool _closeOnConnected;
        private string _address;
        private Int32 _port;
        private string _connectStatus;
        private LinkedList<string> _existingServers;

        private Uri _uriWorld;
        private Uri _uriCrossedWorld;
        private Uri _uriComputerBN;
        private Uri _uriComputer;
        private Uri _uriLoadCorrect;
        private Uri _uriLoadFailed;
        private Uri _uriLoading;
        private Server _srv;

        public event PropertyChangedEventHandler PropertyChanged;

        public NewConnectionDialog(out Server serv, MainWindow parentWin, LinkedList<string> existingServers)
        {
            serv = new Server();
            _srv = serv;
            Debug.Assert(_srv == serv);
            _existingServers = existingServers;
            _parentWindow = parentWin;
            InitializeComponent();
            rdbtn_IPaddress.IsChecked = true;   //altrimenti dà nullRefException perchè il secondo radio non è ancora inizializzato quando il primo viene spuntato
            btn_Connect.IsEnabled = false;
            ptxt_DNSport.SetParent(this);
            ptxt_IPport.SetParent(this);
            iptxt_IPaddress.SetParent(this);
            atxt_DNSaddress.SetParent(this);
            _portIsValid = false;
            _connectStatus = "";
            _uriWorld = new Uri(@"pack://application:,,,/imgs/32x32_world.png");
            _uriCrossedWorld = new Uri(@"pack://application:,,,/imgs/32x32_worldCrossed.png");
            _uriComputerBN = new Uri(@"pack://application:,,,/imgs/48x48_ComputerBN.png");
            _uriComputer = new Uri(@"pack://application:,,,/imgs/48x48_Computer.png");
            _uriLoadCorrect = new Uri(@"pack://application:,,,/imgs/loadCorrect.gif");
            _uriLoadFailed = new Uri(@"pack://application:,,,/imgs/loadFailed.gif");
            _uriLoading = new Uri(@"pack://application:,,,/imgs/loadingBar.gif");
        }

        private void rdbtn_IPaddress_Checked(object sender, RoutedEventArgs e)
        {
            grid_DNSaddress.IsEnabled = false;
            if (grid_IPaddress.IsEnabled != true)
                grid_IPaddress.IsEnabled = true;
            iptxt_IPaddress.SetFocus();
            //cancello tutti i valori immessi nei campi relativi all'indirizzo IP
            if (atxt_DNSaddress.textBox.Text != null)
            {
                if (atxt_DNSaddress.textBox.Text.Equals("") == false)
                    atxt_DNSaddress.textBox.Clear();
            }
            if (ptxt_DNSport.txt_port != null)
            {
                if (ptxt_DNSport.txt_port.Equals("") == false)
                    ptxt_DNSport.txt_port.Clear();
            }
            _portIsValid = false;
            btn_Connect.IsEnabled = false;

        }

        private void rdbtn_DNSaddress_Checked(object sender, RoutedEventArgs e)
        {
            grid_IPaddress.IsEnabled = false;
            grid_DNSaddress.IsEnabled = true;
            atxt_DNSaddress.SetFocus();
            //cancello tutti i valori immessi nei campi relativi all'indirizzo IP
            if (iptxt_IPaddress.Address != null)
            {
                if (iptxt_IPaddress.Address.Equals("") == false)
                    iptxt_IPaddress.Clear();
            }
            if (ptxt_IPport.txt_port != null)
            {
                if (ptxt_IPport.txt_port.Equals("") == false)
                    ptxt_IPport.txt_port.Clear();
            }
            _portIsValid = false;
            btn_Connect.IsEnabled = false;
        }

        public bool AddressValid
        {
            get { return this._addressIsValid; }
            set
            {
                if (this._addressIsValid != value)
                {
                    this._addressIsValid = value;
                    OnPropertyChanged("AddressValid");
                }
                //non andrebbe fatto così, ma con il binding in XAML
                btn_Connect.IsEnabled = (_portIsValid && _addressIsValid && (rdbtn_IPaddress.IsChecked == true || rdbtn_DNSaddress.IsChecked == true));
            }
        }

        public Boolean PortValid
        {
            get { return this._portIsValid; }
            set
            {
                if (this._portIsValid != value)
                {
                    this._portIsValid = value;
                    OnPropertyChanged("PortValid");
                }
                //non andrebbe fatto così, ma con il binding in XAML
                btn_Connect.IsEnabled = (_portIsValid && _addressIsValid && (rdbtn_IPaddress.IsChecked == true || rdbtn_DNSaddress.IsChecked == true));
            }
        }


        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void txt_DNSaddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!atxt_DNSaddress.textBox.Text.Equals(""))
            {
                AddressValid = true;
            }
            else
            {
                AddressValid = false;
            }
        }

        private async void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
                if (rdbtn_IPaddress.IsChecked == true)
                {
                    _address = iptxt_IPaddress.Address;
                    _port = ptxt_IPport.Port;
                }
                else if (rdbtn_DNSaddress.IsChecked == true)
                {
                    _address = atxt_DNSaddress.textBox.Text;
                    _port = ptxt_DNSport.Port;
                }
                else
                {
                    _address = null;
                    _port = 0;
                }
            if (_existingServers.Contains(_address))
            {   //MA devo controllare il caso in cui prima metto l'ip e poi il nome corrispondente allo stesso ip
                MessageBox.Show(MsgServerAlreadyExisting(_address), "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else {
                stackp_ConnectionStatus.Visibility = Visibility.Visible;
                try
                { //se le immagini non possono essere caricate, non visualizzo il pannello con le immagini
                    img_world.Source = new BitmapImage(_uriWorld);
                    img_server.Source = new BitmapImage(_uriComputerBN);
                    ImageBehavior.SetAnimatedSource(gif_loading2server, new BitmapImage(_uriLoading));
                    ImageBehavior.SetAnimatedSource(gif_loading2world, new BitmapImage(_uriLoading));
                }
                catch (System.IO.IOException)
                {
                    stackp_ConnectionImgs.Visibility = Visibility.Collapsed;
                } 
                ConnectionLabel = "Connecting";
                _srv.SetAddressAndPort(_address, _port);
                Action<string> actionUpdateStatusBar = _parentWindow.UpdateStatusBar;
                StringBuilder connectingTo = new StringBuilder("Trying to connect to ");
                connectingTo.Append(_address);
                connectingTo.Append(":");
                connectingTo.Append(_port);
                _parentWindow.Dispatcher.BeginInvoke(actionUpdateStatusBar, connectingTo.ToString());
                ConnectionLabel = await Task.Run(() => _srv.Startup());
                if (ConnectionLabel.Equals("Connected"))
                {
                    connectingTo.Clear();
                    connectingTo.Append("Connected to ");
                    connectingTo.Append(_address);
                    _parentWindow.Dispatcher.BeginInvoke(actionUpdateStatusBar, connectingTo.ToString());
                }
                    //animazione della gui
            }
        }

        /** PropertyChangeListener per modificare la label dello stato di connessione
        **/
        public string ConnectionLabel
        {
            get { return _connectStatus; }
            set
            {
                if (this._connectStatus != value)
                {
                    this._connectStatus = value;
                    OnPropertyChanged("ConnectionLabel");
                }
                lbl_ConnectionStatus.Text = _connectStatus;

                if (_connectStatus.Equals("Connecting"))
                {   //la connessione è in corso, quindi riporto il colore della lbl a nero
                    if (lbl_ConnectionStatus.Foreground != Brushes.Black)
                        lbl_ConnectionStatus.Foreground = Brushes.Black;
                    stackp_Main.Cursor = Cursors.Wait;  //il cursore dell'intera finestra deve essere clessidra
                    rdbtn_DNSaddress.IsEnabled = false; //disabilito i radio button degli indirizzi
                    rdbtn_IPaddress.IsEnabled = false;  //e anche i text field 
                    btn_Connect.IsEnabled = false;      //non deve essere possibile modificare gli indirizzi in corso d'opera
                    btn_Cancel.IsEnabled = false;

                    if (rdbtn_IPaddress.IsChecked == true)
                    {
                        iptxt_IPaddress.IsEnabled = false;
                        ptxt_IPport.IsEnabled = false;
                    }
                    else if (rdbtn_DNSaddress.IsChecked == true)
                    {
                        atxt_DNSaddress.IsEnabled = false;
                        ptxt_DNSport.IsEnabled = false;
                    }
                }
                else
                {   //non sono in connessione (sono o prima o dopo), riporto le cose com'erano prima
                    stackp_Main.Cursor = Cursors.Arrow;
                    rdbtn_DNSaddress.IsEnabled = true;
                    rdbtn_IPaddress.IsEnabled = true;
                    btn_Connect.IsEnabled = true;
                    btn_Cancel.IsEnabled = true;
                    if (rdbtn_IPaddress.IsChecked == true)
                    {
                        iptxt_IPaddress.IsEnabled = true;
                        ptxt_IPport.IsEnabled = true;
                    }
                    else if (rdbtn_DNSaddress.IsChecked == true)
                    {
                        atxt_DNSaddress.IsEnabled = true;
                        ptxt_DNSport.IsEnabled = true;
                    }

                    if (!_connectStatus.Equals("Connected"))    //la connessione è fallita
                    {   //porto la lbl su rosso e cambio le immaginette
                        lbl_ConnectionStatus.Foreground = Brushes.Red;
                        try
                        {
                            if (stackp_ConnectionImgs.Visibility == Visibility.Visible)
                            {
                                img_world.Source = new BitmapImage(_uriCrossedWorld);
                                ImageBehavior.SetAnimatedSource(gif_loading2server, new BitmapImage(_uriLoadFailed));
                                ImageBehavior.SetAnimatedSource(gif_loading2world, new BitmapImage(_uriLoadFailed));
                            }
                        }
                        catch (System.IO.IOException)
                        {   //se una delle immaginette non può essere caricata, rendo collapsed il pannello delle imgs
                            stackp_ConnectionImgs.Visibility = Visibility.Collapsed;
                        }
                        _srv.IsValid = false;
                    }
                    else
                    {   //connesso.
                        _srv.IsValid = true;
                        //Metto il colore della lbl a nero
                        if (lbl_ConnectionStatus.Foreground != Brushes.Black)
                            lbl_ConnectionStatus.Foreground = Brushes.Black;
                        try
                        {   //imposto le immaginette di connessione avvenuta
                            if (stackp_ConnectionImgs.Visibility == Visibility.Visible)
                            {
                                img_server.Source = new BitmapImage(_uriComputer);
                                ImageBehavior.SetAnimatedSource(gif_loading2server, new BitmapImage(_uriLoadCorrect));
                                ImageBehavior.SetAnimatedSource(gif_loading2world, new BitmapImage(_uriLoadCorrect));
                            }
                        }
                        catch (System.IO.IOException)
                        {
                            stackp_ConnectionImgs.Visibility = Visibility.Collapsed;
                        }
                        //collasso il bottone connect e rendo visibile il bottone ok
                        btn_Connect.Visibility = Visibility.Collapsed;
                        btn_Cancel.Visibility = Visibility.Collapsed;
                        btn_Ok.Visibility = Visibility.Visible;
                        if (_closeOnConnected)
                            this.Close();
                    }
                }
            }
        }


        /**Se checkbox ha il tick, la finestra si chiude appena la connessione va a buon fine
        */
        private void chk_closeOnConnected_Checked(object sender, RoutedEventArgs e)
        {
            if (chk_closeOnConnected.IsChecked == true)
                _closeOnConnected = true;
            else
                _closeOnConnected = false;
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string MsgServerAlreadyExisting(string ip)
        {
            return "Server " + ip + " already exists.";
        }
    }
}
