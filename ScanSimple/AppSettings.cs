// ScanSimple
// Copyright (C) 2017, by David W. Jeske, all Rights Reserved

using System;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Win32;


namespace ScanSimple
{


    static class Prefs
    {
        // non portable registry setting...
        // http://www.pcreview.co.uk/forums/using-userappdataregistry-t1226335.html

        // lookinto portable options in the future...
        // https://github.com/aritchie/settings

        public static bool HasPref(string key) {

            RegistryKey myKey = Application.UserAppDataRegistry;

            object value = myKey.GetValue(key);
            return value != null;
        }

        public static void SetPref(string key, string value) {
            RegistryKey myKey = Application.UserAppDataRegistry;

            myKey.SetValue(key, value);
            myKey.Flush();

        }
        public static void SetPref(string key, double value) {
            RegistryKey myKey = Application.UserAppDataRegistry;

            myKey.SetValue(key, value.ToString());
            myKey.Flush();

        }

        public static double GetPref(string key, double defaultValue) {
            RegistryKey myKey = Application.UserAppDataRegistry;

            return double.Parse((string)myKey.GetValue(key, defaultValue.ToString()));            
        }

        public static void ClearAllSettings() {
            RegistryKey myKey = Application.UserAppDataRegistry;
            foreach (var key in myKey.GetValueNames()) {
                myKey.DeleteValue(key);                
            }
            myKey.Flush();
        }

    }
}
