using System;
using Xwt;
using Xwt.Drawing;
using System.Xml;

namespace ScanSimple
{
    class ScannerControllerWidget : Widget
    {
        internal MainWindow myWindow;

        ImageView scannerImage;
        Button scanButton;        
        ScannerController scannerController;

        public ScannerControllerWidget(MainWindow myWindow) {
            this.myWindow = myWindow;
            var outerBox = new HBox();
            this.Content = outerBox;

            outerBox.PackStart(scannerImage = new ImageView(Image.FromResource("ScanSimple.icons.scanner.jpg")));                 
            outerBox.PackStart(scanButton = new Button("Scan"), expand:true, fill:false);
            scanButton.Clicked += ScanButton_Clicked;

            scannerController = new ScannerController(this);
            

        }

        private void ScanButton_Clicked(object sender, EventArgs e) {
            myWindow.AddScanSession(new ScanSessionWidget());            
        }
    }
}
