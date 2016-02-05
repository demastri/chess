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
        string fileName { get; set; }

        public FileGameList(string fn, string user, GameList refGL) 
            : base(user)
        {
            fileName = fn;
            Load();
            Games = refGL.Games;
        }

        protected override void Load() 
        { 
            if (fileName!= "" && File.Exists(fileName))
            {
                StreamReader tr = new StreamReader(fileName);
                PGNTokenizer nextTokenSet = new PGNTokenizer(tr, defaultGrammarFileLocation);
                tr.Close();
                for (int i = 0; i < nextTokenSet.GameCount; i++)
                {
                    nextTokenSet.LoadGame(i);
                    Games.Add(new PGNGame(nextTokenSet));
                }
            }
        }

        public override void Save() 
        { 
        }
        public static void Save(string fn, string user, GameList refGL)
        {
        }
        public static GameList Load(string fn, string user)
        {
            GameList outGameList = new GameList();
            if (fn != "" && File.Exists(fn))
            {
                StreamReader tr = new StreamReader(fn);
                PGNTokenizer nextTokenSet = new PGNTokenizer(tr, defaultGrammarFileLocation);
                tr.Close();
                for (int i = 0; i < nextTokenSet.GameCount; i++)
                {
                    nextTokenSet.LoadGame(i);
                    outGameList.Games.Add(new PGNGame(nextTokenSet));
                }
            }
            return outGameList;
        }

    }
}
