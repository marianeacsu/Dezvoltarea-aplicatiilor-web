using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ProiectFinal.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectFinal.Models
{
    public class Product
    {
        [Key]
        public int ProdId { get; set; }


        [Required(ErrorMessage = "Numele produsului este obligatoriu")]
        [StringLength(100, ErrorMessage = "Numele nu poate avea mai mult de 20 de caractere")]
        public string ProdName { get; set; }

        [Required(ErrorMessage = "Continutul produsului este obligatoriu")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Required(ErrorMessage = "Pretul produsului este obligatoriu")]
        public int ProdPrice { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Categoria obligatorie")]
        public int CategoryId { get; set; }

        public string FileName { get; set; }
        public string UserId { get; set; }
        public bool Accepted { get; set; }
        public double ProdRating { get; set; }
        [NotMapped]
        public HttpPostedFileBase Image { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }


    }
}