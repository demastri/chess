using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Drawing.Text;

namespace CorrMgr
{
    public partial class _Default : Page
    {
        private PrivateFontCollection pfc;

        protected void Page_Load(object sender, EventArgs e)
        {
            DrawBoard();
        }

        private void DrawBoard()
        {
            double limitingSize = boardDisplay.Width.Value < boardDisplay.Height.Value ? boardDisplay.Width.Value : boardDisplay.Height.Value;
            double fontFactor = 13.75;
            int fontSize = (int)(limitingSize / fontFactor);

            string emptyBoard =
                  "!\"\"\"\"\"\"\"\"#" + Environment.NewLine // top line
                + "ç + + + +%" + Environment.NewLine    // h-rank with rank ID
                + "æ+ + + + %" + Environment.NewLine
                + "å + + + +%" + Environment.NewLine
                + "ä+ + + + %" + Environment.NewLine
                + "ã + + + +%" + Environment.NewLine
                + "â+ + + + %" + Environment.NewLine
                + "á + + + +%" + Environment.NewLine
                + "à+ + + + %" + Environment.NewLine    // a-rank with rank ID
                + "/èéêëìíîï)" + Environment.NewLine;    // bottom line w/fileID


            string thisBoard = emptyBoard;
            //if (curGame != null)
            //{
            //    foreach (Square sq in curGame.CurrentPosition.board.Keys)
            //    {
            //        thisBoard = PokePiece(thisBoard, sq.row + 1, sq.col + 1, curGame.CurrentPosition.board[sq]);
            //    }
            //    FENText.Text = curGame.ToFEN();
            //}
            //else
            //    FENText.Text = "";

            boardDisplay.Text = thisBoard;
        }


    }
}