using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web.Mvc;

namespace SecretSantaWeb.Models
{
    public class Participant
    { 
        [Key]
        [DisplayName("ID")]
        public int ParticipantId { get; set; }
        [DisplayName("Имя")]
        public string Name { get; set; }
        [DisplayName("Фамилия")]
        public string Surname { get; set; }
        [DisplayName("Пароль")]
        public string Password { get; set; }
        public int? BestowedParticipantId { get; set; }
        public Participant BestowedParticipant { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
    }
   
}