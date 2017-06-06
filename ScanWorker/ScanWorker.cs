using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Windows.Forms;

namespace ScanWorker
{

    public interface IScanWorker
    {
        bool ping(); // so we can check if the server is responding
        void Exit(); // to tell us to shutdown cleanly

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

        public void Exit() {
            Console.WriteLine("Received Shutdown Request");
            mainForm.Invoke((MethodInvoker)delegate {
                Console.WriteLine("Shutting Down...");
                Application.Exit();
            });
        }

        #endregion
    }

}
