using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class AddressBox : UserControl
    {
        private static readonly List<Key> DigitKeys = new List<Key> { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9 };
        private static readonly List<Key> AlphaKeys = new List<Key> { Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J, Key.K, Key.L, Key.M, Key.N, Key.O, Key.P, Key.Q, Key.S, Key.T, Key. U, Key.V, Key.W, Key.X, Key.Y, Key.Z};
        private static readonly List<Key> PunctuationKeys = new List<Key> { Key.OemPeriod, Key.Decimal, Key.OemMinus, Key.Subtract};
        private static readonly List<Key> MoveForwardKeys = new List<Key> { Key.Right };
        private static readonly List<Key> MoveBackwardKeys = new List<Key> { Key.Left };
        private static readonly List<Key> OtherAllowedKeys = new List<Key> { Key.Delete, Key.Tab };
        private NewConnectionDialog _parentWindow;

        public AddressBox()
        {
            InitializeComponent();
        }

        public void SetFocus()
        {
            textBox.Focus();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var regexStartWithDot = new Regex("^\\.");
            var regexEndWithDot = new Regex("\\.$");
            //is valid if text starts with a letter or a number
            if(textBox.Text.Contains(".."))
            {
                textBox.Foreground = Brushes.Red;
                textBox.ToolTip = Msg_InvalidChar();
                _parentWindow.AddressValid = false;
            }else if(regexStartWithDot.IsMatch(textBox.Text)){
                textBox.Foreground = Brushes.Red;
                textBox.ToolTip = Msg_CannotStartWithDot();
                _parentWindow.AddressValid = false;
            }else if (regexEndWithDot.IsMatch(textBox.Text))
            {
                textBox.Foreground = Brushes.Red;
                textBox.ToolTip = Msg_CannotEndWithDot();
                _parentWindow.AddressValid = false;
            }
            else if(textBox.Text.Equals(""))
            {                
                if (textBox.Foreground == Brushes.Red)
                {
                    textBox.Foreground = Brushes.Black;    //rimetto il testo in nero
                    textBox.ClearValue(TextBox.ToolTipProperty);   //tolgo il tooltip
                }
                _parentWindow.AddressValid = false;
            }
            else
            {
                if (textBox.Foreground == Brushes.Red)
                {
                    textBox.Foreground = Brushes.Black;    //rimetto il testo in nero
                    textBox.ClearValue(TextBox.ToolTipProperty);   //tolgo il tooltip
                }
                _parentWindow.AddressValid = true;
            }
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && DigitKeys.Contains(e.Key) || AlphaKeys.Contains(e.Key))
            {
                //impedisco che shift + numero faccia apparire "£$%& eccetera
            }
            else if (!(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && PunctuationKeys.Contains(e.Key))
            {

            }
            else if (MoveBackwardKeys.Contains(e.Key) ||
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
            return e.Key == Key.C && (e.KeyboardDevice.Modifiers == ModifierKeys.Control) ||
                   e.Key == Key.V && (e.KeyboardDevice.Modifiers == ModifierKeys.Control) ||
                   e.Key == Key.A && (e.KeyboardDevice.Modifiers == ModifierKeys.Control) ||
                   e.Key == Key.X && (e.KeyboardDevice.Modifiers == ModifierKeys.Control) ||
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
        }

        public void SetParent(NewConnectionDialog parent)
        {
            _parentWindow = parent;
        }

        private string Msg_InvalidChar()
        {
            return "An invalid sequence of characters has been found.";
        }

        private string Msg_CannotStartWithDot()
        {
            return "An address cannot start with a dot";
        }

        private string Msg_CannotEndWithDot()
        {
            return "An address cannot end with a dot";
        }
    }
}
