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
    /// Logica di interazione per CatchKeyTextBox.xaml
    /// </summary>
    public partial class CatchKeyTextBox : UserControl
    {
        //per le combinazioni di tasti
        private static readonly List<Key> SpecialKeys = new List<Key> { Key.Tab, Key.Delete, Key.PageDown, Key.PageUp, Key.Print, Key.Return, Key.Back, Key.Cancel, Key.Enter, Key.End, Key.Escape, Key.Home, Key.Insert, Key.Apps};
        private static readonly List<Key> DigitKeys = new List<Key> { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9 };
        private static readonly List<Key> FunctionKeys = new List<Key> { Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12 };
        private static readonly List<Key> ModKeys = new List<Key> { Key.LeftCtrl, Key.RightCtrl, Key.RightAlt, Key.LeftAlt, Key.RightShift, Key.LeftShift, Key.RWin, Key.LWin, Key.System };
        private static readonly List<Key> WinKeys = new List<Key> { Key.LWin, Key.RWin };
        private List<string> _keys;
        List<int> _keyCodes;
        private bool _altKeyPressed;

        public CatchKeyTextBox()
        {
            InitializeComponent();
            _keys = new List<string>();
            _keyCodes = new List<int>();
            _altKeyPressed = false;
        }

        private void txtB_captureKeyCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string keyString = null;
            int keyValue = -1;
            Debug.Assert(_keys != null);
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            if (img_okTick.Visibility == Visibility.Visible)
                hideOkTick();

            //CASO #1
            //questo trova tutte le combinazioni eccetto alt+modificatore+qualcosa, alt+qualcosa
            if ((e.KeyboardDevice.Modifiers & (ModifierKeys.Alt)) == ModifierKeys.Alt)
                if (!_keys.Contains("ALT")) { _keys.Add("ALT");
                    _keyCodes.Add((int)key);
                }
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                if (!_keys.Contains("CTRL"))
                {
                    _keys.Add("CTRL");
                    _keyCodes.Add((int)key);
                }
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                if (!_keys.Contains("SHIFT")) { _keys.Add("SHIFT"); _keyCodes.Add((int)key); }
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
                if (!_keys.Contains("WINDOWS")) { _keys.Add("WINDOWS"); _keyCodes.Add((int)key); }

            if (e.SystemKey == Key.F10) //questo trova quando premo F10, che è una special key per windows
            {
                    keyString = e.SystemKey.ToString();
                    keyValue = (int)key;
                    Debug.WriteLine("F10 pressed");
            }

            if (SpecialKeys.Contains(key)) //ho premuto un tasto corrispondente a INS, PAGEUP, e simili
            {
                if (Keyboard.IsKeyDown(Key.Apps))
                    e.Handled = true;
                else if (Keyboard.IsKeyDown(Key.Tab))
                    e.Handled = true;
                keyString = key.ToString();
                if (_altKeyPressed) _altKeyPressed = false;
                keyValue = (int)key;
            }
            else if (DigitKeys.Contains(key)) //ho premuto un numero
            {
                if (e.Key.ToString().Contains("D"))
                    keyString = key.ToString().Replace("D", string.Empty);
                else keyString = key.ToString().Replace("NumPad", string.Empty);
                keyValue = (int)key;
                if (_altKeyPressed) _altKeyPressed = false;
            }
            else if (FunctionKeys.Contains(key)) //ho premuto un tasto tra F1 ed F12
            {
                keyString = key.ToString();
                keyValue = (int)key;
                if (_altKeyPressed) _altKeyPressed = false;
            }
            else if (WinKeys.Contains(key)) {   //ho premuto WINDOWS
                if (!_keys.Contains("WINDOWS")) { _keys.Add("WINDOWS");
                    keyValue = (int)key;
                }
            }
            else if (!ModKeys.Contains(key))//ho premuto una lettera
            {
                keyString = key.ToString();
                keyValue = (int)key;
                if (_altKeyPressed) _altKeyPressed = false;
            }
            else e.Handled = true;

            if (keyString != null)
            {
                _keys.Add(keyString);
                _keyCodes.Add(keyValue);
                txtB_captureKeyCombo.Text = string.Join("+", _keys);
                foreach (var k in _keyCodes)
                    Debug.Write(k + " ");
                Debug.WriteLine("");
                _keys.Clear();
                _keyCodes.Clear();
                e.Handled = true;
            }
            
        }

        //Evita che premendo il tasto Apps (tra rightWindows e RightControl) si apra un menu contestuale
        private void txtB_captureKeyCombo_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        public void CleanValue() {
            _keys.Clear();
            _keyCodes.Clear();
            txtB_captureKeyCombo.Text = "";
        }

        private void Border_LostFocus(object sender, RoutedEventArgs e)
        {
            border_UserControl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB4B4B4"));
        }

        private void border_GotFocus(object sender, RoutedEventArgs e)
        {
            border_UserControl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF3399FF"));
        }

        public void showOkTick()
        {
            txtB_captureKeyCombo.Width = 172;
            img_okTick.Visibility = Visibility.Visible;
        }

        public void hideOkTick()
        {
            txtB_captureKeyCombo.Width = 200;
            img_okTick.Visibility = Visibility.Collapsed;
        }
    }

    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null 
            // (to avoid crash bugs for views in the designer) 
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];
                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
