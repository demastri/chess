using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using ChessPosition.V2.EFModel;

namespace ChessPosition.V2.PGN
{
    public class DbGameList : GameList
    {
        CorrMgrEntities dbContext = new CorrMgrEntities();

        public DbGameList(string conn, string user)
            : base(conn, user)
        {
            Load();
        }

        public override void Load()
        {
            Games = new List<Game>();
            // load games for this user
            foreach (EFModel.Game dbg in dbContext.Games.ToList().Where(g => g.User.DisplayName == User))
            {
                DbGame nextGame = new DbGame( dbg );
                Games.Add(nextGame);
            }
        }

        public override void Save()
        {
            // messy way -> (the app can literally do anything, how else can they stay in sync??)
            // delete _the user's_ games from the db via the context
            // add them back from the games list
            // save the user's games back to the db via the context
            // ###

            // testing steps
            // 0 - read/write from pgn, as a test
            // 1 - read from pgn, write data back to the database as well
            // 2 - read the data from the database, write to both
            // 3 - read/write from the db
            // 4 - read from db, merge with incoming pgn data, write all back to db
            
        }
    }
}
