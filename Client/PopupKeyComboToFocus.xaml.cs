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

namespace Client
{
    /// <summary>
    /// Logica di interazione per PopupKeyComboToFocus.xaml
    /// </summary>
    public partial class PopupKeyComboToFocus : UserControl
    {
        private static readonly List<Key> ModKeys = new List<Key> { Key.LWin, Key.RWin, Key.LeftCtrl, Key.RightCtrl, Key.RightAlt, Key.LeftAlt, Key.RightShift, Key.LeftShift, Key.RWin, Key.LWin, Key.System };

        private List<string> _keys;
        private List<int> _keyCodes;
        private ServerElement _srvElement;

        public PopupKeyComboToFocus()
        {
            InitializeComponent();
            _keys = new List<string>();
            _keyCodes = new List<int>();
        }

        public void SetGrandParent(ServerElement grandParent)
        {
            _srvElement = grandParent;
            cktxt_getKeyCombo.SetGrandParent(_srvElement);
        }

        private void btn_clean_Click(object sender, RoutedEventArgs e)
        {
            cktxt_getKeyCombo.CleanValue();
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
                if (_keyCodes.Count > 0 || _keys.Count > 0)
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

        private void btn_cleanSpecial_Click(object sender, RoutedEventArgs e)
        {
            if (btn_ctrl.IsChecked == true) btn_ctrl.IsChecked = false;
            if (btn_alt.IsChecked == true) btn_alt.IsChecked = false;
            if (btn_shift.IsChecked == true) btn_shift.IsChecked = false;
            if (btn_win.IsChecked == true) btn_win.IsChecked = false;
            if (!txtB_GetKey.Text.Equals(string.Empty)) txtB_GetKey.Text = string.Empty;

            if (_keys.Count > 0 || _keyCodes.Count > 0)
            {
                _keys.Clear();
                _keyCodes.Clear();
            }
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if (btn_ctrl.IsChecked == true) _keyCodes.Insert(0, KeyInterop.VirtualKeyFromKey(Key.LeftCtrl));
            if (btn_alt.IsChecked == true) _keyCodes.Insert(0, KeyInterop.VirtualKeyFromKey(Key.LeftAlt));
            if (btn_shift.IsChecked == true) _keyCodes.Insert(0, KeyInterop.VirtualKeyFromKey(Key.LeftShift));
            if (btn_win.IsChecked == true) _keyCodes.Insert(0, KeyInterop.VirtualKeyFromKey(Key.LWin));

#if(DEBUG)
            Debug.WriteLine("_keys:");
            foreach (var v in _keyCodes)
                Debug.WriteLine(v);
#endif
            Debug.Assert(_keyCodes.Count <= 5, "La lista contiene più di 5 tasti.");
            var kmsg = new KeyMessage(null, _keyCodes.Count, _keyCodes.ToArray());
            string json = JsonConvert.SerializeObject(kmsg);

            //controllo che string != null, altrimenti mando un msgBox di errore
            if (json == null)
            {
                MessageBox.Show("Error in serializing key combo into JSON", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _srvElement.SendKeyCombo(json);
            btn_cleanSpecial_Click(null, null);
        }

        private void btn_sendALTF4_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add(KeyInterop.VirtualKeyFromKey(Key.LeftAlt));
            keys.Add(KeyInterop.VirtualKeyFromKey(Key.F4));

            var kmsg = new KeyMessage(null, keys.Count, keys.ToArray());
            string json = JsonConvert.SerializeObject(kmsg);

            //controllo che string != null, altrimenti mando un msgBox di errore
            if (json == null)
            {
                MessageBox.Show("Error in serializing key combo into JSON", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _srvElement.SendKeyCombo(json);
        }

        private void btn_sendAltTab_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add(KeyInterop.VirtualKeyFromKey(Key.LeftAlt));
            keys.Add(KeyInterop.VirtualKeyFromKey(Key.Tab));

            var kmsg = new KeyMessage(null, keys.Count, keys.ToArray());
            string json = JsonConvert.SerializeObject(kmsg);

            //controllo che string != null, altrimenti mando un msgBox di errore
            if (json == null)
            {
                MessageBox.Show("Error in serializing key combo into JSON", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _srvElement.SendKeyCombo(json);
        }

        private void btn_sendWinKey_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add(KeyInterop.VirtualKeyFromKey(Key.LWin));

            var kmsg = new KeyMessage(null, keys.Count, keys.ToArray());
            string json = JsonConvert.SerializeObject(kmsg);

            //controllo che string != null, altrimenti mando un msgBox di errore
            if (json == null)
            {
                MessageBox.Show("Error in serializing key combo into JSON", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _srvElement.SendKeyCombo(json);
        }

        private void btn_sendPrint_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add(KeyInterop.VirtualKeyFromKey(Key.PrintScreen));

            var kmsg = new KeyMessage(null, keys.Count, keys.ToArray());
            string json = JsonConvert.SerializeObject(kmsg);

            //controllo che string != null, altrimenti mando un msgBox di errore
            if (json == null)
            {
                MessageBox.Show("Error in serializing key combo into JSON", "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _srvElement.SendKeyCombo(json);
        }

        private void exp_specialKey_Collapsed(object sender, RoutedEventArgs e)
        {
            border_container.Height = 80;
        }

        private void exp_specialKey_Expanded(object sender, RoutedEventArgs e)
        {
            border_container.Height = 140;
        }
    }
}
