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
using System.Data.SqlClient;

namespace PopulateAnalysisDB
{
    public partial class Form1 : Form
    {
        List<string> Games;
        int curGame;

        public Form1()
        {
            InitializeComponent();
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            if( openFileDialog1.ShowDialog() == DialogResult.OK )
            {
                fileLabel.Text = openFileDialog1.FileName;
            }
        }

        private void textToDBButton_Click(object sender, EventArgs e)
        {
            PullGamesFromFile();
            WriteGamesToDB();
        }
        void PullGamesFromFile()
        {
            StreamReader tr = new StreamReader(fileLabel.Text);

            Games = new List<string>();

            string GameString = "";
            bool inGame = false;
            bool inHeader = false;

            while (!tr.EndOfStream)
            {
                string l = tr.ReadLine();
                bool newTag = (l.Length > 1 && l[0] == '[' && l[1] != '%');   // it's a new game;
                if (inGame)
                    if (!inHeader)
                        if (newTag)   // it's a new game;
                        {
                            Games.Add(GameString);
                            GameString = "";
                            inGame = false;
                        }
                        else
                            GameString += l;
                    else
                    {
                        if (!newTag)   // not in header anymore
                            inHeader = false;
                        GameString += l + Environment.NewLine;
                    }
                if (!inGame)
                    if (newTag)   // it's a new game;
                    {
                        inGame = true;
                        inHeader = true;
                        GameString += l + Environment.NewLine;
                    }
            }
            if (inGame)
                Games.Add(GameString);
            nbrGamesLabel.Text = Games.Count.ToString() + " games loaded";
            curGame = 0;
            DisplayGameText();
        }
        void WriteGamesToDB()
        {
            string connStr = "Data Source=DEMASTRI-W10-05\\JPD_SQL17_01;Initial Catalog=ChessAnalysis;Integrated Security=True";
            SqlConnection sConnn = new SqlConnection(connStr);
            sConnn.Open();
            SqlCommand sCmd = sConnn.CreateCommand();
            string SQL;

            // GameFiles
            Guid fileID = Guid.NewGuid();
            SQL = "Insert into GameFiles (FileID, Location) Values('" + fileID.ToString() + "', '"+ fileLabel.Text + "')";
            sCmd.CommandText = SQL;
            sCmd.ExecuteNonQuery();
            
            // Games
            foreach (string s in Games)
            {
                string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                Guid gameID = Guid.NewGuid();
                SQL = "Insert into Games (GameID, FileID, PGNText, StartingFEN) Values('" + gameID.ToString() + "', '" + fileID.ToString() + "', '"+s+"', '"+startingFEN+"')";
                sCmd.CommandText = SQL;
                sCmd.ExecuteNonQuery();
            }
            sConnn.Close();
        }


        void DisplayGameText()
        {
            pgnTextBox.Text = Games[curGame];
            GameLabel.Text = (curGame + 1).ToString() + " of " + Games.Count.ToString();
        }

        private void prevGame_Click(object sender, EventArgs e)
        {
            if (--curGame < 0)
                curGame = Games.Count - 1;
            DisplayGameText();
        }

        private void nextGame_Click(object sender, EventArgs e)
        {
            if (++curGame >= Games.Count)
                curGame = 0;
            DisplayGameText();
        }
    }
}
