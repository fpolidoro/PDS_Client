﻿#pragma checksum "..\..\IPTextBox.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1FF24BFBFA34388143DCE3A672536FEB"
//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// IPTextBox
    /// </summary>
    public partial class IPTextBox : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 49 "..\..\IPTextBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox FirstSegment;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\IPTextBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SecondSegment;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\IPTextBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ThirdSegment;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\IPTextBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LastSegment;
        
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
            System.Uri resourceLocater = new System.Uri("/Client;component/iptextbox.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\IPTextBox.xaml"
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
            this.FirstSegment = ((System.Windows.Controls.TextBox)(target));
            
            #line 50 "..\..\IPTextBox.xaml"
            this.FirstSegment.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBoxBase_OnTextChanged);
            
            #line default
            #line hidden
            
            #line 50 "..\..\IPTextBox.xaml"
            this.FirstSegment.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.UIElement_OnPreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 50 "..\..\IPTextBox.xaml"
            this.FirstSegment.AddHandler(System.Windows.DataObject.PastingEvent, new System.Windows.DataObjectPastingEventHandler(this.DataObject_OnPasting));
            
            #line default
            #line hidden
            return;
            case 2:
            this.SecondSegment = ((System.Windows.Controls.TextBox)(target));
            
            #line 55 "..\..\IPTextBox.xaml"
            this.SecondSegment.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBoxBase_OnTextChanged);
            
            #line default
            #line hidden
            
            #line 55 "..\..\IPTextBox.xaml"
            this.SecondSegment.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.UIElement_OnPreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 55 "..\..\IPTextBox.xaml"
            this.SecondSegment.AddHandler(System.Windows.DataObject.PastingEvent, new System.Windows.DataObjectPastingEventHandler(this.DataObject_OnPasting));
            
            #line default
            #line hidden
            return;
            case 3:
            this.ThirdSegment = ((System.Windows.Controls.TextBox)(target));
            
            #line 60 "..\..\IPTextBox.xaml"
            this.ThirdSegment.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBoxBase_OnTextChanged);
            
            #line default
            #line hidden
            
            #line 60 "..\..\IPTextBox.xaml"
            this.ThirdSegment.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.UIElement_OnPreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 60 "..\..\IPTextBox.xaml"
            this.ThirdSegment.AddHandler(System.Windows.DataObject.PastingEvent, new System.Windows.DataObjectPastingEventHandler(this.DataObject_OnPasting));
            
            #line default
            #line hidden
            return;
            case 4:
            this.LastSegment = ((System.Windows.Controls.TextBox)(target));
            
            #line 65 "..\..\IPTextBox.xaml"
            this.LastSegment.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBoxBase_OnTextChanged);
            
            #line default
            #line hidden
            
            #line 65 "..\..\IPTextBox.xaml"
            this.LastSegment.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.UIElement_OnPreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 65 "..\..\IPTextBox.xaml"
            this.LastSegment.AddHandler(System.Windows.DataObject.PastingEvent, new System.Windows.DataObjectPastingEventHandler(this.DataObject_OnPasting));
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

