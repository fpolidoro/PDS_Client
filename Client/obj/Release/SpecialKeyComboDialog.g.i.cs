﻿#pragma checksum "..\..\SpecialKeyComboDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E04754A9340DDDF6C1E4D514BA886EB7"
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
    /// SpecialKeyComboDialog
    /// </summary>
    public partial class SpecialKeyComboDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 38 "..\..\SpecialKeyComboDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton btn_ctrl;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\SpecialKeyComboDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton btn_alt;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\SpecialKeyComboDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton btn_shift;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\SpecialKeyComboDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton btn_win;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\SpecialKeyComboDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtB_GetKey;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\SpecialKeyComboDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContextMenu cntxtMenu_captureKeyCombo;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\SpecialKeyComboDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Send;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\SpecialKeyComboDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_reset;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\SpecialKeyComboDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image img_clearCombo;
        
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
            System.Uri resourceLocater = new System.Uri("/Client;component/specialkeycombodialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SpecialKeyComboDialog.xaml"
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
            
            #line 8 "..\..\SpecialKeyComboDialog.xaml"
            ((Client.SpecialKeyComboDialog)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btn_ctrl = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 3:
            this.btn_alt = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 4:
            this.btn_shift = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 5:
            this.btn_win = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 6:
            this.txtB_GetKey = ((System.Windows.Controls.TextBox)(target));
            
            #line 42 "..\..\SpecialKeyComboDialog.xaml"
            this.txtB_GetKey.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.txtB_GetKey_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.cntxtMenu_captureKeyCombo = ((System.Windows.Controls.ContextMenu)(target));
            return;
            case 8:
            this.btn_Send = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\SpecialKeyComboDialog.xaml"
            this.btn_Send.Click += new System.Windows.RoutedEventHandler(this.btn_Send_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btn_reset = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\SpecialKeyComboDialog.xaml"
            this.btn_reset.Click += new System.Windows.RoutedEventHandler(this.btn_reset_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.img_clearCombo = ((System.Windows.Controls.Image)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

