using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace trimcomments
{
    class Program
    {
        internal class game
        {
            public string STRtext;
            public string GameText;
            public string InitialFEN;
            public string Result;
            public List<move> Moves;
        }
        internal class move
        {
            public string baseRep;
            public int plySeq;
            public int src;
            public int dest;
            public char movePc;
            public char promoPc;
            public bool isWhite;
            public string postFEN;
        }
        static void Main(string[] args)
        {
            string fn = "C:\\Users\\jdemastri\\Downloads\\SimplePGNPutTest.pgn";
            List<string> HeaderText = new List<string>();
            List<string> GameText = new List<string>();
            StreamReader sr = new StreamReader(fn);

            bool done = false;

            // pull raw STR and game text from file
            while ( !done )
            {
                List<string> s = GetGameText(sr);
                if (!(done = (s == null || s.Count < 2)))
                {
                    HeaderText.Add(s[0]);
                    GameText.Add(s[1]);
                }
            }

            // clean comments++ from gameText
            for (int i = 0; i < GameText.Count; i++)
            {
                string thisGame = GameText[i];
                thisGame = StripComments(thisGame);
                thisGame = StripMovePlaceholders(thisGame);

                Console.WriteLine(HeaderText[i] + "\n" + GameText[i]);
                Console.WriteLine("---------------");
                Console.WriteLine(HeaderText[i] + "\n" + thisGame);
                Console.WriteLine();
                Console.WriteLine();
                GameText[i] = thisGame;
            }

            // generate positions from games
            string startFEN = "xxx";                    // ###
            List<game> finalGames = new List<game>();
            for (int i = 0; i < GameText.Count; i++)
            {
                string lastFEN = startFEN;
                string s = GameText[i];
                List<move> moves = new List<move>();
                string[] plies = s.Trim().Split();
                int plycount = 0;
                bool WhiteOnMove = true;
                foreach(string p in plies)
                {
                    char lastChar = p[p.Length - 1];
                    if (lastChar == '.') // move marker...
                        continue;
                    move thisPos = new move();
                    thisPos.baseRep = p;
                    thisPos.isWhite = WhiteOnMove;
                    WhiteOnMove = !WhiteOnMove;
                    thisPos.plySeq = ++plycount;

                    // resolve this move in context ###

                    char pc = p[0];
                    thisPos.movePc = ('A' <= pc && pc <= 'Z' ? pc : 'P');
                    pc = p[0];
                    thisPos.promoPc = ('A' <= lastChar && lastChar <= 'Z' ? lastChar : '-');
                    thisPos.src = thisPos.dest = 0;     // ###
                    thisPos.postFEN = lastFEN;          // ###

                    lastFEN = thisPos.postFEN;

                    moves.Add(thisPos);
                }
                game g = new game();
                g.STRtext = HeaderText[i];
                g.GameText = GameText[i];
                g.InitialFEN = startFEN;
                g.Result = moves[moves.Count - 1].baseRep;
                moves.RemoveAt(moves.Count - 1);
                g.Moves = moves;

                finalGames.Add(g);
            }
        }

        static string StripMovePlaceholders(string o)
        {
            int i = -1;
            while( (i = o.IndexOf("...")) >= 0 )
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

        static string StripComments(string o)
        {
            string outString = o;
            char[] parens = { '(', ')', '{', '}', '$','x','?','!','+','#', '=' };

            Stack<int> openLoc = new Stack<int>();
            int curIndex = 0;

            while (curIndex >= 0)
            {
                curIndex = outString.IndexOfAny( parens, curIndex);
                if (curIndex >= 0)
                {
                    bool isEval = outString[curIndex] == '$';
                    bool isOpen = outString[curIndex] == '(' || outString[curIndex] == '{';
                    bool isClose = outString[curIndex] == ')' || outString[curIndex] == '}';
                    bool isDecoration = !isEval && !isOpen && !isClose;

                    if( isDecoration )
                    {
                        outString = outString.Substring(0, curIndex) + outString.Substring(curIndex + 1);
                    }
                    else if ( isEval )
                    {
                        // remove this character and any following numeric characters
                        outString = outString.Substring(0, curIndex) + outString.Substring(curIndex+1);
                        while('0' <= outString[curIndex] && outString[curIndex] <= '9')
                            outString = outString.Substring(0, curIndex) + outString.Substring(curIndex+1);
                        curIndex = curIndex-1;
                    }
                    else if (isOpen)
                        openLoc.Push(curIndex++);
                    else
                    {
                        if (openLoc.Count == 0)
                            ;
                        int start = openLoc.Pop();
                        outString = outString.Substring(0, start - 1) + outString.Substring(curIndex+1);
                        curIndex = start-1;
                    }
                }
            }
            if (openLoc.Count != 0)
                ;
            return outString;
        }


        static string pushedLine = "";
        static List<string> GetGameText(StreamReader sr)
        {
            List<string> outList = new List<string>();

            bool inGame = false;
            bool inHeader = false;
            bool done = false;
            string gameText = "";
            while (!done)
            {
                string l;
                if (pushedLine != "")
                {
                    l = pushedLine;
                    pushedLine = "";
                }
                else
                {
                    if (sr.EndOfStream)
                    {
                        outList.Add(gameText);
                        return outList;
                    }
                    l = sr.ReadLine();
                }
                if (!inGame)
                {
                    if (l.Length >= 2 && l[0] == '[' && l[1] != '%')
                    {
                        inGame = inHeader = true;
                        gameText += l + " ";
                    }
                }
                else
                {
                    if (inHeader)
                        if (l.Length < 1 || l[0] != '[')    // still
                        {
                            inHeader = false;
                            outList.Add(gameText);
                            gameText = "";
                        }
                    if (!inHeader && l.Length >= 2 && l[0] == '[' && l[1] != '%')    // start of next game...
                        inGame = inHeader = false;
                    if (inGame)
                        gameText += l + " ";
                    else
                    {
                        pushedLine = l;
                        outList.Add(gameText);
                        return outList;
                    }
                }
            }
            return null;
        }
    }
}
