﻿#define useV2

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#if useV2
using ChessPosition.V2;
#else
using ChessPosition;
#endif

namespace PGNViewer
{
    public partial class MoveEditor : Form
    {
        int plyNumber; 
        public MoveEditor()
        {
            InitializeComponent();
        }
        public void Init(Ply thisPly)
        {
            plyNumber = thisPly.Number;

            MoveNbrLabel.Text = thisPly.MoveNumber.ToString() + "." + (thisPly.Number % 2 == 0 ? " ..." : "");
#if !useV2
            CorrMoveText.Text = thisPly.refToken.value;
#endif
            CorrMoveTime.Value = PGNViewer.CommentTime(thisPly.comments);
            penaltyTime.Text = PGNViewer.PenaltyTime(thisPly.comments).ToString();
            if (penaltyTime.Text == "")
                penaltyTime.Text = "0";
        }
        public Ply Extract()
        {
            Ply outPly = new Ply();
#if useV2
            outPly.comments.Add(new Comment(false, CorrMoveTime.Value.ToString("MM/dd/yyyy HHmm")));
            if (penaltyTime.Text != "0")
                if (plyNumber == 0)
                    outPly.comments.Add(new Comment(false, "TimeAtStart:" + Convert.ToInt32(penaltyTime.Text)));
                else
                    outPly.comments.Add(new Comment(false, "PenaltyDays:" + Convert.ToInt32(penaltyTime.Text)));
#else
            outPly.comments.Add(new PGNComment(CorrMoveTime.Value.ToString("{MM/dd/yyyy HHmm}")));
            if (penaltyTime.Text != "0")
                if (plyNumber == 0)
                    outPly.comments.Add(new PGNComment("{TimeAtStart:" + Convert.ToInt32(penaltyTime.Text)+"}"));
                else
                    outPly.comments.Add(new PGNComment("{PenaltyDays:" + Convert.ToInt32(penaltyTime.Text) + "}"));
#endif
            return outPly;
        }

        private void EditorNowButton_Click(object sender, EventArgs e)
        {
            CorrMoveTime.Value = DateTime.Now;
            OKButton.Select();
        }

    }
}
