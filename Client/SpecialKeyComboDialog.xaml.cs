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
        List<int> _keyCodes;

        public SpecialKeyComboDialog()
        {
            InitializeComponent();
            _keys = new List<string>();
            _keyCodes = new List<int>();
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
                _keys.Add(keyString);
                _keyCodes.Add(keyValue);
                txtB_GetKey.Text = keyString;
                foreach (var k in _keyCodes)
                    Debug.Write(k + " ");
                Debug.WriteLine("");
                _keys.Clear();
                _keyCodes.Clear();
                e.Handled = true;
            }
        }
    }
}
