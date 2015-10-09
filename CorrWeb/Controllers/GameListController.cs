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
        private TodoItemContext db = new TodoItemContext();

        // GET api/GameList
        public IEnumerable<GameList> GetGameLists()
        {
            return db.GameLists.AsEnumerable();
        }

        // GET api/GameList/5
        public GameList GetGameList(int id)
        {
            GameList gamelist = db.GameLists.Find(id);
            if (gamelist == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return gamelist;
        }

        // PUT api/GameList/5
        public HttpResponseMessage PutGameList(int id, GameList gamelist)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != gamelist.GameListId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(gamelist).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/GameList
        public HttpResponseMessage PostGameList(GameList gamelist)
        {
            if (ModelState.IsValid)
            {
                db.GameLists.Add(gamelist);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, gamelist);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = gamelist.GameListId }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/GameList/5
        public HttpResponseMessage DeleteGameList(int id)
        {
            GameList gamelist = db.GameLists.Find(id);
            if (gamelist == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.GameLists.Remove(gamelist);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, gamelist);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}