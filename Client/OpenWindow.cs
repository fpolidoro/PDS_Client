using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Client
{
    class OpenWindow
    {
        [JsonProperty(PropertyName = "WindowName")]
        public string Name { get; set; }
                
        public string FocusTime { get; set; }

        [JsonProperty(PropertyName = "Icon")]
        public BitmapImage Icon { get; set; }

        [JsonProperty(PropertyName = "HasFocus")]
        public bool HasFocus { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public String Status { get; set; }
        
        public OpenWindow()
        {

        }

    }
}
