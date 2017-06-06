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

        public MainForm() {
            InitializeComponent();
            this.Text = "ScanWorker";

            _twain = new Twain(new WinFormsWindowMessageHook(this));
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            Application.Exit();
        }
    }
}
