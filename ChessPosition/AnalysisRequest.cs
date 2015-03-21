using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class AnalysisRequest
    {
        public int thisID;
        public string FEN;
        public EngineParameters param;
        DateTime submitTime;
        DateTime executionStartTime;
        DateTime completionTime;
        Analysis thisAnalysis;
        public string Status;
        public int EngineInstance;

        public AnalysisRequest(string paramStr )
        {
            string[] tokens = paramStr.Split('|');

            int curIndex = (tokens[0].Trim() == "" ? 1 : 0);
            int id = Convert.ToInt32(tokens[curIndex++]);
            string fenString = tokens[curIndex++];
            EngineParameters ep = new EngineParameters(tokens[curIndex++], Convert.ToInt32(tokens[curIndex++]), Convert.ToInt32(tokens[curIndex++]));
            /// here's the mapping
            /// the id takes 10 bytes
            /// the phash takes 50 bytes
            /// give the rest to EngineParameters to resolve, and we're ok...
            Init(ep, fenString, id);
        }
        public AnalysisRequest(EngineParameters ep, string fenString, int id)
        {
            Init(ep, fenString, id);
        }

        public void Init(EngineParameters ep, string fenString, int id)
        {
            EngineInstance = -1;
            thisID = id;
            param = ep;
            FEN = fenString;
            submitTime = DateTime.Now;
            executionStartTime = DateTime.MinValue;
            completionTime = DateTime.MinValue;
            thisAnalysis = null;
            Status = "Waiting";
        }


        public string ToQueueString()
        {
            string outString = "";
            outString = String.Format( "|{0}|{1}|{2}|", thisID, FEN, param.ToString() );
            outString = outString.Replace("||", "|");
            return outString;
        }
    }
}
