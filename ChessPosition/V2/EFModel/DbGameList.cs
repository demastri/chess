using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using ChessPosition.V2.EFModel;

namespace ChessPosition.V2.Db
{
    public class DbGameList : GameList
    {
        public static CorrMgrEntities dbContext = new CorrMgrEntities();

        public DbGameList(string user)
            : base("", user)
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
            // delete _the user's_ games from the db via the context
            dbContext.Games.RemoveRange(dbContext.Games.Where(g => g.User.DisplayName == User));
            // add them back from the games list
            foreach (Game g in Games)
                dbContext.Games.Add(DbGame.GenerateEFGame(g, User));
            // save the user's games back to the db via the context
            dbContext.SaveChanges();
        }
        public static void Save(GameList games, string user)
        {
            // messy way -> (the app can literally do anything, how else can they stay in sync??)

            // check if the user exists
            bool exists = (dbContext.Users.Where(u => u.DisplayName == user).Count() > 0);

            if (!exists)
            {
                EFModel.User nextUser = new User();
                nextUser.UserID = Guid.NewGuid();
                nextUser.DisplayName = user;

                dbContext.Users.Add(nextUser);
                dbContext.SaveChanges();
            }

            // delete _the user's_ games from the db via the context
            dbContext.Comments.RemoveRange(dbContext.Comments.Where(c => c.Ply.Game.User.DisplayName == user));
            dbContext.Plies.RemoveRange(dbContext.Plies.Where(p => p.Game.User.DisplayName == user));
            dbContext.Tags.RemoveRange(dbContext.Tags.Where(t => t.Game.User.DisplayName == user));
            dbContext.Games.RemoveRange(dbContext.Games.Where(g => g.User.DisplayName == user));
            dbContext.SaveChanges();

            // add them back from the games list
            foreach (Game g in games.Games)
                dbContext.Games.Add(DbGame.GenerateEFGame(g, user));
            // save the user's games back to the db via the context
            dbContext.SaveChanges();

            // testing steps
            // 0 - read/write from pgn, as a test
            // complete - 24-Feb-16
            // 1 - read from pgn, write data back to the database as well
            // complete - 26-Feb-16
            // 2 - read the data from the database, write to both
            // complete - 26-Feb-16
            // 3 - read/write from the db
            // complete - 26-Feb-16
            // 4 - read from db, merge with incoming pgn data, write all back to db

        }
    }
}
