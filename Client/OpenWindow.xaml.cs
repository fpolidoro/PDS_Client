using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        [JsonProperty(PropertyName = "WindowName")]
        public string ProcName { get; set; }

        [JsonProperty(PropertyName = "Icon")]
        public string IconaBase64 { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public String Status { get; set; }

        private TimeSpan _hasFocusTime;

        public OpenWindow()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            _hasFocusTime = new TimeSpan();
            img_OpenWindowIcon.Source = Base64ToBitmapImage(IconaBase64);
            txtb_ProcessName.Text = ProcName;             
        }
        public bool HasFocus()
        {
            if (Status == "OnFocus")
                return true;
            return false;
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
