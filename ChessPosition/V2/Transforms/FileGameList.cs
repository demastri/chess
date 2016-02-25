using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace ChessPosition.V2.PGN
{
    public class FileGameList : GameList
    {
        private static string defaultGrammarFileLocation = "Parser/Grammars/PGNSchema.xml";

        public FileGameList(string fn, string user)
            : base(fn, user)
        {
            Load();
        }

        public override void Load() 
        {
            if (connDetail != "" && File.Exists(connDetail))
            {
                StreamReader tr = new StreamReader(connDetail);
                PGNTokenizer nextTokenSet = new PGNTokenizer(tr, defaultGrammarFileLocation);
                tr.Close();
                for (int i = 0; i < nextTokenSet.GameCount; i++)
                {
                    nextTokenSet.LoadGame(i);

                    string tokenStr = "";
                    foreach (PGNToken pgnt in nextTokenSet.tokens)
                        tokenStr += pgnt.ToString();

                    Games.Add(new PGNGame(nextTokenSet));
                }
            }
        }

        public override void Save()
        {
            StreamWriter tr = new StreamWriter(connDetail);
            foreach (Game g in Games)
            {
                PGNGame outGame = new PGNGame(g);
                outGame.SavePGN(tr);
            }
            tr.Flush();
            tr.Close();
        }

    }
}
