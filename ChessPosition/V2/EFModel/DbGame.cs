using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPosition.V2.Db
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
                Tags[t.TagName] = t.TagValue;
            // load plies
            foreach (EFModel.Ply p in dbg.Plies.OrderBy( p=>p.PlyNumber ) )
            {
                // for each ply
                Ply outPly = new Ply(Square.FromInt(p.SourceSquare), Square.FromInt(p.DestSquare), charToPieceType(p.PromoPiece));
                outPly.Number = p.PlyNumber;
                // load comments
                foreach (EFModel.Comment c in p.Comments)
                    outPly.comments.Add(new Comment(c.commentEndsLine, c.CommentValue));
                Plies.Add(outPly);
            }
            // ### load variations (includes recursive ply / comment / variation loads)

            if (Game.terminators.Contains(dbg.Terminator))
                Terminator = (Game.Terminators)Game.terminators.IndexOf(dbg.Terminator);
            else
                Terminator = Game.Terminators.InProgress;
        }

        public static EFModel.Game GenerateEFGame(V2.Game refGame, string user)  // build the db entity from the reference game
        {
            EFModel.Game outGame = new EFModel.Game();

            outGame.GameID = Guid.NewGuid();
            outGame.UserID = DbGameList.dbContext.Users.Where(u => u.DisplayName == user).First().UserID;

            foreach( string tKey in refGame.Tags.Keys )
            {
                EFModel.Tag nextTag = new EFModel.Tag();
                nextTag.TagID = Guid.NewGuid();
                nextTag.TagName = tKey;
                nextTag.TagValue = refGame.Tags[tKey];
                nextTag.GameID = outGame.GameID;
                outGame.Tags.Add(nextTag);
            }

            foreach( Ply p in refGame.Plies )
            {
                EFModel.Ply nextPly = new EFModel.Ply();
                nextPly.PlyID = Guid.NewGuid();
                nextPly.GameID = outGame.GameID;
                nextPly.SourceSquare = p.src.ToInt();
                nextPly.DestSquare = p.dest.ToInt();
                nextPly.PromoPiece = (p.promo == null ? 'X' : p.promo.ToFENChar());
                nextPly.PlyNumber = p.Number;

                foreach (Comment comment in p.comments)
                {
                    EFModel.Comment nextComment = new EFModel.Comment();
                    nextComment.CommentID = Guid.NewGuid();
                    nextComment.PlyID = nextPly.PlyID;
                    nextComment.CommentValue = comment.value;
                    nextComment.commentEndsLine = comment.isToEOL;

                    nextPly.Comments.Add(nextComment);
                }
                outGame.Plies.Add(nextPly);
            }

            outGame.Terminator = refGame.TerminatorString;
            
            return outGame;
        }

        private Piece charToPieceType(int refInt)
        {
            return Piece.FromFENChar((char)refInt);
        }
    }
}
