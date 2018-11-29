using System;
using System.Data.Entity;

namespace SecretSantaWeb.Models
{
    public class Participant
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public int? BestowedParticipantId { get; set; }
        public Participant BestowedParticipant { get; set; }
    }
   
}