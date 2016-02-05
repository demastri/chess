﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Web.Mvc;
using ChessPosition;

namespace CorrWeb.Models
{
    public class GameList
    {
        ChessPosition.GameList refGameList;
        static public GameList GameListContext = null;

        private static Dictionary<string, List<ChessPosition.Game>> gameCache = null;
        private string _uid;
        private List<ChessPosition.Game> _games;

        public List<string> eventList;
        public Dictionary<string, List<ChessPosition.Game>> onMoveGameList;
        public Dictionary<string, List<ChessPosition.Game>> waitingGameList;
        public Dictionary<string, List<ChessPosition.Game>> completeGameList;

        [Required]
        public string UserId { get { return _uid; } }
        public int selGameIndex { get; set; }
        public static int panelSelect = 0;
        public virtual List<ChessPosition.Game> Games { get { return _games; } }

        public GameList()
        {
            refGameList = null;
            panelSelect = ((panelSelect) + 1 % 4);
            selGameIndex = -1;
            if (gameCache == null)
                gameCache = new Dictionary<string, List<ChessPosition.Game>>();
            _uid = "";
            _games = new List<ChessPosition.Game>();
        }
        public GameList(string uid)
        {
            refGameList = new ChessPosition.GameList( uid );
            panelSelect = ((panelSelect) + 1 % 4);
            selGameIndex = -1;
            if (gameCache == null)
                gameCache = new Dictionary<string, List<ChessPosition.Game>>();

            _uid = uid;
            if (gameCache.ContainsKey(uid))
                _games = gameCache[uid];
            else
            {
                string refDataLocation = "C:\\Projects\\JPD\\BBRepos\\Chess\\CorrWeb\\App_Data\\";
                string parserLocation = "C:\\Projects\\JPD\\BBRepos\\Chess\\ChessPosition\\Parser\\Grammars\\PGNSchema.xml";
                string fileLocation = refDataLocation + uid + ".pgn";
                _games = refGameList.Games;
                gameCache.Add(_uid, _games);
            }
            ParseGameLists();
        }
        public int Count()
        {
            return Games.Count;
        }
        public ChessPosition.Game FindGame(int listIndex, string eventIndex, int gameIndex)
        {
            Dictionary<string, List<ChessPosition.Game>> outList =
                (listIndex == 0 ? onMoveGameList :
                (listIndex == 1 ? waitingGameList :
                (listIndex == 2 ? completeGameList : null)));
            if (outList != null)
                return outList[eventIndex][gameIndex];
            return null;
        }
        public string FindGameName(int listIndex, string eventIndex, int gameIndex)
        {
            ChessPosition.Game thisGame = FindGame(listIndex, eventIndex, gameIndex);
            if (thisGame == null)
                return "";
            return thisGame.Tags["White"] + " - " + thisGame.Tags["Black"] + ": " + thisGame.Tags["Result"];
        }


        private void ParseGameLists()
        {
            string plrDispName = "DeMastri, John";  // ###

            eventList = new List<string>();
            onMoveGameList = new Dictionary<string, List<ChessPosition.Game>>();
            waitingGameList = new Dictionary<string, List<ChessPosition.Game>>();
            completeGameList = new Dictionary<string, List<ChessPosition.Game>>();

            foreach (ChessPosition.Game g in _games)
            {
                string thisEvent = g.Tags["Event"];

                if (g.Tags["Result"] != "*")
                {
                    if (!completeGameList.ContainsKey(thisEvent))
                        completeGameList.Add(thisEvent, new List<ChessPosition.Game>());
                    completeGameList[thisEvent].Add(g);
                }
                else
                {
                    string wPlr = g.Tags["White"];
                    string bPlr = g.Tags["Black"];
                    bool onMove =
                      ((g.OnMove == ChessPosition.PlayerEnum.White && wPlr == plrDispName) ||
                       (g.OnMove == ChessPosition.PlayerEnum.Black && bPlr == plrDispName));
                    if (onMove)
                    {
                        if (!onMoveGameList.ContainsKey(thisEvent))
                            onMoveGameList.Add(thisEvent, new List<ChessPosition.Game>());
                        onMoveGameList[thisEvent].Add(g);
                    }
                    else
                    {
                        if (!waitingGameList.ContainsKey(thisEvent))
                            waitingGameList.Add(thisEvent, new List<ChessPosition.Game>());
                        waitingGameList[thisEvent].Add(g);
                    }
                }
            }
        }
        public HtmlString GetPositionString(int listIndex, string eventIndex, int gameIndex, int positionIndex)
        {
            string emptyBoard =
                "!\"\"\"\"\"\"\"\"#<br />" +
                "ç + + + +%<br />" +
                "æ+ + + + %<br />" +
                "å + + + +%<br />" +
                "ä+ + + + %<br />" +
                "ã + + + +%<br />" +
                "â+ + + + %<br />" +
                "á + + + +%<br />" +
                "à+ + + + %<br />" +
                "/èéêëìíîï)<br />";
            ChessPosition.Game curGame = FindGame(listIndex, eventIndex, gameIndex);
            
            string thisBoard = emptyBoard;
            if (curGame != null)
            {
                curGame.ResetPosition();
                curGame.AdvancePosition(positionIndex);

                foreach (Square sq in curGame.CurrentPosition.board.Keys)
                {
                    Piece thisPc = curGame.CurrentPosition.board[sq];
                    char thisPcChar = Char.ToLower(thisPc.ToChess7Char);

                    thisBoard = PokePiece(thisBoard, sq.row + 1, sq.col + 1, thisPc, false);
                }
            }
            return new HtmlString(thisBoard);
        }
        private string PokePiece(string refStr, int rank, int file, Piece pc, bool invert) // rank/file ranged 1-8
        {
            if (invert)
            {
                rank = 9 - rank;
                file = 9 - file;
            }
            int locToPoke = ((10 + "<br />".Length) * (1 + 8 - rank)) + (file);
            bool isWhite = (((((rank - 1) % 2) == ((file - 1) % 2)) ? 1 : 0) == 0);   // 1 => b
            char pcChar = isWhite ? Char.ToLower(pc.ToChess7Char) : Char.ToUpper(pc.ToChess7Char);  // upper case is on a dark square...

            return JPD.Utilities.Utils.SwapChar(refStr, locToPoke, pcChar);
        }
    }
}