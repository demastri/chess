using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProcessWrappers;

namespace ChessPosition.Engines
{
    public class Stockfish : Engine
    {

        public Stockfish()
        {
            //engineLoc = "C:\\Projects\\JPD\\BBRepos\\Chess\\engines\\stockfish\\stockfish_5_32bit.exe";
            engineLoc = "D:\\Projects\\Workspaces\\BBRepos\\Chess\\engines\\stockfish\\stockfish_5_32bit.exe";
            myEngineProcess = new HostWrapper(engineLoc, true, ProcessControl, false);

            myEngineProcess.Start();
        }
        public override void SetPostion(AnalysisRequest ar)
        {
            base.SetPostion(ar);

            myEngineProcess.WriteToClient("stop");
            myEngineProcess.WriteToClient("position fen " + ar.FEN);
            if (ar.param.searchDepth > 0)
                myEngineProcess.WriteToClient("go depth "+ar.param.searchDepth.ToString());
            else
                myEngineProcess.WriteToClient("go movetime " + ar.param.searchTimeMS.ToString());
        }
        public override void SetPostion(EngineParameters ep, string fenString)
        {
            base.SetPostion(ep, fenString);

            myEngineProcess.WriteToClient("stop");
            myEngineProcess.WriteToClient("position fen " + fenString);
            if (ep.searchDepth > 0)
                myEngineProcess.WriteToClient("go depth " + ep.searchDepth.ToString());
            else
                myEngineProcess.WriteToClient("go movetime " + ep.searchTimeMS.ToString());
        }
        public override void Status()
        {
            myEngineProcess.WriteToClient("uci");
        }
        public override void Stop()
        {
            base.Stop();
            myEngineProcess.WriteToClient("stop");
        }
        public override void Quit()
        {
            base.Quit();
            myEngineProcess.WriteToClient("quit");
        }
    }
}
