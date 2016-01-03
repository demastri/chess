using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace PGNViewer
{
    public partial class CorrResponseDialog : Form
    {
        public CorrResponseDialog()
        {
            InitializeComponent();
        }

        public void SendToClipboard_Click(object sender, EventArgs e)
        {
            ClipboardHelper.CopyToClipboard(htmlView.DocumentText, htmlView.DocumentText);
        }

        private void CorrResponseDialog_Shown(object sender, EventArgs e)
        {
            SendToClipboard_Click(null, null);
        }
    }
}
