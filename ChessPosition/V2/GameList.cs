using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2
{
    public class GameList
    {
        public List<Game> Games { get; set; }
        public string User { get; set; }

        protected virtual void Load() {}
        public virtual void Save() {}

        public GameList()
        {
            Games = new List<Game>();
            User = "";
        }
        public GameList(string user)
        {
            Games = new List<Game>();
            User = user;
        }
        
        public Game this[int i]
        {
            get { return Games[i]; }
        }

        public GameList FindGame(Dictionary<string, string> tags)
        {
            GameList outList = new GameList();
            foreach (Game g in Games)
            {
                bool ok = true;
                foreach (string tag in tags.Keys)
                    if (!g.Tags.ContainsKey(tag) || g.Tags[tag] != tags[tag])
                    {
                        ok = false;
                        break;
                    }
                if (ok)
                    outList.Games.Add(g);
            }
            return outList;
        }

    }
}
