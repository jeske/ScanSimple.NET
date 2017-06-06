// ScanSimple
// Copyright (C) 2017, by David W. Jeske, all Rights Reserved

using System;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace ScanWorker
{

    public interface IScanWorker
    {
        bool ping(); // so we can check if the worker is responding
        void RegisterMaster(IScanMaster scanMaster);
        void Exit(); // to tell us to shutdown cleanly
        
        void StartScanning();
        void SelectSource();
    }

    public interface IScanMaster
    {
        bool pong(); // so we can check if the master is responding
        void ScanDataTransfer(byte[] scanBitmap);        
    }

    public class ScanWorker : MarshalByRefObject, IScanWorker
    {
        MainForm mainForm;

        public ScanWorker(MainForm mainForm) {
            this.mainForm = mainForm;
        }
        // make sure our singleton lives forever in .NET remoting
        public override object InitializeLifetimeService() {
            return (null);
        }


        #region Network API

        public bool ping() {
            Console.WriteLine("ping on ThreadId: " + Thread.CurrentThread.ManagedThreadId);
            mainForm.Invoke((MethodInvoker) delegate { this.UiThread_ping(); } );
            return true;
        }

        private void UiThread_ping() {
            Console.WriteLine("_ping on ThreadId: " + Thread.CurrentThread.ManagedThreadId);
        }

        public void RegisterMaster(IScanMaster scanMaster) {
            scanMaster.pong();
            mainForm.Invoke((MethodInvoker)delegate { 
                mainForm.RegisterMaster(scanMaster);
            });
        }

        public void Exit() {
            Console.WriteLine("Received Shutdown Request");
            mainForm.Invoke((MethodInvoker)delegate {
                Console.WriteLine("Shutting Down...");
                Application.Exit();
            });
        }


        public void StartScanning() {
            mainForm.Invoke((MethodInvoker)delegate {
                mainForm.StartScanning();
            });
        }

        public void SelectSource() {
            if (!mainForm.twain_hook.UseFilter) {
                mainForm.Invoke((MethodInvoker)delegate {
                    mainForm.SelectSource();
                });
            }
        }

        #endregion
    }

}
