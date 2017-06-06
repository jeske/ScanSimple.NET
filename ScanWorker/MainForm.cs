// ScanSimple
// Copyright (C) 2017, by David W. Jeske, all Rights Reserved

using System;
using System.Windows.Forms;
using TwainDotNet;
using TwainDotNet.WinFroms;

using System.IO;

namespace ScanWorker
{
    public partial class MainForm : Form
    {
        Twain _twain;
        ScanSettings _settings;
        IScanMaster scanMaster;
        public WinFormsWindowMessageHook twain_hook;

        public MainForm() {
            InitializeComponent();
            this.Text = "ScanWorker";
            this.WindowState = FormWindowState.Minimized;
            
            // when we are ready, we can hide the worker from the taskbar
            // this.ShowInTaskbar = false;

            _twain = new Twain(twain_hook = new WinFormsWindowMessageHook(this));
        }

        public void RegisterMaster(IScanMaster scanMaster) {
            this.scanMaster = scanMaster;
            _twain.TransferImage += delegate (Object sender, TransferImageEventArgs args) {
                Console.WriteLine("sending Scanned Image...");
                if (args.Image != null) {
                    if (scanMaster != null) {
                        MemoryStream ms = new MemoryStream();
                        args.Image.Save(ms,System.Drawing.Imaging.ImageFormat.Bmp);
                        scanMaster.ScanDataTransfer(ms.ToArray());
                    }
                }
            };
        }

        public void StartScanning() {
            _settings = new ScanSettings();         
            _settings.ShowTwainUI = false;
            _settings.ShowProgressIndicatorUI = false;
            /*
            _settings.UseDocumentFeeder = useAdfCheckBox.Checked;
            _settings.ShowTwainUI = useUICheckBox.Checked;
            _settings.ShowProgressIndicatorUI = showProgressIndicatorUICheckBox.Checked;
            _settings.UseDuplex = useDuplexCheckBox.Checked;
            _settings.Resolution =
                blackAndWhiteCheckBox.Checked
                ? ResolutionSettings.Fax : ResolutionSettings.ColourPhotocopier;
            _settings.Area = !checkBoxArea.Checked ? null : AreaSettings;
            _settings.ShouldTransferAllPages = true;

            _settings.Rotation = new RotationSettings() {
                AutomaticRotate = autoRotateCheckBox.Checked,
                AutomaticBorderDetection = autoDetectBorderCheckBox.Checked
            };
            */

            try {
                _twain.StartScanning(_settings);
            }
            catch (TwainException ex) {
                MessageBox.Show(ex.Message);
                Enabled = true;
            }
        }

        public void SelectSource() {
            if (this.CanFocus) {
                Console.WriteLine("SelectSource()");
                _twain.SelectSource();
            }
        }

        protected override void OnClosed(EventArgs e) {
            Console.WriteLine("ScanWorker: OnClosed: Exiting.");
            Application.Exit();            
        }
    }
}
