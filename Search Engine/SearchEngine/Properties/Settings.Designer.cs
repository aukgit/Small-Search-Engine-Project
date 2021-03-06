﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SearchEngine.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("pdf,jpg,gif,dat,mov,3gp,wmv,mp3,wav,ogg")]
        public string searchingDisableExtension {
            get {
                return ((string)(this["searchingDisableExtension"]));
            }
            set {
                this["searchingDisableExtension"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("txt,php,cs,vb,asp,aspx,cshtml,html,htm,js")]
        public string searchingExtension {
            get {
                return ((string)(this["searchingExtension"]));
            }
            set {
                this["searchingExtension"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int searchingSizeType {
            get {
                return ((int)(this["searchingSizeType"]));
            }
            set {
                this["searchingSizeType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal startingSize {
            get {
                return ((decimal)(this["startingSize"]));
            }
            set {
                this["startingSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal endingSize {
            get {
                return ((decimal)(this["endingSize"]));
            }
            set {
                this["endingSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool lookForDates {
            get {
                return ((bool)(this["lookForDates"]));
            }
            set {
                this["lookForDates"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int searchingDatesType {
            get {
                return ((int)(this["searchingDatesType"]));
            }
            set {
                this["searchingDatesType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime startingDate {
            get {
                return ((global::System.DateTime)(this["startingDate"]));
            }
            set {
                this["startingDate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime endingDate {
            get {
                return ((global::System.DateTime)(this["endingDate"]));
            }
            set {
                this["endingDate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool searchForContent {
            get {
                return ((bool)(this["searchForContent"]));
            }
            set {
                this["searchForContent"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool contentFoundMany {
            get {
                return ((bool)(this["contentFoundMany"]));
            }
            set {
                this["contentFoundMany"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool searchFromDatabase {
            get {
                return ((bool)(this["searchFromDatabase"]));
            }
            set {
                this["searchFromDatabase"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool doUseNaturalSearch {
            get {
                return ((bool)(this["doUseNaturalSearch"]));
            }
            set {
                this["doUseNaturalSearch"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool doUseSorting {
            get {
                return ((bool)(this["doUseSorting"]));
            }
            set {
                this["doUseSorting"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool doUseHashing {
            get {
                return ((bool)(this["doUseHashing"]));
            }
            set {
                this["doUseHashing"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool saveToDatabase {
            get {
                return ((bool)(this["saveToDatabase"]));
            }
            set {
                this["saveToDatabase"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool searchFromCache {
            get {
                return ((bool)(this["searchFromCache"]));
            }
            set {
                this["searchFromCache"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        public int expireCacheDays {
            get {
                return ((int)(this["expireCacheDays"]));
            }
            set {
                this["expireCacheDays"] = value;
            }
        }
    }
}
