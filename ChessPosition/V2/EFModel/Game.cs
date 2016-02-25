//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChessPosition.V2.EFModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Game
    {
        public Game()
        {
            this.Plies = new HashSet<Ply>();
            this.Tags = new HashSet<Tag>();
        }
    
        public System.Guid GameID { get; set; }
        public System.Guid UserID { get; set; }
        public string Terminator { get; set; }
    
        public virtual User User { get; set; }
        public virtual ICollection<Ply> Plies { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}