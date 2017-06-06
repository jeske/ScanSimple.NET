using System;
using Xwt;
using Xwt.Drawing;
using System.IO;

namespace ScanSimple
{
    class ScannerControllerWidget : Widget
    {
        internal MainWindow mainWindow;        

        ImageView scannerImage;
        Button scanButton;        
        public ScannerController scannerController;
        HBox outerBox;

        public ScannerControllerWidget(MainWindow myWindow) {
            this.mainWindow = myWindow;            

            outerBox = new HBox();
            outerBox.BackgroundColor = Color.FromBytes(240,240,240);
            this.Content = outerBox;

            outerBox.PackStart(scannerImage = new ImageView(Xwt.Drawing.Image.FromResource("ScanSimple.icons.scanner.jpg")));                 
            outerBox.PackStart(scanButton = new Button("Scan"), expand:true, fill:false);
            scanButton.Clicked += ScanButton_Clicked;

            scannerController = new ScannerController(this);           
        }

        public void HandleScannerData(byte[] image) {            
            var imageView = new ImageView();
            imageView.Margin = 0.3;
            imageView.MinHeight = 200;
            imageView.MinWidth = 200;
            imageView.TooltipText = "image";
            outerBox.PackStart(imageView,expand:true,fill:true);            
            
            try {
                MemoryStream ms = new MemoryStream(image);
                imageView.Image = Xwt.Drawing.Image.FromStream(ms);            
            } catch (Exception e) {
                Console.WriteLine("exception: " + e.ToString());
            }
        }

        private void ScanButton_Clicked(object sender, EventArgs e) {
            mainWindow.AddScanSession(new ScanSessionWidget());            
            scannerController.scanWorker.StartScanning();
        }
    }
}
