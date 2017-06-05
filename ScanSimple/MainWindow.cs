using System;
using Xwt;
using Xwt.Drawing;
using System.Xml;
using System.Reflection;

namespace ScanSimple
{
    class MainWindow : Window
    {
        VBox topPanel;
        ScannerControllerWidget scanControllerWidget;
        VBox scanSessionsVBox;
        ScrollView mainScrollView;

        protected override void OnClosed() {
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

            }
        }

        public void AddScanSession(ScanSessionWidget scanSession) {
            scanSessionsVBox.PackStart(scanSession);
        }
    }

}
