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
        public PopupKeyComboToFocus()
        {
            InitializeComponent();
            _keys = new List<string>();
            _keyCodes = new List<int>();
    }

        private void btn_clean_Click(object sender, RoutedEventArgs e)
        {
            //cktxt_getKeyCombo.CleanValue();
        }

        private void btn_cleanSpecial_Click(object sender, RoutedEventArgs e)
        {
            //cktxt_getKeyCombo.CleanValue();
        }

        private void btn_sendALTF4_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add((int)Key.LeftAlt);
            keys.Add((int)Key.F4);

            //Send(keys);
        }

        private void btn_sendAltTab_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add((int)Key.LeftAlt);
            keys.Add((int)Key.Tab);

            //Send(keys);
        }

        private void btn_sendAltTabRight_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add((int)Key.LeftAlt);
            keys.Add((int)Key.Tab);
            keys.Add((int)Key.Right);

            //Send(keys);
        }

        private void btn_sendAltTabLeft_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add((int)Key.LeftAlt);
            keys.Add((int)Key.Tab);
            keys.Add((int)Key.Left);

            //Send(keys);
        }

        private void btn_sendPrint_Click(object sender, RoutedEventArgs e)
        {
            List<int> keys = new List<int>();
            keys.Add((int)Key.LeftAlt);
            keys.Add((int)Key.Tab);
            keys.Add((int)Key.Left);

            //Send(keys);
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
                keyValue = (int)key;
                Debug.WriteLine("F10 pressed");
            }
            else if (!ModKeys.Contains(key))//ho premuto una lettera
            {
                keyString = key.ToString();
                keyValue = (int)key;
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
