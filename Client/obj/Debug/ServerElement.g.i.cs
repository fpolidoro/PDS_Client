﻿#pragma checksum "..\..\ServerElement.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D648F20B933AD69B6B89F501BB34F2A8"
//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

using Client;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Client {
    
    
    /// <summary>
    /// ServerElement
    /// </summary>
    public partial class ServerElement : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image img_ConnectionStatus;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtb_serverAddress;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Menu menu;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image image;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mitem_reconnect;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mitem_disconnect;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mitem_close;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mitem_requestWinList;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel stackp_WindowsList;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBox_OpenWindows;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\ServerElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image gif_retrievingList;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Client;component/serverelement.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ServerElement.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.img_ConnectionStatus = ((System.Windows.Controls.Image)(target));
            return;
            case 2:
            this.txtb_serverAddress = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.menu = ((System.Windows.Controls.Menu)(target));
            return;
            case 4:
            this.image = ((System.Windows.Controls.Image)(target));
            return;
            case 5:
            this.mitem_reconnect = ((System.Windows.Controls.MenuItem)(target));
            
            #line 19 "..\..\ServerElement.xaml"
            this.mitem_reconnect.Click += new System.Windows.RoutedEventHandler(this.mitem_reconnect_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.mitem_disconnect = ((System.Windows.Controls.MenuItem)(target));
            
            #line 24 "..\..\ServerElement.xaml"
            this.mitem_disconnect.Click += new System.Windows.RoutedEventHandler(this.mitem_disconnect_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.mitem_close = ((System.Windows.Controls.MenuItem)(target));
            
            #line 29 "..\..\ServerElement.xaml"
            this.mitem_close.Click += new System.Windows.RoutedEventHandler(this.mitem_close_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.mitem_requestWinList = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 9:
            this.stackp_WindowsList = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 10:
            this.listBox_OpenWindows = ((System.Windows.Controls.ListBox)(target));
            return;
            case 11:
            this.gif_retrievingList = ((System.Windows.Controls.Image)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

