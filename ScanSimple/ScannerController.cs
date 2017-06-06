using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

using System.Diagnostics;
using System.IO;

using System.Threading;

using ScanWorker;

namespace ScanSimple
{

    class ScannerController
    {
        ScannerControllerWidget myWidget;
        IScanWorker scanWorker;
        Process workerProcess;

        public ScannerController(ScannerControllerWidget myWidget) {
            this.myWidget = myWidget;

            // connect to the scanner controller
            IpcChannel clientChannel = new IpcChannel();
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
            }            

        }

        public void ShutdownScanWorker() {
            if (scanWorker != null) {
                scanWorker.Exit(); // send Exit message
                scanWorker = null;
            }
            if (workerProcess != null) {
                // try to wait for the worker to shutdown cleanly
                for (int i=0;i<20;i++) {
                    Thread.Sleep(40);
                    if (workerProcess.HasExited) {
                        Console.WriteLine("worker exited cleanly: " + workerProcess.ExitCode);
                        break;
                    }
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
            workerProcess = Process.Start(workerEXEPath);

            // make sure the child process dies when we die, using a Job object
            // https://stackoverflow.com/questions/3342941/kill-child-process-when-parent-process-is-killed
            ChildProcessTracker.AddProcess(workerProcess);
        }
    }
}
