#define DEBUG
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        public string SelectedProcess
        {
            get;
            private set;
        }
        public ListBoxWithButtons()
        {
            InitializeComponent();
            popup.Closed += popup_Closed;
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

            if (e.NewItems != null)
            {
                if (listView_focusedProcesses.Items.Count >= _NoOfItemsInView)
                {
                    btn_down.IsEnabled = true;
                    btn_up.IsEnabled = true;
                }/*
                foreach (var v in e.NewItems)
                    listBox_showAllProcesses.Items.Add(v);*/
            }

            if (e.OldItems != null)
            {
                if (listView_focusedProcesses.Items.Count <= _NoOfItemsInView)
                {
                    btn_down.IsEnabled = false;
                    btn_up.IsEnabled = false;
                }
                /*foreach (var v in e.OldItems)
                    listBox_showAllProcesses.Items.Remove(v);*/
            }
        }

        private void btn_up_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(listView_focusedProcesses) as ScrollViewer;

            if (scrollViewer != null)
            {
                // Logical Scrolling by Item
                scrollViewer.LineUp();
            }
        }

        private void btn_down_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(listView_focusedProcesses) as ScrollViewer;

            if (scrollViewer != null)
            {
                // Logical Scrolling by Item
                scrollViewer.LineDown();
            }
        }

        /*  Per abilitare/disabilitare i bottoni di scroll
        */
        private void ListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = e.OriginalSource as ScrollViewer;
            if (scrollViewer != null)
            {
                btn_up.IsEnabled = scrollViewer.ScrollableHeight > 0 && scrollViewer.VerticalOffset > 0;
                btn_down.IsEnabled = scrollViewer.ScrollableHeight > 0 &&
                      scrollViewer.VerticalOffset + scrollViewer.ViewportHeight < scrollViewer.ExtentHeight;
            }
        }

        //helper to get the ScrollViwer component of something like a ListBox, ListView, etc
        private static DependencyObject GetScrollViewer(DependencyObject o)
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

        /* private void ClosePopupPopup_Click(object sender, RoutedEventArgs e)
         {
             popup.IsOpen = false;
         }*/
        private void buttonShowPopup_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
            //ClosePopup.SetValue(Canvas.ZIndexProperty, 1);
            btn_showPopup.SetValue(Canvas.ZIndexProperty, 0);
        }

        void popup_Closed(object sender, EventArgs e)
        {
            btn_showPopup.SetValue(Canvas.ZIndexProperty, 1);
            //ClosePopup.SetValue(Canvas.ZIndexProperty, 0);
        }

        private void listView_focusedProcesses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if(DEBUG)
            Debug.WriteLine("Selection changed: ");
            foreach (ElementToShow i in listView_focusedProcesses.SelectedItems)
                Debug.WriteLine(i.ProcName);
            Debug.WriteLine("-------------------");
#endif
            //fa in modo che quando (soprattutto dal popup) seleziono un elemento che in listView non è visibile,
            //la listView venga scrollata fino all'item selezionato
            listView_focusedProcesses.ScrollIntoView(listView_focusedProcesses.SelectedItem);
            if (listView_focusedProcesses.SelectedItem != null)
                SelectedProcess = (listView_focusedProcesses.SelectedItem as ElementToShow).ProcName;
            else SelectedProcess = null;
        }

        //Quando seleziono un elemento dal popup, devo renderlo selezionato anche nella lista
        private void popup_LostFocus(object sender, RoutedEventArgs e)
        {
            listView_focusedProcesses.SelectedItems.Clear();
            foreach (ElementToShow v in listBox_showAllProcesses.SelectedItems)
            {
                int index = listBox_showAllProcesses.SelectedIndex;
                listView_focusedProcesses.SelectedIndex = index;
            }
#if (DEBUG)
            Debug.WriteLine("Popup lost focus - listView selectedItems: ");
            foreach (ElementToShow i in listView_focusedProcesses.SelectedItems)
                Debug.WriteLine(i.ProcName);
            Debug.WriteLine("-------------------");
#endif
        }
    }
}
