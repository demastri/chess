using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    // e.g. "info depth 2 score cp 214 time 1242 nodes 2124 nps 34928 pv e2e4 e7e5 g1f3"

    public class Analysis
    {
        public static bool isUCIstring(string s)
        {
            string[] parseTokens = s.Split(' ');
            return (parseTokens[0] == "info" || parseTokens[0] == "bestmove");
        }

        public bool isComplete;
        public int searchDepthPly;
        public int selectiveSearchDepthPly;
        public long searchTimeMS;
        public long searchNodes;
        public int searchRateNPS;
        public string moveToPonder;
        public List<string> bestLine;
        public decimal Score;
        PlayerEnum posOnMove;

        public Analysis(PlayerEnum onMove)
        {
            posOnMove = onMove;
            Init();
        }
        public Analysis(string inputString, bool fromQueue)
        {
            Init();
            if (fromQueue)
                UpdateWithQueueString(inputString);
            else
                UpdateWithUCIString(inputString);
        }
        public void UpdateWithQueueString(string queueString)
        {
            string[] tokens = queueString.Split('|');

            int curIndex = (tokens[0].Trim() == "" ? 1 : 0);

            isComplete = (tokens[curIndex++] == "Y");
            searchDepthPly = Convert.ToInt32(tokens[curIndex++]);
            selectiveSearchDepthPly = Convert.ToInt32(tokens[curIndex++]);
            searchTimeMS = Convert.ToInt32(tokens[curIndex++]);
            searchNodes = Convert.ToInt32(tokens[curIndex++]);
            searchRateNPS = Convert.ToInt32(tokens[curIndex++]);
            moveToPonder = tokens[curIndex++].Trim();
            Score = Convert.ToDecimal(tokens[curIndex++]);
            posOnMove = (tokens[curIndex++] == "w" ? PlayerEnum.White : PlayerEnum.Black);

            bestLine = new List<string>();
            while (curIndex < tokens.Count()  )
                bestLine.Add(tokens[curIndex++]);            
        }
        public void UpdateWithUCIString(string uciString)
        {
            int parseTokenIndex;
            string[] parseTokens;

            int placeVal;
            parseTokens = uciString.Split(' ');
            for (parseTokenIndex = 0; parseTokenIndex < parseTokens.Count(); parseTokenIndex++)
            {
                switch (parseTokens[parseTokenIndex])
                {
                    case "info":    // ok - have a line to process
                        break;
                    case "depth":
                        searchDepthPly = Convert.ToInt32(parseTokens[++parseTokenIndex]);
                        break;
                    case "seldepth":
                        selectiveSearchDepthPly = Convert.ToInt32(parseTokens[++parseTokenIndex]);
                        break;
                    case "time":
                        searchTimeMS = Convert.ToInt64(parseTokens[++parseTokenIndex]);
                        break;
                    case "nodes":
                        searchNodes = Convert.ToInt64(parseTokens[++parseTokenIndex]);
                        break;
                    case "pv":
                        bestLine.Clear();
                        while (++parseTokenIndex < parseTokens.Count())
                            bestLine.Add(parseTokens[parseTokenIndex]);
                        break;
                    case "multipv":
                            parseTokenIndex++; // skip the line index (not using multiPV mode currently)
                        break;
                    case "score":
                        string scoreType = parseTokens[++parseTokenIndex];
                        int baseScore = Convert.ToInt32(parseTokens[++parseTokenIndex]) * (posOnMove == PlayerEnum.White ? 1 : -1);
                        if( scoreType == "cp" )
                            Score = baseScore / 100.0m;
                        if( scoreType == "mate" )
                            Score = 1000 * baseScore;
                        if (scoreType == "lowerbound" || scoreType == "upperbound")
                            ;   // nothing to do here (yet??)
                        break;
                    case "currmove":
                        ++parseTokenIndex; // unused currently, skip the given move
                        break;
                    case "currmonvenumber":
                        placeVal = Convert.ToInt32(parseTokens[++parseTokenIndex]); // unused currently
                        break;
                    case "hashfull":
                        placeVal = Convert.ToInt32(parseTokens[++parseTokenIndex]); // unused currently
                        break;
                    case "nps":
                        searchRateNPS = Convert.ToInt32(parseTokens[++parseTokenIndex]);
                        break;
                    case "tbhits":
                        placeVal = Convert.ToInt32(parseTokens[++parseTokenIndex]); // unused currently
                        break;
                    case "sbhits":
                        placeVal = Convert.ToInt32(parseTokens[++parseTokenIndex]); // unused currently
                        break;
                    case "cpuload":
                        placeVal = Convert.ToInt32(parseTokens[++parseTokenIndex]); // unused currently
                        break;
                    case "string":
                        // ###
                        break;
                    case "refutation":
                        // ###
                        break;
                    case "currline":
                        // ###
                        break;
                    case "bestmove":
                        // should be able to assert that 
                        if (bestLine.Count>0 && parseTokens[++parseTokenIndex] != bestLine[0])
                        {
                            if (bestLine.Count == 0)
                                bestLine.Add(parseTokens[parseTokenIndex]);
                            else
                                bestLine[0] = parseTokens[parseTokenIndex];
                            System.Console.WriteLine("really...the best line isn't the one we've been considering..." + bestLine[0]);
                        }
                        isComplete = true;
                        break;
                }
            }

        }
        public override string ToString()
        {
            if( Math.Abs(Score) >= 1000 )
                return String.Format("Score: Mate in {0} Best:{3}" + Environment.NewLine + "Nodes: {1,9:D}  Rate: {2,8:D}\n",
                    Math.Abs(Score/1000), searchNodes, searchRateNPS, (bestLine.Count > 0 ? bestLine[0] : "NA"));

            return String.Format ("Score: {0,5:F2} Depth:{4} Best:{3}"+Environment.NewLine+"Nodes: {1,9:D}  Rate: {2,8:D}\n", 
                Score, searchNodes, searchRateNPS, (bestLine.Count > 0 ? bestLine[0] : "NA" ), searchDepthPly);

        }
        /// <summary>
        /// public int AnalysisID;
        /// public bool isComplete;
        /// public int searchDepthPly;
        /// public int selectiveSearchDepthPly;
        /// public long searchTimeMS;
        /// public long searchNodes;
        /// public int searchRateNPS;
        /// public string moveToPonder;
        /// public List<string> bestLine;
        /// public decimal Score;
        /// 
        /// PlayerEnum posOnMove;
        /// private int parseTokenIndex;
        /// private string[] parseTokens;

        public string ToQueueString()
        {
            string outString = "";
            outString = String.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|", 
                (isComplete ? "Y" : "N"), searchDepthPly, selectiveSearchDepthPly, 
                searchTimeMS, searchNodes, searchRateNPS, (moveToPonder == "" ? " " : moveToPonder), Score, (posOnMove == PlayerEnum.White ? "w" : "b")
                );
            foreach (string move in bestLine)
                outString += move + "|";
            return outString;
        }
        private void Init()
        {
            searchDepthPly = selectiveSearchDepthPly = searchRateNPS = -1;
            searchTimeMS = searchNodes = -1L;
            bestLine = new List<string>();
            Score = 0;
            isComplete = false;
            moveToPonder = "";
        }
    }
}
