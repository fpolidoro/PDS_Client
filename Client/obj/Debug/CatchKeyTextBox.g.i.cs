﻿#pragma checksum "..\..\CatchKeyTextBox.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E74330E0EC38B07EBEDDBA8880857198"
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
    /// CatchKeyTextBox
    /// </summary>
    public partial class CatchKeyTextBox : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 26 "..\..\CatchKeyTextBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border border_UserControl;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\CatchKeyTextBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtB_captureKeyCombo;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\CatchKeyTextBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContextMenu cntxtMenu_captureKeyCombo;
        
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
            System.Uri resourceLocater = new System.Uri("/Client;component/catchkeytextbox.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CatchKeyTextBox.xaml"
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
            this.border_UserControl = ((System.Windows.Controls.Border)(target));
            
            #line 26 "..\..\CatchKeyTextBox.xaml"
            this.border_UserControl.LostFocus += new System.Windows.RoutedEventHandler(this.Border_LostFocus);
            
            #line default
            #line hidden
            
            #line 26 "..\..\CatchKeyTextBox.xaml"
            this.border_UserControl.GotFocus += new System.Windows.RoutedEventHandler(this.border_GotFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtB_captureKeyCombo = ((System.Windows.Controls.TextBox)(target));
            
            #line 32 "..\..\CatchKeyTextBox.xaml"
            this.txtB_captureKeyCombo.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.txtB_captureKeyCombo_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 32 "..\..\CatchKeyTextBox.xaml"
            this.txtB_captureKeyCombo.ContextMenuOpening += new System.Windows.Controls.ContextMenuEventHandler(this.txtB_captureKeyCombo_ContextMenuOpening);
            
            #line default
            #line hidden
            return;
            case 3:
            this.cntxtMenu_captureKeyCombo = ((System.Windows.Controls.ContextMenu)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

