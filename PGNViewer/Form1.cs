using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ChessPosition;

namespace PGNViewer
{
    public partial class PGNViewer : Form
    {
        public PGNViewer()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string PGNLoc = openFileDialog1.FileName;
                if (PGNLoc != "" && File.Exists(PGNLoc))
                {
                    StreamReader tr = new StreamReader(PGNLoc);
                    GameList.Items.Clear();
                    
                    Game g;
                    while ((g = Game.ReadGame(tr)) != null)
                    {
                        GameList.Items.Add(g.GameDate+" "+g.PlayerWhite+"-"+g.PlayerBlack);
                    }
                }
            }
        }
    }
}
