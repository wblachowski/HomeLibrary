using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryBook
    {
        [Key]
        public int Id { get; set; }

        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; }

        public int LibraryId { get; set; }
        [ForeignKey("LibraryId")]
        public Library Library { get; set; }
    }
}