using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace ProiectFinal.Models
{
    public class Image
    {
        [Key]
        public int FileId { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
    }
}