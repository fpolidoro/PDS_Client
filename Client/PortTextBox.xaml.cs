using System;
using System.Collections.Generic;
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

namespace Client
{
    /// <summary>
    /// Logica di interazione per PortTextBox.xaml
    /// </summary>
    public partial class PortTextBox : UserControl
    {
        private static readonly List<Key> DigitKeys = new List<Key> { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9 };
        private static readonly List<Key> MoveForwardKeys = new List<Key> { Key.Right };
        private static readonly List<Key> MoveBackwardKeys = new List<Key> { Key.Left };
        private static readonly List<Key> OtherAllowedKeys = new List<Key> { Key.Delete, Key.Tab };
        private NewConnectionDialog _parentWindow;

        private Int32 _port;

        public Int32 Port {
            get { return _port; }
        }


        public PortTextBox()
        {
            InitializeComponent();
        }

        private void txt_port_TextChanged(object sender, TextChangedEventArgs e)
        {
            string port = txt_port.Text.ToString();
            Int32 portno;
            if (Int32.TryParse(port, out portno)) {
                if (portno < 0 || portno > 65535) {
                    img_alert.Visibility = Visibility.Visible;
                    txt_port.Foreground = Brushes.Red;
                    txt_port.ToolTip = Msg_InvalidPort();
                    _parentWindow.PortValid = false;
                    _port = 0;
                }
                else {  //la porta è valida
                    if (img_alert.IsVisible == true) {  //nascondo l'immaginetta alert, se presente
                        img_alert.Visibility = Visibility.Hidden;
                        if (txt_port.Foreground == Brushes.Red) {   
                            txt_port.Foreground = Brushes.Black;    //rimetto il testo in nero
                            txt_port.ClearValue(TextBox.ToolTipProperty);   //tolgo il tooltip
                        }
                    }
                    _port = portno;
                    _parentWindow.PortValid = true;
                }
            }
            else if(port.Equals("")){   //allora ho cancellato il numero appena scritto
                if (img_alert.IsVisible == true)
                {
                    img_alert.Visibility = Visibility.Hidden;
                    if (txt_port.Foreground == Brushes.Red)
                    {
                        txt_port.Foreground = Brushes.Black;
                        txt_port.ClearValue(TextBox.ToolTipProperty);
                        
                    }
                }
                _port = 0;
                _parentWindow.PortValid = false;
            }
            else { //se il numero non era un numero
                img_alert.Visibility = Visibility.Visible;
                txt_port.Foreground = Brushes.Red;
                txt_port.ToolTip = Msg_InvalidPort();
                _port = 0;
                _parentWindow.PortValid = false;
            }

            
        }

        private void txt_port_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && DigitKeys.Contains(e.Key))
            {
                //evito che premendo shift, sia permessa l'immissione di "£$%& eccetera
            }
            else if(MoveBackwardKeys.Contains(e.Key) ||
                MoveForwardKeys.Contains(e.Key) || (e.Key == Key.Back))
            {
                //utile a vedere se il tasto premuto è di quelli ammessi, altrimenti si esce
                //dal blocco intero (if-else) e il tasto viene ignorato
            }
            else
            {
                e.Handled = !AreOtherAllowedKeysPressed(e);
            }
        }

        //permette di utilizzare CTRL-C, CTRL-V eccetera
        private bool AreOtherAllowedKeysPressed(KeyEventArgs e)
        {
            return e.Key == Key.C && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.V && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.A && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.X && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   OtherAllowedKeys.Contains(e.Key);
        }

        private void DataObject_OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText)
            {
                e.CancelCommand();
                return;
            }

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            int num;

            if (!int.TryParse(text, out num))
            {
                e.CancelCommand();
            }
        }

        internal void SetParent(NewConnectionDialog parent)
        {
            _parentWindow = parent;
        }

        private string Msg_InvalidPort()
        {
            return "Numero di porta invalido. La porta deve essere un numero tra 1 and 65535";
        }
    }
}
