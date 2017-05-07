using System;
using System.Collections.Generic;
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
    public partial class ElementToShow
    {
        public string ProcName { get; set; }
        public BitmapImage ProcImg { get; set; }

        public ElementToShow(string procName, BitmapImage bmpIcon)
        {
            ProcName = procName;
            ProcImg = bmpIcon;
            InitializeComponent();
            txtB_element.Text = ProcName;
            img_element.Source = ProcImg;
        }
    }
}
