using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecretSantaWeb.Models
{
    public class Group
    {
        [Key]
        [DisplayName("ID")]
        public int GroupId { get; set; }
        [DisplayName("Группа")]
        public string Title { get; set; }
        [DisplayName("Пароль")]
        public string Password { get; set; }
        [DisplayName("Участники")]
        public ICollection<Participant> Participants { get; set; }
        public Group()
        {
            Participants = new List<Participant>();
        }
    }
}