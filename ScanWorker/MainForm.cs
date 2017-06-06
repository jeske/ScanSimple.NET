using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TwainDotNet;
using TwainDotNet.WinFroms;

namespace ScanWorker
{
    public partial class MainForm : Form
    {
        Twain _twain;
        ScanSettings _settings;
        IScanMaster scanMaster;

        public MainForm() {
            InitializeComponent();
            this.Text = "ScanWorker";

            _twain = new Twain(new WinFormsWindowMessageHook(this));
        }

        public void RegisterMaster(IScanMaster scanMaster) {
            this.scanMaster = scanMaster;
            _twain.TransferImage += delegate (Object sender, TransferImageEventArgs args) {
                if (args.Image != null) {
                    if (scanMaster != null) {
                        scanMaster.ScanDataTransfer(args.Image);
                    }
                }
            };
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            Application.Exit();
        }
    }
}
