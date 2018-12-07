namespace SecretSantaWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "PasswordView", c => c.String());
            AddColumn("dbo.Groups", "PasswordDelete", c => c.String());
            DropColumn("dbo.Groups", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Groups", "Password", c => c.String());
            DropColumn("dbo.Groups", "PasswordDelete");
            DropColumn("dbo.Groups", "PasswordView");
        }
    }
}
