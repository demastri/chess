using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JPD.Parser;

namespace ChessPosition
{
    public class PGNTokenizer
    {
        public string pgn;
        static public string startNextGameTag = "";
        public List<PGNToken> tokens;
        List<Sentence> games;

        public void TokenizeViaParser(StreamReader sr)
        {
            string grammarFile = "Parser/Grammars/PGNSchema.xml";
            string corpusFile = "Parser/Corpora/Sample.pgn";
            try
            {
                Parser p = new Parser(grammarFile);
                List<Token> parseTokens = p.Tokenize(sr);
                //List<Token> parseTokens = p.Tokenize(corpusFile);
                games = p.Compose(parseTokens);
                // at this point, we have Sentences (Games) that conform to the Grammar
                // composed of tokens we can directly translate into domain-usable objects
                // the list of sentences correspond to the games in the file
                // foreach game we have the tree of ParseNodes to walk, which resolve to a set of instantiated tokens
                LoadGame(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        
        }

        public int GameCount { get { return games.Count; } }

        public void LoadGame(int i)
        {
            tokens = new List<PGNToken>();
            if (i < 0 || i >= GameCount)
                return;
            Sentence game = games[i];   // for each game
            List<PGNToken> tagTokens = PGNToken.TokenFactory(game, game.node, "TagSection/TagPair");
            tokens.AddRange(tagTokens);
            List<PGNToken> moveTokens = PGNToken.TokenFactory(game, game.node, "MoveSection/*");
            tokens.AddRange(moveTokens);
            List<PGNToken> termTokens = PGNToken.TokenFactory(game, game.node, "GameTerminator");
            tokens.AddRange(termTokens);
        }

        public PGNTokenizer(StreamReader sr)
        {
            TokenizeViaParser(sr);
            return;


            // read and add to pgn until you see a terminator...
            // another valid end of game state is whitespace then another tag
            // ### could be embedded with comments afterwards
            if( startNextGameTag != "" )
                pgn = startNextGameTag+ " " + Environment.NewLine;
            else
                pgn = "";
            startNextGameTag = "";

            bool inAGame = false;

            bool done = false;
            while (!done && !sr.EndOfStream)
            {
                bool inATag = false;
                string line = sr.ReadLine();
                if (line.Trim() != "")
                {
                    if (line[0] == '[')
                        inATag = true;
                    else
                        inAGame = true;

                    if (inAGame && inATag) // ok - we've rolled to the next one 
                    {
                        startNextGameTag = line + Environment.NewLine;
                        done = true;
                        continue;
                    }

                    pgn += line + " " + Environment.NewLine;
                    foreach (string s in PGNTerminator.terminators)
                        if (line.IndexOf(s) >= 0 && line.Substring(line.IndexOf(s) + s.Length).Trim() == "")
                            done = true;
                }
            }
            Tokenize();
        }
        public PGNTokenizer(string s)
        {
            pgn = s;
            Tokenize();
        }
        public void Tokenize()
        {
            TokenizeViaParser(null);
            return; // 
            /// full definition of actual PGN token set:
            /// http://www6.chessclub.com/help/PGN-spec
            /// starts as an input string
            ///     whitespace consists of space, tab, newline
            ///     comments, outside of any token:
            ///         starting with a ; character to the end of line
            ///         starting with a { proceeding to the next } character, do not nest
            ///     escape mechanism
            ///         % character in the first column of a line, ONLY - remainder of line ignored by "normal" scanning tools
            ///     string token
            ///         series of 0+ printing chars delim's by " characters, internal \ rep'd by \\, internal " rep'd by \"
            ///         non-printing chars, like nl and tab are NOT permitted inside strings
            ///     integer token
            ///         series of 1+ decimal digit characters, terminated prior to first non-symbol char following integer sequence
            ///         special case of symbol token
            ///     period token
            ///         . is self terminating, used for move number indications
            ///     asterisk token
            ///         * is self terminating, used as possible game terminator
            ///     L/R bracket characters
            ///         [ ] are self terminating, used to delimit tag pairs
            ///     L/R angle bracket characters
            ///         < > are self terminating, used for future expansion
            ///     L/R paren characters
            ///         ( ) are self terminating, used to delimit Recursive Annotation Variations
            ///     NAG Glyph
            ///         $ (dollar sign) followed by 1+ digit characters
            ///     symbol token
            ///         letter or digit followed by 0+ "symbol continuation characters" - AZaz09_+#=:-
            ///         terminated prior to first non-symbol character
            ///         
            ///     Game composed of 2 sections
            ///         Tag section
            ///             contains 0+ tag pairs - 4 consecutive tokens: [ symbol string ]
            ///                 symbol is the tag name
            ///                     must be unique within a single tag section (case sensitive)
            ///                     should begin with an upper case letter (for Archival Storage)
            ///                     composed of letters, digits and underscore
            ///                 string is the tag value
            ///                     within the string, : is used as a multiple value separator ONLY
            ///                 0+ whitespace characters between tokens in a tag pair (import)
            ///                 no whitespace between [ and symbol or string and ], single space between symbol and string (export)
            ///                 multiple tag pairs on the same line, or single tag pair spanning multiple lines (import)
            ///                 each tag pair left justified on its own line, single empty line following last tag pair (export)
            ///             Mandatory tags (STR - Seven Tag Roster) for Archival Storage:
            ///                 Event, 
            ///                     Descriptive, consistent name, use single ? character if name is unavailable
            ///                 Site
            ///                     City, Region and standard name for country, use IOC name if appropriate, use single ? character if unknown
            ///                 Date
            ///                     Starting date for THIS GAME, in format YYYY.MM.DD, using ?? as appropriate for unknown values
            ///                 Round
            ///                     Ordinal round number, single dash - if inappropriate, single ? if unknown, multipart format 4.1.2 ok as appropriate
            ///                 White, Black
            ///                     Family name followed by comma and space, followed by first name or inital, then any middle name or initial
            ///                     Initials followed by periods, commas followed by spaces
            ///                 Result
            ///                     One of these 4 strings:
            ///                         1-0 0-1 1/2-1/2 *
            ///             Other Tags (wb means that tag has WhiteTagName and BlackTagName variants, ev means EventTagName variant):
            ///                 wbTitle, wbElo, wbUSCF, wbNA (network address or email), wbType (human/program)
            ///                 evDate (Date is the Game Date, this is the Event Date), evSponsor
            ///                 Section, Stage, Board, 
            ///                 Opening Information, Opening, Variation, SubVariation, ECO, NIC
            ///                 Time (HH:MM:SS), UTCTime, UTCDate
            ///                 TimeControl (6 types)
            ///                     ? - move/sec suddenDeathSec baseSec+moveIncSec *sandclockSecs
            ///                 SetUp 0 => normal pos, 1 => nonstandard, read from FEN tag
            ///                 FEN
            ///                 Termination (typ reason for game conclusion)
            ///                 Annotator, Mode, PlyCount 
            ///         
            ///
            ///         movetext section (uses Standard Algebraic Notation, ending with a game terminator)
            ///             Move number indications:
            ///                 integer tokens followed by 0+ period tokens before following white or black move
            ///                 not required at all, and superfluous ones are allowed if correct (import)
            ///                 0+ whitespace characters between the digit sequence and an (optional) period (import)
            ///                 White move number is an integer giving the fullmove number with a single period appended (export)
            ///                 All White move elts have a preceding move number indication (export)
            ///                 Black move number is an integer giving the fullmove number with three period chars appended (export)
            ///                 Black move elements have a preceding move number indication if (export):
            ///                     there is an intervening annotation or comment between the black move and the prev white move
            ///                     there is no prev white move (if game/position starts with black to move)
            ///             Movetext SAN (FAN may also be used)
            ///                 Square identification - two char name - col a-h, row 1-8
            ///                 Piece identification - PRNBKQ - P is not used in PGN export, may appear in input
            ///                 Move construction
            ///                     Moving piece letter (xP) followed by destination square
            ///                     All captures include 'x' prior to destination square
            ///                     Pawn captures include file of originiating square prior to 'x'
            ///                     Castling designated by O-O or O-O-O
            ///                     ep captures have no special notation - as if captured P was on the destination sq
            ///                     Promotions are denoted by '=' following the destination square, with a piece letter following '='
            ///                     Disambiguation (in order)
            ///                         use file if it completely disambuguates the source
            ///                         use rank if it completely disambuguates the source
            ///                         otherwise use full source square
            ///                     + and # should be appended as objective indicators
            ///                     No special markings for double or discovered checkes, or drawing moves
            ///                 Move suffix annotations
            ///                     ! ? !! !? ?! ?? at most one of these appended as last part of move (import)
            ///                     on export, use the Numeric Annotation Glyph as appropriate (export)
            ///                 Movetext RAV (Recursive Annotation Variation)
            ///                     Sequence of 1+ moves enclosed in parens
            ///                     unplay the move prior to the RAV
            ///                     can be nested
            ///                 Game termination
            ///                     eacn movetext section has exactly one game term marker as the last element matching Result tag
            ///             NAG definition - others are also defined
            ///                 0 1 2 3  4  5  6 
            ///                   ! ? !! ?? !? ?!
            ///                 1-9 describe the move
            ///                 10-135 describe the position
            ///                 136-139 describe time pressure
            ///                 
            ///         Within a file, games should be collated (ascending) by
            ///             Date, Event, Site, White, Black, Result, movetext



            tokens = new List<PGNToken>();
            for( int i=0; i<pgn.Length; )
            {
                if (char.IsWhiteSpace(pgn[i]))
                    i++;
                else
                {
                    int refi = i;
                    PGNToken pgntoken = BuildToken( ref i );
                    if (pgntoken != null)
                    {
                        tokens.Add(pgntoken);
                    }
                    if (i < refi)
                        i = refi;
                }
            }
        }
        public PGNToken BuildToken( ref int i ) 
        {
            PGNToken outToken = null;
            switch (pgn[i])
            {
                case '[':   // should be a tag value
                    outToken = new PGNTag(pgn, i);
                    break;
                case '{':   // should be an annotation
                    outToken = new PGNComment(pgn, i);
                    break;
                case ',':   // should be a comment
                    outToken = new PGNComment(pgn, i);
                    break;
                case '(':   // should be a variation! ###
                    outToken = new PGNComment(pgn, i);
                    break;
                default:   // should be a move number or string...
                    outToken = new PGNTerminator(pgn, i);
                    if( outToken.tokenType == PGNTokenType.Invalid )
                        if( char.IsDigit(pgn[i]) )
                            outToken = new PGNMoveNumber(pgn, i);
                        else if (("PRNBKQabcdefghO").IndexOf(pgn[i]) >= 0)
                            outToken = new PGNMoveString(pgn, i);
                    break;
            }
            if (outToken == null || outToken.tokenType == PGNTokenType.Invalid)
                return null;
            i = outToken.startLocation + outToken.tokenString.Length;
            return outToken;
        }

    }
}
