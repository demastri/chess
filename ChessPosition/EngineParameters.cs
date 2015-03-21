using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition
{
    public class EngineParameters
    {
        public string engineName;
        public int searchDepth;
        public int searchTimeMS;
        public EngineParameters(string engine, int depth, int ms)
        {
            engineName = engine;
            searchDepth = depth;
            searchTimeMS = ms;
        }
        public string ToString()
        {
            return String.Format("|{0}|{1}|{2}|", engineName, searchDepth, searchTimeMS);
        }
    }
}
