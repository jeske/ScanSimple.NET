// ScanSimple
// Copyright (C) 2017, by David W. Jeske, all Rights Reserved

using System;
using Xwt;
using Xwt.Drawing;


namespace ScanSimple
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {
            // Prefs.ClearAllSettings();

            MainWindow w = null;
            try { 
                Application.Initialize(ToolkitType.Wpf);

                w = new MainWindow();
                w.Show();

                Application.Run();

            } finally {
                if (w != null) {
                    w.Dispose();
                }
                Application.Dispose();
            }
        }
    }
}
