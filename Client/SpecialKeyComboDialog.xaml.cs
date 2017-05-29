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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Logica di interazione per SpecialKeyComboDialog.xaml
    /// </summary>
    public partial class SpecialKeyComboDialog : Window
    {
        private static readonly List<Key> ModKeys = new List<Key> { Key.LWin, Key.RWin, Key.LeftCtrl, Key.RightCtrl, Key.RightAlt, Key.LeftAlt, Key.RightShift, Key.LeftShift, Key.RWin, Key.LWin, Key.System };
 
        private List<string> _keys;
        private List<int> _keyCodes;
        private bool send;

        public SpecialKeyComboDialog(out List<int> keyCodes)
        {
            InitializeComponent();
            _keys = new List<string>();
            keyCodes = new List<int>();
            _keyCodes = keyCodes;
            send = false;
        }

        private void txtB_GetKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string keyString = null;
            int keyValue = -1;
            Debug.Assert(_keys != null);
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            if (e.SystemKey == Key.F10) //questo trova quando premo F10, che è una special key per windows
            {
                keyString = e.SystemKey.ToString();
                keyValue = KeyInterop.VirtualKeyFromKey(key);
                Debug.WriteLine("F10 pressed");
            }
            else if (!ModKeys.Contains(key))//ho premuto una lettera
            {
                keyString = key.ToString();
                keyValue = KeyInterop.VirtualKeyFromKey(key);
            }
            else e.Handled = true;

            if (keyString != null)
            {
                //se arrivo qua e le liste sono non nulle, vuol dire che avevo già scritto qualcosa nel field
                //e ora ne ho riscritta un'altra
                if(_keyCodes.Count > 0 || _keys.Count > 0)
                {
                    _keys.Clear();
                    _keyCodes.Clear();
                }
                _keys.Add(keyString);
                _keyCodes.Add(keyValue);
                txtB_GetKey.Text = keyString;
#if(DEBUG)
                foreach (var k in _keyCodes)
                    Debug.Write(k + " ");
                Debug.WriteLine("");
#endif
                e.Handled = true;
            }
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            if (btn_ctrl.IsChecked == true) btn_ctrl.IsChecked = false;
            if (btn_alt.IsChecked == true) btn_alt.IsChecked = false;
            if (btn_shift.IsChecked == true) btn_shift.IsChecked = false;
            if (btn_win.IsChecked == true) btn_win.IsChecked = false;
            if (!txtB_GetKey.Text.Equals(string.Empty)) txtB_GetKey.Text = string.Empty;

            if (_keys.Count > 0 || _keyCodes.Count > 0) {
                _keys.Clear();
                _keyCodes.Clear();
            }
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if (btn_ctrl.IsChecked == true) _keyCodes.Add(KeyInterop.VirtualKeyFromKey(Key.LeftCtrl));
            if (btn_alt.IsChecked == true) _keyCodes.Add(KeyInterop.VirtualKeyFromKey(Key.LeftAlt));
            if (btn_shift.IsChecked == true) _keyCodes.Add(KeyInterop.VirtualKeyFromKey(Key.LeftShift));
            if (btn_win.IsChecked == true) _keyCodes.Add((int)Key.LWin);
#if(DEBUG)
            Debug.WriteLine("_keys:");
            foreach (var v in _keys)
                Debug.WriteLine(v);
#endif
            Debug.Assert(_keyCodes.Count <= 5, "La lista contiene più di 5 tasti.");
            send = true;
            Close();    //chiudo la finestra
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("OnClosing called");
            //quando sto chiudendo la finestra, potrei avere selezionato cose, quindi keys.Count != 0
            //ma, se non ho selezionato invia, al main arriverebbe una lista di schifezze, quindi devo
            //prima resettarlo.
            //se invece ho cliccato invia, è corretto che al main arrivi una lista non vuota
            if (!send)
            {
                _keys.Clear();
                _keyCodes.Clear();
            }
        }
    }
}
