using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ProiectFinal.Models;

namespace ProiectFinal.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; }

        public int Rating { get; set; }

        public DateTime Date { get; set; }

        public int ProdId { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Product Product { get; set; }
    }
}