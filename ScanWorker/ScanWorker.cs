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

    }

    public class ScanWorker : MarshalByRefObject, IScanWorker
    {
        MainForm mainForm;
        public bool ping() {
            Console.WriteLine("ping on ThreadId: " + Thread.CurrentThread.ManagedThreadId);
            mainForm.Invoke((MethodInvoker) delegate { this._ping(); } );
            return true;
        }

        private void _ping() {
            Console.WriteLine("_ping on ThreadId: " + Thread.CurrentThread.ManagedThreadId);
        }

        public string doSomething(string test) {
            Console.WriteLine("Server received: {0}", test);


            return String.Format("Server did: {0}", test);
        }
        public ScanWorker(MainForm mainForm) {
            this.mainForm = mainForm;
        }

        // make sure our singleton lives forever in .NET remoting
        public override object InitializeLifetimeService() {
            return (null);
        }
    }

}
