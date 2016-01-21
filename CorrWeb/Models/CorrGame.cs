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
        public ChessPosition.Game thisGame { get; set; }

        public virtual GameList thisGameList { get; set; }
    }
}
