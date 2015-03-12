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
        protected HostWrapper myEngineProcess;
        protected string engineLoc;

        public delegate void AnalysisUpdateHandler();
        public event AnalysisUpdateHandler AnalysisUpdate;

        public string lastAnalysisReply;
        public string lastResponse;
        public Analysis curAnalysis;
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
        public static Engine InitEngine(string engineName)
        {
            switch (engineName.ToLower())
            {
                case "":
                case "none":
                    return new Engine();
                case "crafty":
                    return new Engines.Crafty();
                case "stockfish":
                    return new Engines.Stockfish();
            }
            return null;
        }

        public virtual void Stop()
        {
        }
        public virtual void Quit()
        {
            running = false;
        }
        public virtual void StartAnalysis(Position pos)
        {
            // no idea how to instantiate this yet
            SetPostion(pos.ToFEN(0, 10));
        }
        public virtual void StartAnalysis(Game g)
        {
            // no idea how to instantiate this yet
            SetPostion( g.CurrentPosition.ToFEN(0, 10));
        }
        public virtual void SetPostion(string fenString)
        {
            PlayerEnum onMove = (fenString[fenString.IndexOf(' ') + 1] == 'w' ? PlayerEnum.White : PlayerEnum.Black);
            curAnalysis = new Analysis(onMove);
            running = true;
        }
        public int ProcessControl(HostWrapper thisHost)
        {
            System.Threading.Thread.Sleep(50);
            while (thisHost.incoming.Count > 0)
            {
                string s = thisHost.incoming[0];
                // ### this will be engine specific to turn string to analysis - for now display whatever you get
                if (s != null && Analysis.isUCIstring(lastResponse = s))
                {
                    lastAnalysisReply = s;
                    curAnalysis.UpdateWithUCIString(lastAnalysisReply);
                }
                thisHost.incoming.RemoveAt(0);
                RaiseAnalysisUpdate();
            }
            return running ? HostWrapper.IsRunning : HostWrapper.IsEnding;
        }
        public virtual void CheckProgress()
        {
            if( myEngineProcess != null )
                myEngineProcess.CheckProgress();
        }
        protected void RaiseAnalysisUpdate()
        {
            AnalysisUpdate();
        }
    }
}
