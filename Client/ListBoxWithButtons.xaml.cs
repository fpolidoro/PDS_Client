using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Logica di interazione per ListBoxWithButtons.xaml
    /// </summary>
    public partial class ListBoxWithButtons : UserControl
    {
        public ListBoxWithButtons()
        {
            InitializeComponent();
        }

        public void SetItemSource(ObservableCollection<string> source)
        {
            listBox_focusedProcesses.ItemsSource = source;
        }

        private void btn_down_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void btn_up_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
