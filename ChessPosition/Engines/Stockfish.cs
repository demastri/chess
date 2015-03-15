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
            engineLoc = "C:\\Projects\\JPD\\BBRepos\\Chess\\engines\\stockfish\\stockfish_5_32bit.exe";
            engineLoc = "D:\\Projects\\Workspaces\\BBRepos\\Chess\\engines\\stockfish\\stockfish_5_32bit.exe";
            myEngineProcess = new HostWrapper(engineLoc, true, ProcessControl);

            myEngineProcess.Start();
        }
        public override void SetPostion(string fenString)
        {
            base.SetPostion(fenString);

            myEngineProcess.WriteToClient("stop");
            myEngineProcess.WriteToClient("position fen " + fenString);
            myEngineProcess.WriteToClient("go depth 20");
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
