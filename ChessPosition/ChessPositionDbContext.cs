using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data;

namespace ChessPosition
{
    public class ChessPositionDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public ChessPositionDbContext()
            : base("name=ChessPositionDatabase")
        {
        }

    }
}
