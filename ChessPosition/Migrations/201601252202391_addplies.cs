namespace ChessPosition.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addplies : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Plies",
                c => new
                    {
                        PlyID = c.Guid(nullable: false),
                        src_row = c.Byte(nullable: false),
                        src_col = c.Byte(nullable: false),
                        dest_row = c.Byte(nullable: false),
                        dest_col = c.Byte(nullable: false),
                        Game_GameID = c.Guid(),
                    })
                .PrimaryKey(t => t.PlyID)
                .ForeignKey("dbo.Games", t => t.Game_GameID)
                .Index(t => t.Game_GameID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Plies", "Game_GameID", "dbo.Games");
            DropIndex("dbo.Plies", new[] { "Game_GameID" });
            DropTable("dbo.Plies");
        }
    }
}
