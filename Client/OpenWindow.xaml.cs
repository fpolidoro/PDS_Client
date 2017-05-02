using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
        public string IconaBase64 { get { return _base64string; }
            set { if (!value.Equals(""))
                    _base64string = value;
            }
        }
        private string _base64string;

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

        /*public TimeSpan FocusTime{
            get{return _hasFocusTime; }
            set{ _hasFocusTime = _hasFocusTime.Add(value); }
        }*/
        public TimeSpan FocusTime {
            get { return _focusStopWatch.Elapsed; }
        }

        private event PropertyChangedEventHandler PropertyChanged;
        private string _status;
        private TimeSpan _hasFocusTime;
        private Stopwatch _focusStopWatch;

        public OpenWindow()
        {
            InitializeComponent();
            _status = "";
            
        }

        public void Initialize()
        {
            _hasFocusTime = TimeSpan.Zero;  //azzero il timespan di permanenza in focus
            if (!IconaBase64.Equals(""))    //i json successivi al NewWindow non avranno l'icona, quindi arriverà una stringa vuota
            {
                img_OpenWindowIcon.Source = Base64ToBitmapImage(IconaBase64);
            }
            ID = Convert.ToInt32(WindowID);
            txtb_ProcessName.Text = ProcName;
            PropertyChanged += new PropertyChangedEventHandler(HighlightWindowOnFocus);
            txtb_ProcessTimeFocused.Text = "0.0%";
            _focusStopWatch = new Stopwatch();
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
                    cntxtMenu_SendKeyToThisWindow.IsEnabled = true;
                    cntxtMenu_SendKeyToThisWindow.Visibility = Visibility.Visible;
                    _focusStopWatch.Start();
                }
                else
                {   //qualcun altro è in focus, tolgo il colore di sfondo
                    this.ClearValue(BackgroundProperty);
                    cntxtMenu_SendKeyToThisWindow.IsEnabled = false;
                    cntxtMenu_SendKeyToThisWindow.Visibility = Visibility.Hidden;
                    _focusStopWatch.Stop();
                }
            }
        }

        //converte da base64 a Image
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


        //converte l'icona in scala di grigi
        public void ConvertToGrayscale()
        {
            //ottengo l'icona dal base64 (perchè ha le trasparenze)
            byte[] imageBytes = Convert.FromBase64String(_base64string);
            System.Drawing.Image image;
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                image = System.Drawing.Image.FromStream(ms);
            }
            
            System.Drawing.Bitmap grayscaleImage = new System.Drawing.Bitmap(image.Width, image.Height, image.PixelFormat);

            // Create the ImageAttributes object and apply the ColorMatrix
            System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
            System.Drawing.Imaging.ColorMatrix grayscaleMatrix = new System.Drawing.Imaging.ColorMatrix(new float[][]{
        new float[] {0.299f, 0.299f, 0.299f, 0, 0},
        new float[] {0.587f, 0.587f, 0.587f, 0, 0},
        new float[] {0.114f, 0.114f, 0.114f, 0, 0},
        new float[] {     0,      0,      0, 1, 0},
        new float[] {     0,      0,      0, 0, 1}
        });
            attributes.SetColorMatrix(grayscaleMatrix);

            // Use a new Graphics object from the new image.
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(grayscaleImage))
            {
                // Draw the original image using the ImageAttributes created above.
                g.DrawImage(image,
                            new System.Drawing.Rectangle(0, 0, grayscaleImage.Width, grayscaleImage.Height),
                            0, 0, grayscaleImage.Width, grayscaleImage.Height,
                            System.Drawing.GraphicsUnit.Pixel,
                            attributes);
            }

            //cast da Bitmap a BitmapImage
            using (var memory = new MemoryStream())
            {
                grayscaleImage.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                //imposto l'icona in scala di grigi
                img_OpenWindowIcon.Source = bitmapImage;
            }
            //porto i testi in grigio
            txtb_ProcessName.Foreground = Brushes.Gray;
            txtb_ProcessTimeFocused.Foreground = Brushes.Gray;
            //se questo oggetto corrisponde ad una finestra in focus, trasformo lo sfondo da giallo a grigio chiaro
            if (Status.Equals("OnFocus"))
            {
                Background = Brushes.LightGray;
            }
        }

        public void ComputeFocusPercentage(TimeSpan focusTimeOfAll) {
            if (!focusTimeOfAll.Equals(TimeSpan.Zero)) {
                double focusPercentage = _focusStopWatch.ElapsedMilliseconds / focusTimeOfAll.TotalMilliseconds;
                txtb_ProcessTimeFocused.Text = focusPercentage.ToString("##0.0#%", CultureInfo.InvariantCulture);
                //Nota: non ho bisogno di moltiplicare per 100 in focusPercentage perchè lo fa già il metodo ToString qui sopra,
                //grazie al simbolo %: # indica una cifra (se non presente, nulla visualizzato), 0 indica una cifra, se non presente, visualizza 0
                Debug.WriteLine(ProcName + ": " + focusPercentage + "*100 = " + focusPercentage*100 + "%");
                Debug.WriteLine(ProcName + ": hasFocusTime=" + _focusStopWatch.ElapsedMilliseconds + " totalMs=" + focusTimeOfAll.TotalMilliseconds);
            }
            else
            {
                Debug.WriteLine(ProcName + ": focusTimeOfAll is ZERO");
                txtb_ProcessTimeFocused.Text = "0.0%";
            }
        }
    }
}
