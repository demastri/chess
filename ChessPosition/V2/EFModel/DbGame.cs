using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.PGN
{
    public class DbGame : Game
    {
        // structures needed for db translation
        public Guid ID;
        public DbGame(EFModel.Game dbg)
            : base()
        {
            LoadGame(dbg);
        }
        private void LoadGame(EFModel.Game dbg)
        {
            // init the game...
            // load tags
            ID = dbg.GameID;
            foreach (EFModel.Tag t in dbg.Tags)
                Tags.Add(t.TagName, t.TagValue);
            // load plies
            foreach (EFModel.Ply p in dbg.Plies)
            {
                // for each ply
                Ply outPly = new Ply(intToSquare(p.SourceSquare), intToSquare(p.DestSquare), charToPieceType(p.PromoPiece));
                // load comments
                foreach (EFModel.Comment c in p.Comments)
                    outPly.comments.Add(new Comment(c.commentEndsLine, c.CommentValue));
                Plies.Add(outPly);
            }
            // ### load variations (includes recursive ply / comment / variation loads)
        }
        private Square intToSquare(int refInt)
        {
            if (refInt < 0 || refInt / 10 > 7 || refInt % 10 > 7)
                return Square.None();
            return new Square((Square.Rank)(refInt / 10), (Square.File)(refInt % 10));

        }
        private Piece charToPieceType(int refInt)
        {
            return Piece.FromFENChar((char)refInt);
        }
    }
}
