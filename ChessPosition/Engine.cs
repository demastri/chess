using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProcessWrappers;

namespace ChessPosition
{
    public class Engine
    {
#if false
        protected HostWrapper myEngineProcess;
        protected string engineLoc;

        public delegate void AnalysisUpdateHandler(int analysisID);
        public event AnalysisUpdateHandler AnalysisUpdateEvent;
        public event AnalysisUpdateHandler AnalysisCompleteEvent;

        public string lastEngineReply;
        public string lastAnalysisReply;
        public string lastResponse;
        public AnalysisRequest curAnalysisRequest;
        public bool running;

        static Dictionary<Piece, int> ValueMap = new Dictionary<Piece, int>()
        {
            { new Piece( PlayerEnum.White, Piece.PieceType.Pawn), 1 },
            { new Piece( PlayerEnum.White, Piece.PieceType.Rook), 5 },
            { new Piece( PlayerEnum.White, Piece.PieceType.Knight), 3 },
            { new Piece( PlayerEnum.White, Piece.PieceType.Bishop), 3 },
            { new Piece( PlayerEnum.White, Piece.PieceType.Queen), 9 },
            { new Piece( PlayerEnum.White, Piece.PieceType.King), 99 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Pawn), -1 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Rook), -5 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Knight), -3 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Bishop), -3 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.Queen), -9 },
            { new Piece( PlayerEnum.Black, Piece.PieceType.King), -99 }
        };
#endif
        public static Engine InitEngine(string engineName)
        {
            switch (engineName.ToLower())
            {
                case "":
                case "none":
                    return new Engine();
#if false
                case "crafty":
                    return new Engines.Crafty();
                case "stockfish":
                    return new Engines.Stockfish();
#endif
            }
            return null;
        }

        public virtual void Stop()
        {
        }
        public virtual void Quit()
        {
#if false
            myEngineProcess.Cleanup();
            running = false;
#endif
        }
#if false
        public virtual void Status()
        {
        }
        public virtual bool Idle()
        {
            return curAnalysisRequest == null || curAnalysisRequest.thisAnalysis == null || curAnalysisRequest.thisAnalysis.isComplete;
        }
        public virtual bool AnalysisComplete()
        {
            return curAnalysisRequest != null && curAnalysisRequest.thisAnalysis != null && curAnalysisRequest.thisAnalysis.isComplete;

        }
        public virtual void StartAnalysis(EngineParameters ep, Position pos)
        {
            // no idea how to instantiate this yet
            SetPostion(ep, pos.ToFEN(0, 10));
        }
        public virtual void StartAnalysis(EngineParameters ep, Game g)
        {
            // no idea how to instantiate this yet
            SetPostion(ep, g.ToFEN());
        }
        public virtual void SetPostion(AnalysisRequest ar)
        {
            PlayerEnum onMove = (ar.FEN[ar.FEN.IndexOf(' ') + 1] == 'w' ? PlayerEnum.White : PlayerEnum.Black);
            curAnalysisRequest = ar;
            curAnalysisRequest.thisAnalysis = new Analysis(onMove);
            running = true;
            curAnalysisRequest.MarkRunning();
        }
        public virtual void SetPostion(EngineParameters ep, string fenString)
        {
            PlayerEnum onMove = (fenString[fenString.IndexOf(' ') + 1] == 'w' ? PlayerEnum.White : PlayerEnum.Black);
            Position p = new Position(fenString);
            curAnalysisRequest = new AnalysisRequest(ep, p);
            curAnalysisRequest.thisID = p.Hash.GetHashCode();
            curAnalysisRequest.thisAnalysis = new Analysis(onMove);
            running = true;
            curAnalysisRequest.MarkRunning();
        }
        public int ProcessControl(HostWrapper thisHost)
        {
            while (thisHost.incoming.Count > 0)
            {
                string s = thisHost.incoming[0];
                myEngineProcess.WriteLog("Incoming -> " + s);

                lastEngineReply = s;
                // ### this will be engine specific to turn string to analysis - for now display whatever you get
                if (s != null && Analysis.isUCIstring(lastResponse = s))
                {
                    lastAnalysisReply = s;
                    curAnalysisRequest.thisAnalysis.UpdateWithUCIString(lastAnalysisReply);
                    RaiseAnalysisUpdate(curAnalysisRequest.thisID);
                }
                thisHost.incoming.RemoveAt(0);
                if (AnalysisComplete())
                {
                    curAnalysisRequest.MarkCompleted();
                    RaiseAnalysisComplete(curAnalysisRequest.thisID);
                }
            }
            return running ? HostWrapper.IsRunning : HostWrapper.IsEnding;
        }
#endif
        public virtual void CheckProgress()
        {
#if false
            if (myEngineProcess != null)
                myEngineProcess.CheckProgress();
#endif
        }
#if false
        protected void RaiseAnalysisUpdate(int analysisID)
        {
            if (AnalysisUpdateEvent != null)
                AnalysisUpdateEvent(analysisID);
        }
        protected void RaiseAnalysisComplete(int analysisID)
        {
            if (AnalysisCompleteEvent != null)
                AnalysisCompleteEvent(analysisID);
        }
#endif
    }
}
