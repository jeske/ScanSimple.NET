// ScanSimple
// Copyright (C) 2017, by David W. Jeske, all Rights Reserved

using System;
using System.Drawing;
using ScanWorker;

namespace ScanSimple
{
    class ScanMaster : MarshalByRefObject, IScanMaster
    {
        MainWindow mainWindow;
        ScannerControllerWidget scanControllerWidget;

        public ScanMaster(MainWindow mainWindow, ScannerControllerWidget scanControllerWidget) {
            this.mainWindow = mainWindow;
            this.scanControllerWidget = scanControllerWidget;
        }
        // make sure our singleton lives forever in .NET remoting
        public override object InitializeLifetimeService() {
            return (null);
        }

        #region IScanMaster remote calls
        public bool pong() {
            Console.WriteLine("master pong()");
            return true;
        }

        public void ScanDataTransfer(byte[] image) {
            Console.WriteLine("master received image bytes: " + image.Length);
            mainWindow.InvokeAsync((Action)delegate {
                scanControllerWidget.HandleScannerData(image);
            });
        }

        #endregion
    }
}
