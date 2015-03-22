using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessPosition;

namespace AnalysisFarmConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (true)
            {
                AnalysisFarm thisFarm = null;
                if (thisFarm == null)
                {
                    thisFarm = new AnalysisFarm(6, null);
                    thisFarm.Start();
                }
            }
            Console.WriteLine("Farm started - key to exit");
            while (!Console.KeyAvailable)
                System.Threading.Thread.Sleep(50);
        }
    }
}
