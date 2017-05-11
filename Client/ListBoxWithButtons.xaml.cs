#define DEBUG
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client
{
    /// <summary>
    /// Logica di interazione per ListBoxWithButtons.xaml
    /// </summary>
    public partial class ListBoxWithButtons : UserControl
    {
        private MainWindow _parent;
        private static int _NoOfItemsInView = 3;
        public ListBoxWithButtons()
        {
            InitializeComponent();
        }

        public void SetParent(MainWindow parent)
        {
            _parent = parent;
            _parent.WindowsToShow.CollectionChanged += new NotifyCollectionChangedEventHandler(ProcessesToShowCollectionChanged);
        }

        private void ProcessesToShowCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            #if DEBUG
            Debug.WriteLine("CollectionChanged in listbox");
            Debug.WriteLine("listbox.Count = " + listView_focusedProcesses.Items.Count);
            #endif

            if(e.NewItems != null)
            {
                //ElementToShow el = _parent.WindowsToShow.FirstOrDefault();
                if(listView_focusedProcesses.Items.Count >= _NoOfItemsInView)
                {
                    btn_down.IsEnabled = true;
                    btn_up.IsEnabled = true;
                }
            }

            if(e.OldItems != null)
            {
                if (listView_focusedProcesses.Items.Count <= _NoOfItemsInView)
                {
                    btn_down.IsEnabled = false;
                    btn_up.IsEnabled = false;
                }
            }

            /*if (listView_focusedProcesses.Items.Count <= _NoOfItemsInView)
            {
                btn_down.IsEnabled = false;
                btn_up.IsEnabled = false;
            }
            else
            {
                btn_down.IsEnabled = true;
                btn_up.IsEnabled = true;
            }*/
        }

        private void btn_up_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(listView_focusedProcesses) as ScrollViewer;

            if (scrollViewer != null)
            {
                // Logical Scrolling by Item
                scrollViewer.LineUp();
                if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                    btn_up.IsEnabled = false;
                else if (!btn_up.IsEnabled) btn_up.IsEnabled = true;
                // Physical Scrolling by Offset
                //scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset + 3);
            }
    }

        private void btn_down_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(listView_focusedProcesses) as ScrollViewer;

            if (scrollViewer != null)
            {
                // Logical Scrolling by Item
                scrollViewer.LineDown();
                if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                    btn_down.IsEnabled = false;
                else if (!btn_down.IsEnabled) btn_down.IsEnabled = true;
                // Physical Scrolling by Offset
                //scrollViwer.ScrollToVerticalOffset(scrollViwer.VerticalOffset + 3);
            }
        }

        //helper to get the ScrollViwer component of something like a ListBox, ListView, etc
        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            { return o; }
        
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null) continue;
                else return result;
            }
            return null;
        }


    }
}
