using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.PGN
{
    public class DbGameList : GameList
    {
        public ChessPositionDbContext dbContext = new ChessPositionDbContext();

        public DbGameList(string user)
            : base(user)
        {
        }

        protected override void Load()
        {
        }

        public override void Save()
        {
        }
    }
}
