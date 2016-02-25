#undef useDBContext

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace ChessPosition
{
    public class GameList
    {
        public List<Game> Games { get; set; }
        public string defaultGrammarFileLocation = "Parser/Grammars/PGNSchema.xml";

        public GameList(string filename, string grammarFile, string user)
        {
            InitFromFile( filename, grammarFile, user );
        }
        public GameList(string filename, string user)
        {
            InitFromFile(filename, defaultGrammarFileLocation, user);
        }
        private void InitFromFile(string filename, string grammarFile, string user)
        {
            Games = ReadPGNFile(filename, grammarFile);
            foreach (Game g in Games)
                if (g.PlayerWhite != user && g.PlayerBlack != user)
                    Games.Remove(g);
        }

        public GameList(string user)
        {
            Games = new List<Game>();
#if useDBContext
            List<Game> testGames = dbContext.Games.ToList<Game>();
#endif
        }
        public GameList()
        {
            Games = new List<Game>();
        }

        public bool Save(string user)   // db
        {
#if useDBContext
            foreach (Game g in GameList.dbContext.Games)
                GameList.dbContext.Games.Remove(g);
            foreach (Game g in Games)
            {
                g.TagColl = Game.Tag.TagsFromDictionary(g.Tags);
                GameList.dbContext.Games.Add(g);
            }
            GameList.dbContext.SaveChanges();
#endif
            return false;
        }
        public bool Save(string filename, string user)  // file
        {
            StreamWriter tr = new StreamWriter(filename);
            foreach (Game g in Games)
                g.SavePGN(tr);
            tr.Flush();
            tr.Close();
            return true;
        }

        public Game this[int i]
        {
            get { return Games[i]; }
        }

        private List<Game> ReadPGNFile(string PGNFileLoc)
        {
            string grammarFile = "Parser/Grammars/PGNSchema.xml";
            return ReadPGNFile(PGNFileLoc, grammarFile);

        }
        private List<Game> ReadPGNFile(string PGNFileLoc, string GrammarFile)
        {
            List<Game> GameRef = new List<Game>();
            if (PGNFileLoc != "" && File.Exists(PGNFileLoc))
            {
                StreamReader tr = new StreamReader(PGNFileLoc);
                PGNTokenizer nextTokenSet = new PGNTokenizer(tr, GrammarFile);
                tr.Close();
                for (int i = 0; i < nextTokenSet.GameCount; i++)
                {
                    nextTokenSet.LoadGame(i);
                    GameRef.Add(new Game(nextTokenSet));
                }
            }
            return GameRef;
        }
    }
}
