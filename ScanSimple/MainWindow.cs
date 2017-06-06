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

        public MainWindow() {
            Title = "ScanSimple";            

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
