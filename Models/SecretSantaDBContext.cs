using System;
using System.Data.Entity;


namespace SecretSantaWeb.Models
{
    public class SecretSantaDBContext : DbContext
    {
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Group> Groups { get; set; }
    }


}
//enable-migrations -ContextTypeName SecretSantaWeb.Models.SecretSantaDBContext
//Add-Migration "MigrateDB"
//Update-Database