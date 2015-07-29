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
        DateTime executionCompleteTime;
        public Analysis thisAnalysis;
        public string Status;
        public int EngineInstance;

        public AnalysisRequest(string paramStr )
        {
            string[] tokens = paramStr.Split('|');

            int curIndex = (tokens[0].Trim() == "" ? 1 : 0);
            int id = Convert.ToInt32(tokens[curIndex++]);
            string fenString = tokens[curIndex++];
            EngineParameters ep = new EngineParameters(tokens[curIndex++], Convert.ToInt32(tokens[curIndex++]), Convert.ToInt32(tokens[curIndex++]));
            Init(ep, fenString, id);

            // need to serialize/deserialize the:
            //  status
            //  submit time
            //  completionTime
            //  executionTIme
            //  analysis
            Status = tokens[curIndex++];
            submitTime = DateTime.Parse(tokens[curIndex++]);
            executionStartTime = DateTime.Parse(tokens[curIndex++]);
            executionCompleteTime = DateTime.Parse(tokens[curIndex++]);

            // find the offset for the analysis string...
            int aIndex = JPD.Utilities.Utils.GetNthIndex(paramStr, '|', 10);


            thisAnalysis = new Analysis(paramStr.Substring(aIndex), true);
        }
        public string ToQueueString()
        {
            string outString = "";
            outString = String.Format("|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|", 
                thisID, FEN, param.ToString(), Status, submitTime, executionStartTime, executionCompleteTime, thisAnalysis.ToQueueString()
                );
            outString = outString.Replace("||", "|");
            return outString;
        }
        public AnalysisRequest(EngineParameters ep, Position p)
        {
            Init(ep, p.ToFEN(1, 1), p.Hash.GetHashCode());
        }

        public void Init(EngineParameters ep, string fenString, int id)
        {
            EngineInstance = -1;
            thisID = id;
            param = ep;
            FEN = fenString;
            submitTime = DateTime.MinValue;
            executionStartTime = DateTime.MinValue;
            executionCompleteTime = DateTime.MinValue;
            thisAnalysis = new Analysis(PlayerEnum.White);
            Status = "Created";
        }
        public void MarkQueued()
        {
            Status = "Waiting";
            submitTime = DateTime.Now;
        }
        public void MarkRunning()
        {
            Status = "Running";
            executionStartTime = DateTime.Now;
        }
        public void MarkCompleted()
        {
            Status = "Completed";
            executionCompleteTime = DateTime.Now;
        }
    }
}
