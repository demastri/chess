using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CorrWeb.Models
{
    public class GameList
    {
        private static Dictionary<string, List<ChessPosition.Game>> gameCache = null;
        private string _uid;
        private List<ChessPosition.Game> _games;

        public List<string> eventList;
        public Dictionary<string, List<ChessPosition.Game>> onMoveGameList;
        public Dictionary<string, List<ChessPosition.Game>> waitingGameList;
        public Dictionary<string, List<ChessPosition.Game>> completeGameList;

        [Required]
        public string UserId { get { return _uid; } }
        public virtual List<ChessPosition.Game> Games { get { return _games; } }

        public GameList()
        {
            if (gameCache == null)
                gameCache = new Dictionary<string, List<ChessPosition.Game>>();
            _uid = "";
            _games = new List<ChessPosition.Game>();
        }
        public GameList(string uid)
        {
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
                _games = ChessPosition.Game.ReadPGNFile(fileLocation, parserLocation);
                gameCache.Add(_uid, _games);
            }
            ParseGameLists();
        }
        public int Count()
        {
            return Games.Count;
        }

        private void ParseGameLists() 
        {
            string plrDispName = "DeMastri, John";  // ###

            eventList = new List<string>();
            onMoveGameList = new Dictionary<string,List<ChessPosition.Game>>();
            waitingGameList = new Dictionary<string,List<ChessPosition.Game>>();
            completeGameList = new Dictionary<string, List<ChessPosition.Game>>();
        
            foreach( ChessPosition.Game g in _games )
            {
                string thisEvent = g.Tags["Event"];

                if( g.Tags["Result"] != "*" )
                {
                    if( !completeGameList.ContainsKey( thisEvent ) )
                        completeGameList.Add( thisEvent, new List<ChessPosition.Game>() );
                    completeGameList[thisEvent].Add(g);
                }
                else
                {
                    string wPlr = g.Tags["White"];
                    string bPlr = g.Tags["Black"];
                    bool onMove = 
                      ((g.OnMove == ChessPosition.PlayerEnum.White && wPlr == plrDispName) ||
                       (g.OnMove == ChessPosition.PlayerEnum.Black && bPlr == plrDispName ));
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
    }
}