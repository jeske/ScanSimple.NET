﻿// ScanSimple
// Copyright (C) 2017, by David W. Jeske, all Rights Reserved

using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

using System.Collections;
using System.Diagnostics;
using System.IO;

using System.Threading;

using ScanWorker;

namespace ScanSimple
{

    class ScannerController
    {
        ScannerControllerWidget scanControllerWidget;
        public IScanWorker scanWorker;
        IScanMaster scanMaster;
        Process workerProcess;

        public ScannerController(ScannerControllerWidget scanControllerWidget) {
            this.scanControllerWidget = scanControllerWidget;

            // connect to the scanner controller
            var serverProvider = new BinaryServerFormatterSinkProvider();
            var clientProvider = new BinaryClientFormatterSinkProvider();
            serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            var props = new Hashtable();
            props["name"] = "ipc";
            props["portName"] = ScanWorker.Program.APP_NAME + "_CLIENT";

            IpcChannel clientChannel = new IpcChannel(props,clientProvider,serverProvider);
            ChannelServices.RegisterChannel(clientChannel);

            scanWorker = 
                (IScanWorker) Activator.GetObject(
                    typeof(ScanWorker.ScanWorker),
                    "ipc://ScanSimple/ScanWorker");            

            try {
                // try to connect to existing scan worker process first (for debugging)
                bool alive = scanWorker.ping();
                Console.WriteLine("scanworker ping() output = " + alive);
            } catch (System.Runtime.Remoting.RemotingException e) {
                // if we fail, launch a scan worker...
                LaunchScanWorker();
                for (int i=0;i<10;i++) {
                    Thread.Sleep(50);
                    // and try to connect again
                    try {
                        bool alive2 = scanWorker.ping();
                        Console.WriteLine("scanworker ping() output = " + alive2);
                        break;
                    } catch (System.Runtime.Remoting.RemotingException e2) {
                        // try again.
                        Console.WriteLine("failed to connect to ScanWorker, trying again...");
                    }
                }

                // if we didn't connect, this will throw an exception
                bool alive = scanWorker.ping();

                // now create our scan master listener, and send it over
                scanMaster = new ScanMaster(scanControllerWidget.mainWindow, scanControllerWidget);
                scanWorker.RegisterMaster(scanMaster);
            }            

        }


        bool Exiting = false;
        public void ShutdownScanWorker() {
            Exiting = true;
            if (scanWorker != null) {                
                try { 
                    scanWorker.Exit(); // send Exit message
                } catch (Exception e) {
                    Console.WriteLine("Exception sending ScanWorker.Exit(): " + e.ToString());
                }
                
                scanWorker = null;
            }
            if (workerProcess != null) {
                // try to wait for the worker to shutdown cleanly
                for (int i=0;i<10;i++) {
                    Thread.Sleep(10);
                    if (workerProcess.HasExited) {
                        Console.WriteLine("worker exited cleanly: " + workerProcess.ExitCode);
                        break;
                    }
                }   
                if (!workerProcess.HasExited) {
                    workerProcess.Kill();
                }
            }
        }

        private void LaunchScanWorker() {

            // find the directory we were launched from, so we can find ScanWorker.EXE
            // https://stackoverflow.com/questions/837488/how-can-i-get-the-applications-path-in-a-net-console-application
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            
            string workerEXEPath = Path.Combine(baseDirectory,"ScanWorker.EXE");
            if (!File.Exists(workerEXEPath)) {
                throw new Exception("no ScanWorker.EXE at: " + workerEXEPath);
            }

            // launch the worker process                       
            // workerProcess = Process.Start(workerEXEPath);

            workerProcess = new Process();
            workerProcess.StartInfo.UseShellExecute = false;
            workerProcess.StartInfo.FileName = workerEXEPath;
            workerProcess.StartInfo.RedirectStandardOutput = true;
            workerProcess.OutputDataReceived += WorkerProcess_OutputDataReceived;
            workerProcess.Exited += WorkerProcess_Exited;
            workerProcess.Start();
            workerProcess.BeginOutputReadLine();

            // we might want to catch the standard ouput and do something with it
            // https://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.redirectstandardoutput(v=vs.110).aspx

            // make sure the child process dies when we die, using a Job object
            // https://stackoverflow.com/questions/3342941/kill-child-process-when-parent-process-is-killed
            ChildProcessTracker.AddProcess(workerProcess);
        }

        private void WorkerProcess_Exited(object sender, EventArgs e) {
            if (!Exiting) {
                Console.WriteLine("ScanWorker Exited Unexpectedly: " + workerProcess.ExitCode);
            }
        }

        private void WorkerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e) {            
            if (e.Data == null) {
                Console.WriteLine("ScanWorker Exited: " + workerProcess.ExitCode);
            } else {
                Console.WriteLine("Worker: " + e.Data);
            }
        }
    }
}
