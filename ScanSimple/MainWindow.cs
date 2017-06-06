// ScanSimple
// Copyright (C) 2017, by David W. Jeske, all Rights Reserved

using System;
using Xwt;
using Xwt.Drawing;
using System.Xml;
using System.Reflection;
using System.Threading;

namespace ScanSimple
{
    class MainWindow : Window
    {
        VBox topPanel;
        ScannerControllerWidget scanControllerWidget;
        VBox scanSessionsVBox;
        ScrollView mainScrollView;
        public ImageView imageView;

        protected override void OnClosed() {
            ThreadPool.QueueUserWorkItem(delegate (object o) {
                scanControllerWidget.scannerController.ShutdownScanWorker();
            },null);
            Application.Exit();
        }
        
        protected override void OnBoundsChanged(BoundsChangedEventArgs evt) {
            if (evt.Bounds.Width < 400) { Width = 400; }
            if (evt.Bounds.Height < 400) { Height = 400; }                        

            Prefs.SetPref("MainWindow.Width", evt.Bounds.Width);
            Prefs.SetPref("MainWindow.Height", evt.Bounds.Height);
            Prefs.SetPref("MainWindow.X",evt.Bounds.X);
            Prefs.SetPref("MainWindow.Y",evt.Bounds.Y);
            base.OnBoundsChanged(evt);
        }

        public MainWindow() {
            Title = "ScanSimple";          
            Width = Prefs.GetPref("MainWindow.Width",800);
            Height = Prefs.GetPref("MainWindow.Height", 800);            

            if (Prefs.HasPref("MainWindow.X")) {
                X = Prefs.GetPref("MainWindow.X", 100);
                Y = Prefs.GetPref("MainWindow.Y", 100);
            }
            
            // TODO: check that our window is not too small and on the visible screen

            // Get this type's assembly
            Assembly assem = this.GetType().Assembly;

            // Enumerate the assembly's manifest resources
            foreach (string resourceName in assem.GetManifestResourceNames()) {
                Console.WriteLine(resourceName);
            }

            this.Content = topPanel = new VBox();
            topPanel.PackStart(scanControllerWidget = new ScannerControllerWidget(this));
            topPanel.PackStart(mainScrollView = new ScrollView(),expand:true);
            { 
                var panel = new VBox();
                mainScrollView.Content = panel;
                mainScrollView.VerticalScrollPolicy = ScrollPolicy.Always;

                panel.PackStart(scanSessionsVBox = new VBox());
                panel.PackStart(new Canvas(),true);

                panel.PackStart(imageView = new ImageView(),expand:true);
            }
            
        }

        public void AddScanSession(ScanSessionWidget scanSession) {
            scanSessionsVBox.PackStart(scanSession);
        }
    }

}
