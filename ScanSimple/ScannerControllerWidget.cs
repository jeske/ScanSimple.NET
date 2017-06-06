using System;
using Xwt;
using Xwt.Drawing;
using System.Drawing;
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
            this.Content = outerBox;

            outerBox.PackStart(scannerImage = new ImageView(Xwt.Drawing.Image.FromResource("ScanSimple.icons.scanner.jpg")));                 
            outerBox.PackStart(scanButton = new Button("Scan"), expand:true, fill:false);
            scanButton.Clicked += ScanButton_Clicked;

            scannerController = new ScannerController(this);           
        }

        public void HandleScannerData(Bitmap image) {
            MemoryStream ms = new MemoryStream();
            image.Save(ms,System.Drawing.Imaging.ImageFormat.Bmp);
            outerBox.PackStart(new ImageView(Xwt.Drawing.Image.FromStream(ms)));
        }

        private void ScanButton_Clicked(object sender, EventArgs e) {
            mainWindow.AddScanSession(new ScanSessionWidget());            
        }
    }
}
