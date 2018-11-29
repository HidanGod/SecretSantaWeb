namespace SecretSantaWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Surname = c.String(),
                        Password = c.String(),
                        BestowedParticipantId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Participants", t => t.BestowedParticipantId)
                .Index(t => t.BestowedParticipantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Participants", "BestowedParticipantId", "dbo.Participants");
            DropIndex("dbo.Participants", new[] { "BestowedParticipantId" });
            DropTable("dbo.Participants");
        }
    }
}
