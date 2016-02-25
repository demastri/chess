#undef doHandleVariations
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChessPosition.V2.PGN
{
    public class PGNGame : Game
    {
        // structures needed for PGN translation
        public enum GameSaveOptions { IncludeTags = 1, IncludeMoves = 2, IncludeComments = 4, IncludeVariations = 8, IncludeAll = 15, SimpleGameScore = 3, MoveListOnly = 2 };
        public string PGNSource;
        public List<int> plyStart;
        public List<int> plyEnd;
        public List<string> PGNPlySource;
        public List<PGNToken> PGNtokens;

        public PGNGame(PGNTokenizer pgt)
        {
            InitGame();

            if (pgt.tokens.Count > 0) // ok, we have one
            {
                PGNtokens = pgt.tokens;
                PGNSource = "";
                int tokenIndex = 0;

                BuildTagsFromTokens(PGNtokens, ref tokenIndex);
                BuildPliesFromTokens(PGNtokens, ref tokenIndex);
                BuildTerminatorFromTokens(PGNtokens, ref tokenIndex);

                // ok, we're out of tokens, must be a game in progress...that's the default...
                ResetPosition();
                PGNSource = GeneratePGNSource(GameSaveOptions.IncludeAll);
                return;
            }
        }
        public PGNGame(Game refGame)
        {
            InitGame();
            CopyGame(refGame);
        }
        public void SavePGN(StreamWriter sw)
        {
            int defaultLineTrigger = 90;
            sw.Write(GeneratePGNSource(defaultLineTrigger, GameSaveOptions.IncludeAll));
        }

        protected override void InitGame()
        {
            base.InitGame();
            PGNSource = "";
            PGNtokens = new List<PGNToken>();
            plyStart = new List<int>();
            plyEnd = new List<int>();
        }
        private void BuildTagsFromTokens(List<PGNToken> tokenList, ref int curIndex)
        {
            while (curIndex < tokenList.Count && tokenList[curIndex].tokenType == PGNTokenType.Tag)
                HandleTag((PGNTag)tokenList[curIndex++]);
        }
        private void BuildPliesFromTokens(List<PGNToken> tokenList, ref int curIndex)
        {
            Ply nextPly;
            while ((nextPly = BuildPlyFromToken(tokenList, ref curIndex)) != null)
            {
                nextPly.Number = Plies.Count;
                Plies.Add(nextPly);
                AdvanceTo(Plies.Count);
            }
        }
        private void BuildTerminatorFromTokens(List<PGNToken> tokenList, ref int curIndex)
        {
            // ### confirm that there's a terminator at the end...
            if (curIndex < tokenList.Count && tokenList[curIndex].tokenType == PGNTokenType.Terminator)
                Terminator = new PGNTerminator(tokenList[curIndex].tokenString).terminatorType;
            else
                Terminator = Terminators.InProgress;
            Tags["Result"] = TerminatorString;
        }

        private void BuildPlyFromToken_xxx(PGNToken token, ICollection<Ply> curPlyCollection, ref Ply lastPly, int curPlyNumber)
        {
            // ### for some variation handling - fix later...
        }
        private Ply BuildPlyFromToken(List<PGNToken> tokenList, ref int curIndex)
        {
            // the only things that should be here are move numbers, comments and move strings
            // anything else and we should be done.
            Ply outPly = null;
            bool done = false;
            while (!done)
            {
                PGNToken token = tokenList[curIndex];
                switch (token.tokenType)
                {
                    case PGNTokenType.Tag:
                    case PGNTokenType.Invalid:
                    case PGNTokenType.Terminator:
                        return outPly;  // return without consuming this token
                    case PGNTokenType.MoveNumber:
                        if (outPly != null)     // would start another new move...return the one we've built
                            return outPly;
                        curIndex++;             // otherwise start one here
                        break;
                    case PGNTokenType.Comment:  // could be one of many
                        outPly.comments.Add(new Comment(!((PGNComment)token).isBraceComment, token.value));
                        curIndex++;
                        break;
                    case PGNTokenType.MoveString:
                        if (outPly != null)
                            return outPly;
                        outPly = new Ply(); // otherwise start one here
                        AddMoveToPly(token.value, outPly);
                        curIndex++;             // consume this token
                        if (((PGNMoveString)token).variations != null)
                        {
#if doHandleVariations
                            Position varStartPosition = new Position(CurrentPosition);
                            foreach (List<PGNToken> var in ((PGNMoveString)token).variations)
                            {
                                CurrentPosition = new Position(varStartPosition);
                                List<Ply> varPlies = HandleVariation(var, curPlyCollection, curPlyNumber);
                                if (lastPly.variations == null)
                                    lastPly.variations = new List<List<Ply>>();
                                lastPly.variations.Add(varPlies);
                            }
                            CurrentPosition = new Position(varStartPosition);
#endif
                        }
                        break;
                }
            }
            return outPly;
        }

        public void GetMoveLocations(int thisPly, out int startLoc, out int moveLength)
        {
            startLoc = plyStart[thisPly];
            moveLength = plyEnd[thisPly] - plyStart[thisPly] + 1;
        }

        public static string GeneratePGNSource(Game thisGame)
        {
            PGNGame myGame = new PGNGame(thisGame);
            return myGame.GeneratePGNSource(-1, GameSaveOptions.MoveListOnly);
        }
        public string GeneratePGNSource(GameSaveOptions options)
        {
            return GeneratePGNSource(-1, options);
        }
        public string GeneratePGNSource(int newLineTrigger, GameSaveOptions options)
        {
            string outString = "";
            // tags
            if (((int)options & (int)GameSaveOptions.IncludeTags) != 0)
            {
                foreach (string tagKey in Tags.Keys)
                    outString += "[" + tagKey + " \"" + Tags[tagKey] + "\"]" + Environment.NewLine;
                outString += Environment.NewLine;
            }

            // moves
            if (((int)options & (int)GameSaveOptions.IncludeMoves) != 0)
            {
                outString += GeneratePGNSource(Plies, 0, outString.Length, newLineTrigger, options);

                // term
                outString += TerminatorString + Environment.NewLine + Environment.NewLine;
            }

            return PGNSource = outString;
        }
        public string GeneratePGNSource(List<Ply> variation, int curPly, int baseStrLen, int newLineTrigger, GameSaveOptions options)
        {
            string outString = "";
            int curLineLength = 0;

            ResetPosition();
            AdvanceTo(curPly - 1);
            // moves
            plyStart = new List<int>();
            plyEnd = new List<int>();
            PGNPlySource = new List<string>();
            foreach (Ply ply in variation)
            {

                string plyString = PGNPly.GeneratePGNSource(CurrentPosition, Plies[curPly], curPly, options, true);
                int dotLoc = plyString.IndexOf('.');
                string moveStr = TrimMoveNbr(plyString);

                if (dotLoc >= 0 && dotLoc < 6)
                    plyStart.Add(outString.Length + dotLoc + 1);
                else
                    plyStart.Add(outString.Length);
                PGNPlySource.Add(moveStr);

                plyEnd.Add(outString.Length + plyString.Length - 1);

                outString += plyString;
                curLineLength += plyString.Length;

                if (curLineLength > newLineTrigger && newLineTrigger > 0)
                {
                    outString += Environment.NewLine;
                    curLineLength = 0;
                }

                AdvanceTo(++curPly);
            }
            return PGNSource = outString;
        }
        private void HandleTag(PGNTag t)
        {
            string valString = t.value;
            if (valString.Length >= 2 && valString[0] == '\"' && valString[valString.Length - 1] == '\"')
            {
                valString = valString.Substring(1);
                valString = valString.Substring(0, valString.Length - 1);
            }
            if (Tags.ContainsKey(t.key))
                Tags[t.key] = valString;
            else
                Tags.Add(t.key, valString);
        }
        public string TrimMoveNbr(string s)
        {
            int dotLoc = s.IndexOf('.');

            if (dotLoc >= 0 && dotLoc < 6)
                return s.Substring(dotLoc + 1);
            return s;
        }

        private List<Ply> HandleVariation(List<PGNToken> var, ICollection<Ply> curPlyCollection, int curPlyNumber)
        {
            /// the token contains any embedded MoveText
            /// simply turn them into Plies or add to existing plies as necessary, and return the set
            List<Ply> outPly = new List<Ply>();
#if doHandleVariations
            Ply curPly = null;
            foreach (PGNToken token in var)
            {
                BuildPlyFromToken(token, outPly, ref curPly, curPlyNumber + outPly.Count);
            }
#endif
            return outPly;
        }

        public string BuildMoveList()
        {
            string outStr = "";
            foreach (PGNToken t in PGNtokens)
            {
                switch (t.tokenType)
                {
                    case PGNTokenType.MoveNumber:
                        outStr += t.tokenString;
                        break;
                    case PGNTokenType.MoveString:
                        outStr += t.tokenString + " ";
                        break;
                }
            }
            if (Plies.Count % 2 == 0) // white on move
                outStr += (Plies.Count / 2 + 1).ToString() + ".";
            outStr += " ...";
            return outStr;
        }

    }
}
