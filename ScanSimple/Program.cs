using System;
using Xwt;
using Xwt.Drawing;


namespace ScanSimple
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {
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
