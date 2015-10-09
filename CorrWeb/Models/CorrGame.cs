using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorrWeb.Models
{
    public class CorrGame
    {
        public int CorrGameId { get; set; }

        [Required]
        public string Opponent { get; set; }
        public string Event { get; set; }
        public string Result { get; set; }

        [ForeignKey("GameList")]
        public int GameListId { get; set; }
        public virtual GameList GameList { get; set; }
    }
}
