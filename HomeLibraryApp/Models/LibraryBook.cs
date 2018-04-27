using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryBook
    {
        [Key]
        public int Id { get; set; }
        public Book Book { get; set; }
        public Library Library { get; set; }
    }
}