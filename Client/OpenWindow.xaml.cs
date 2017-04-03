using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Logica di interazione per OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : UserControl
    {
        [JsonProperty(PropertyName = "WindowID")]
        public string WindowID { get; set; }

        [JsonProperty(PropertyName = "WindowName")]
        public string ProcName { get; set; }

        [JsonProperty(PropertyName = "Icon")]
        public string IconaBase64 { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public String Status { get { return _status; }
            set {
                if (!_status.Equals(value))
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            } }

        public int ID { get; set; }

        private event PropertyChangedEventHandler PropertyChanged;
        private string _status;
        //private TimeSpan _hasFocusTime;

        public OpenWindow()
        {
            InitializeComponent();
            _status = "";
        }

        public void Initialize()
        {
            //_hasFocusTime = new TimeSpan();
            if (!IconaBase64.Equals(""))    //i json successivi al NewWindow non avranno l'icona, quindi arriverà una stringa vuota
            {
                img_OpenWindowIcon.Source = Base64ToBitmapImage(IconaBase64);
            }
            ID = Convert.ToInt32(WindowID);
            txtb_ProcessName.Text = ProcName;
            PropertyChanged += new PropertyChangedEventHandler(HighlightWindowOnFocus);          
        }
        public bool HasFocus()
        {
            if (Status == "OnFocus")
                return true;
            return false;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        //imposta il colore di background dell'elemento in focus
        public void HighlightWindowOnFocus(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("Status")) {
                if (Status.Equals("OnFocus"))
                {   //lo evidenzio cambiando colore di sfondo
                    this.Background = Brushes.Wheat;
                }
                else
                {   //qualcun altro è in focus, tolgo il colore di sfondo
                    this.ClearValue(BackgroundProperty);
                }
            }
        }

        public static BitmapImage Base64ToBitmapImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                BitmapImage bmpimg = new BitmapImage();
                bmpimg.BeginInit();
                bmpimg.CacheOption = BitmapCacheOption.OnLoad;
                bmpimg.StreamSource = ms;
                bmpimg.EndInit();
                return bmpimg;
            }
        }
    }
}
