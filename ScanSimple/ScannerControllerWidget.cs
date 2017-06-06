using System;
using Xwt;
using Xwt.Drawing;
using System.IO;
using System.Threading;

namespace ScanSimple
{
    class ScannerControllerWidget : Widget
    {
        internal MainWindow mainWindow;        

        ImageView scannerImage;
        Button scanButton;        
        Button selectSourceButton;
        public ScannerController scannerController;
        HBox outerBox;

        public ScannerControllerWidget(MainWindow myWindow) {
            this.mainWindow = myWindow;            

            outerBox = new HBox();
            outerBox.BackgroundColor = Color.FromBytes(240,240,240);
            this.Content = outerBox;

            outerBox.PackStart(scannerImage = new ImageView(Xwt.Drawing.Image.FromResource("ScanSimple.icons.scanner.jpg")));                 


            outerBox.PackStart(selectSourceButton = new Button("Select Source"),expand:true,fill:true);
            selectSourceButton.Clicked += SelectSourceButton_Clicked;

            outerBox.PackStart(scanButton = new Button("Scan"), expand:true, fill:false);
            scanButton.Clicked += ScanButton_Clicked;

            scannerController = new ScannerController(this);           
        }

        private void SelectSourceButton_Clicked(object sender, EventArgs e) {
            ThreadPool.QueueUserWorkItem(delegate(object o) {
                scannerController.scanWorker.SelectSource();
            }, null);            
        }
        
        private void ScanButton_Clicked(object sender, EventArgs e) {
            mainWindow.AddScanSession(new ScanSessionWidget());            
            scannerController.scanWorker.StartScanning();
        }

        public void HandleScannerData(byte[] image) {
            var imageView = new ImageView();
            imageView.Margin = 0.3;
            imageView.MinHeight = 200;
            imageView.MinWidth = 200;
            imageView.TooltipText = "image";
            outerBox.PackStart(imageView, expand: true, fill: true);

            try {
                MemoryStream ms = new MemoryStream(image);
                imageView.Image = Xwt.Drawing.Image.FromStream(ms);
            }
            catch (Exception e) {
                Console.WriteLine("exception: " + e.ToString());
            }
        }


    }
}
