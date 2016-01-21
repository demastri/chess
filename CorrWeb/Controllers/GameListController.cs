using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using CorrWeb.Models;

namespace CorrWeb.Controllers
{
    public class GameListController : ApiController
    {
        public Dictionary<string, GameList> ListSet;
        GameList currentGameList;
        
        // GET api/GameList
        public IEnumerable<GameList> GetGameLists()
        {
            ListSet = InitListSet();
            return ListSet.Values;
        }
        private Dictionary<string, GameList> InitListSet()
        {
            Dictionary<string, GameList> outSet = new Dictionary<string, GameList>();
            // this is meaningless - no need for the crossuser set
            // this is a placeholder in case we want multiple sets per user
            return outSet;
        }

        // GET api/GameList/5
        public GameList GetGameList(string id)
        {
            return currentGameList = new GameList(id);  // ListSet[id];
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}