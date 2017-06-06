using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

using ScanWorker;

namespace ScanSimple
{

    class ScannerController
    {
        ScannerControllerWidget myWidget;
        public ScannerController(ScannerControllerWidget myWidget) {
            this.myWidget = myWidget;

            // connect to the scanner controller
            IpcChannel clientChannel = new IpcChannel();
            ChannelServices.RegisterChannel(clientChannel);

            IScanWorker scanWorker = 
                (IScanWorker) Activator.GetObject(
                    typeof(ScanWorker.ScanWorker),
                    "ipc://ScanSimple/ScanWorker");

            bool alive = scanWorker.ping();

            Console.WriteLine("scanworker ping() output = " + alive);

        }
    }
}
