﻿#pragma checksum "..\..\..\..\Forms\Scripting\FrmTransPlaceholder2.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "279E412693E783B7227AF2673AE5DFC610EF4103"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace DBI_Scripting.Forms.Scripting {
    
    
    /// <summary>
    /// FrmTransPlaceholder2
    /// </summary>
    public partial class FrmTransPlaceholder2 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\..\..\Forms\Scripting\FrmTransPlaceholder2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtScriptPath;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\..\..\Forms\Scripting\FrmTransPlaceholder2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnBrowse;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\..\..\Forms\Scripting\FrmTransPlaceholder2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExecute;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\..\Forms\Scripting\FrmTransPlaceholder2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExit;
        
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
            System.Uri resourceLocater = new System.Uri("/DBI Scripting;component/forms/scripting/frmtransplaceholder2.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Forms\Scripting\FrmTransPlaceholder2.xaml"
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
            this.txtScriptPath = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.btnBrowse = ((System.Windows.Controls.Button)(target));
            
            #line 8 "..\..\..\..\Forms\Scripting\FrmTransPlaceholder2.xaml"
            this.btnBrowse.Click += new System.Windows.RoutedEventHandler(this.btnBrowse_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnExecute = ((System.Windows.Controls.Button)(target));
            
            #line 9 "..\..\..\..\Forms\Scripting\FrmTransPlaceholder2.xaml"
            this.btnExecute.Click += new System.Windows.RoutedEventHandler(this.btnExecute_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnExit = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\..\..\Forms\Scripting\FrmTransPlaceholder2.xaml"
            this.btnExit.Click += new System.Windows.RoutedEventHandler(this.btnExit_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

