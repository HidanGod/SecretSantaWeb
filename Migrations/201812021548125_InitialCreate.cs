namespace SecretSantaWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        ParticipantId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Surname = c.String(),
                        Password = c.String(),
                        BestowedParticipantId = c.Int(),
                        GroupId = c.Int(),
                        BestowedParticipant_ParticipantId = c.Int(),
                    })
                .PrimaryKey(t => t.ParticipantId)
                .ForeignKey("dbo.Participants", t => t.BestowedParticipant_ParticipantId)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .Index(t => t.GroupId)
                .Index(t => t.BestowedParticipant_ParticipantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Participants", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Participants", "BestowedParticipant_ParticipantId", "dbo.Participants");
            DropIndex("dbo.Participants", new[] { "BestowedParticipant_ParticipantId" });
            DropIndex("dbo.Participants", new[] { "GroupId" });
            DropTable("dbo.Participants");
            DropTable("dbo.Groups");
        }
    }
}
