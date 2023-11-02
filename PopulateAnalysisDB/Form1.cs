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

using ChessPosition.V2;
using ChessPosition.V2.PGN;
using ChessPosition.V2.Db;

namespace PopulateAnalysisDB
{
    public partial class Form1 : Form
    {
        private static string defaultGrammarFileLocation = "Parser/Grammars/PGNSchema.xml";

        List<Game> Games;
        List<AnalysisPosition> Positions;
        class AnalysisPosition
        {
            public Guid hostGameID;
            public Position thisPos;
            public Game.Terminators terminator;
        }

        string connStr = "Data Source=DEMASTRI-W10-05\\JPD_SQL17_01;Initial Catalog=ChessAnalysis;Integrated Security=True";

        public void LoadGameFromPGN(string pgn)
        {
            pgn = StripComments(pgn);
            pgn = StripMovePlaceholders(pgn);

            PGNTokenizer nextTokenSet = new PGNTokenizer(pgn, defaultGrammarFileLocation);
            for (int i = 0; i < nextTokenSet.GameCount; i++)
            {
                nextTokenSet.LoadGame(i);

                string tokenStr = "";
                foreach (PGNToken pgnt in nextTokenSet.tokens)
                    tokenStr += pgnt.ToString();

                Games.Add(new PGNGame(nextTokenSet));
            }
        }

        int curGame;

        public Form1()
        {
            InitializeComponent();
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileLabel.Text = openFileDialog1.FileName;
            }
        }

        private void textToDBButton_Click(object sender, EventArgs e)
        {
            PullGamesFromFile();
            //WriteGamesToDB();
        }
        void PullGamesFromFile()
        {
            StreamReader tr = new StreamReader(fileLabel.Text);

            string HeaderString = "";
            string GameString = "";

            bool inGame = false;
            bool inHeader = false;

            Games = new List<Game>();
            while (!tr.EndOfStream)
            {
                string l = tr.ReadLine();
                bool newTag = (l.Length > 1 && l[0] == '[' && l[1] != '%');   // it's a new game;
                if (inGame)
                    if (!inHeader)
                        if (newTag)   // it's a new game;
                        {
                            LoadGameFromPGN(HeaderString + "\n" + GameString);  // so clean the old one up

                            HeaderString = GameString = "";
                            inGame = false;
                        }
                        else
                            GameString += l + " ";
                    else
                    {
                        if (!newTag)   // not in header anymore
                        {
                            GameString = "";
                            inHeader = false;
                        }
                        HeaderString += l + " ";
                    }
                if (!inGame)
                    if (newTag)   // it's a new game;
                    {
                        inGame = true;
                        inHeader = true;
                        HeaderString += l + " ";
                    }
            }
            if (inGame)
            {
                LoadGameFromPGN(HeaderString + "\n" + GameString);  // so clean the last one up
            }
            nbrGamesLabel.Text = Games.Count.ToString() + " games loaded";
            curGame = 0;
            DisplayGameText();
        }
        void WriteGamesToDB()
        {
            SqlConnection sConnn = new SqlConnection(connStr);
            sConnn.Open();
            SqlCommand sCmd = sConnn.CreateCommand();
            string SQL;

            // GameFiles
            Guid fileID = Guid.NewGuid();
            SQL = "Insert into GameFiles (FileID, Location) Values('" + fileID.ToString() + "', '" + fileLabel.Text + "')";
            sCmd.CommandText = SQL;
            sCmd.ExecuteNonQuery();

            // Games
            Dictionary<PGNGame, Guid> gameIDs = new Dictionary<PGNGame, Guid>();
            foreach (PGNGame g in Games)
            {
                string pgnText = g.PGNSource;
                g.ResetPosition();

                Guid gameID = Guid.NewGuid();
                gameIDs[g] = gameID;
                SQL = "Insert into Games (GameID, FileID, PGNText, StartingFEN, Terminator) Values('" + gameID.ToString() + "', '" + fileID.ToString() + "', '" + pgnText.Replace("'", "''") + "', '" + g.ToFEN() + "', '" + g.TerminatorString + "')";
                sCmd.CommandText = SQL;
                sCmd.ExecuteNonQuery();

                curGame = 0;
                DisplayGameText();
            }

            // STR detail
            foreach (PGNGame g in Games)
            {
                string Event = g.Tags["Event"].Replace("'", "''");
                string Site = g.Tags["Site"].Replace("'", "''");
                string Date = g.Tags["Date"];
                string Round = g.Tags["Round"];
                string White = g.PlayerWhite.Replace("'", "''");
                string Black = g.PlayerBlack.Replace("'", "''");
                string WhiteRating = g.RatingWhite.ToString();
                string BlackRating = g.RatingBlack.ToString();

                Guid rosterID = Guid.NewGuid();
                SQL = "Insert into SevenTagRoster (TagRosterID, GameID, Event, Site, Date, Round, White, Black, WhiteRating, BlackRating) Values('" +
                    rosterID.ToString() + "', '" +
                    gameIDs[g].ToString() + "', '" +
                    Event + "', '" +
                    Site + "', '" +
                    Date + "', '" +
                    Round + "', '" +
                    White + "', '" +
                    Black + "', '" +
                    WhiteRating + "', '" +
                    BlackRating + "'" +
                    ")";
                sCmd.CommandText = SQL;
                sCmd.ExecuteNonQuery();
            }

            // positions
            // generate positions from games
            // only interested in the mainline variation
            foreach (PGNGame g in Games)
            {
                Guid variationID = Guid.NewGuid();
                SQL = "insert into Variations (VariationID, GameID, ForkPosition, OrderIndex) VALUES ('" + variationID.ToString() + "', '" + gameIDs[g].ToString() + "', NULL, 1 )";
                sCmd.CommandText = SQL;
                sCmd.ExecuteNonQuery();

                SQL = "update Games set MainLine = '" + variationID.ToString() + "' where GameID = '" + gameIDs[g].ToString() + "'";
                sCmd.CommandText = SQL;
                sCmd.ExecuteNonQuery();

                foreach (Ply p in g.Plies)
                {
                    string plyStr = PGNPly.CreatePlyMove(g.CurrentPosition, p);
                    g.AdvancePosition();
                    // write this position to the database
                    SQL = "insert into positions (PositionID, VariationID, MoveText, Ply, ResultingFEN) VALUES( " +
                        "'" + Guid.NewGuid().ToString() + "', " +
                        "'" + variationID.ToString() + "', " +
                        "'" + plyStr + "', " +
                        "" + (p.Number + 1).ToString() + ", " +
                        "'" + g.ToFEN() + "'" +
                        ")";
                    sCmd.CommandText = SQL;
                    sCmd.ExecuteNonQuery();
                }
            }
            sConnn.Close();
        }

        void DisplayGameText()
        {
            PGNGame pg = new PGNGame(Games[curGame]);
            pgnTextBox.Text = pg.PGNSource;
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

        string StripMovePlaceholders(string o)
        {
            int i = -1;
            while ((i = o.IndexOf("...")) >= 0)
            {
                o = o.Substring(0, i) + o.Substring(i + 3);
                i = i - 1;  // char before the ...
                while (o[i] != ' ')
                {
                    o = o.Substring(0, i) + o.Substring(i + 1);
                    i--;
                }
            }
            o = o.Replace("  ", " ");
            o = o.Replace("  ", " ");
            return o;
        }

        string StripComments(string o)
        {
            string outString = o;
            char[] parens = { '(', ')', '{', '}', '$', 'x', '?', '!', '+', '#' };

            Stack<int> openLoc = new Stack<int>();
            int curIndex = 0;

            while (curIndex >= 0)
            {
                curIndex = outString.IndexOfAny(parens, curIndex);
                if (curIndex >= 0)
                {
                    bool isEval = outString[curIndex] == '$';
                    bool isOpen = outString[curIndex] == '(' || outString[curIndex] == '{';
                    bool isClose = outString[curIndex] == ')' || outString[curIndex] == '}';
                    bool isDecoration = !isEval && !isOpen && !isClose;

                    if (isDecoration)
                    {
                        outString = outString.Substring(0, curIndex) + outString.Substring(curIndex + 1);
                    }
                    else if (isEval)
                    {
                        // remove this character and any following numeric characters
                        outString = outString.Substring(0, curIndex) + outString.Substring(curIndex + 1);
                        while ('0' <= outString[curIndex] && outString[curIndex] <= '9')
                            outString = outString.Substring(0, curIndex) + outString.Substring(curIndex + 1);
                        curIndex = curIndex - 1;
                    }
                    else if (isOpen)
                        openLoc.Push(curIndex++);
                    else
                    {
                        if (openLoc.Count == 0)
                            ;
                        int start = openLoc.Pop();
                        outString = outString.Substring(0, start) + outString.Substring(curIndex + 1);
                        curIndex = start - 1;
                    }
                }
            }
            if (openLoc.Count != 0)
                ;
            return outString;
        }
        string PullRosterTagValue(string str, string tag)
        {
            int i = str.IndexOf("[" + tag);
            int q = str.IndexOf("\"", i + 1);
            int eq = str.IndexOf("\"]", q + 1);

            string val = str.Substring(q + 1, eq - q - 1);

            return val;
        }

        private void PullEndPos_Click(object sender, EventArgs e)
        {
            Positions = new List<AnalysisPosition>();

            SqlConnection sConnn = new SqlConnection(connStr);
            sConnn.Open();
            SqlCommand sCmd = sConnn.CreateCommand();
            string SQL = "select GameID, finalFEN, Terminator from EndPositionAnalysis";
            if (inclWWin.Checked && inclBWin.Checked)
                SQL += " where Terminator = '1-0' or Terminator = '0-1'";
            else if (inclWWin.Checked || inclBWin.Checked)
                SQL += " where Terminator = "+(inclWWin.Checked ? "'1-0'" :"'0-1'");

            sCmd.CommandText = SQL;
            SqlDataReader dr = sCmd.ExecuteReader();
            if (dr.HasRows)
                while (dr.Read())
                {
                    AnalysisPosition ap = new AnalysisPosition();
                    ap.hostGameID = dr.GetGuid(0);
                    ap.thisPos = new Position(dr.GetString(1));
                    ap.terminator = (Game.Terminators)Game.terminators.IndexOf(dr.GetString(2));
                    Positions.Add(ap);
                }

            EndPosTag.Text = Positions.Count.ToString() + " positions stored";
            sConnn.Close();
        }

        private void AnalyzePositions_Click(object sender, EventArgs e)
        {
            // analysis structure for each game is 8x8 capturing 
            // black wins mirrored so scores are from winner's perspective (a8 act => a1 analysis, etc)
            //  occ counts weighted by nothing (1 for winner, -1 for loser)
            //  occ counts weighted by pieces
            //  % empty, % occ by winner, % occ by loser derived from raw occ counts

            int decisiveGames = 0;
            int[,] rawWinArray = new int[8, 8];
            int[,] rawLoseArray = new int[8, 8];
            int[,] wtdWinArray = new int[8, 8];
            int[,] wtdLoseArray = new int[8, 8];
            int[] pcValue = new int[] { 1, 5, 3, 3, 9, 15 };

            foreach (AnalysisPosition p in Positions)
            {
                if (p.terminator == Game.Terminators.Draw || p.terminator == Game.Terminators.InProgress)
                    continue;

                decisiveGames++;
                for (int r = 0; r < 8; r++)
                    for (int f = 0; f < 8; f++)
                    {
                        Square sq = new Square((Square.Rank)r, (Square.File)f);
                        if (p.thisPos.board.Keys.Contains(sq))
                        {
                            int effRank = r;
                            bool occByWinner = true;

                            Piece thisPc = p.thisPos.board[sq];
                            int pcWt = pcValue[(int)thisPc.piece];
                            if (p.terminator == Game.Terminators.WWin && thisPc.color == PlayerEnum.Black || p.terminator == Game.Terminators.BWin && thisPc.color == PlayerEnum.White)
                                occByWinner = false;
                            if (p.terminator == Game.Terminators.BWin)
                                effRank = 7 - r;

                            if (occByWinner)
                            {
                                rawWinArray[effRank, f]++;
                                wtdWinArray[effRank, f] += pcWt;
                            }
                            else
                            {
                                rawLoseArray[effRank, f]++;
                                wtdLoseArray[effRank, f] += pcWt;
                            }
                        }
                    }
                analysisTag.Text = decisiveGames.ToString() + " decisive games";
            }

            rawCounts.Text = wtdCounts.Text = "";
            for (int r = 7; r >= 0; r--)
            {
                for (int f = 0; f < 8; f++)
                {
                    rawCounts.Text += rawWinArray[r, f].ToString() + ",";
                    wtdCounts.Text += wtdWinArray[r, f].ToString() + ",";
                }
                rawCounts.Text += Environment.NewLine;
                wtdCounts.Text += Environment.NewLine;
            }
            rawCounts.Text += Environment.NewLine;
            wtdCounts.Text += Environment.NewLine;
            rawCounts.Text += Environment.NewLine;
            wtdCounts.Text += Environment.NewLine;
            for (int r = 7; r >= 0; r--)
            {
                for (int f = 0; f < 8; f++)
                {
                    rawCounts.Text += rawLoseArray[r, f].ToString() + ",";
                    wtdCounts.Text += wtdLoseArray[r, f].ToString() + ",";
                }
                rawCounts.Text += Environment.NewLine;
                wtdCounts.Text += Environment.NewLine;
            }
        }
    }
}
