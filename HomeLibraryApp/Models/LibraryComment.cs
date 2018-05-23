using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryComment
    {
        [Key]
        public int Id { get; set; }

        public int LibraryBookId { get; set; }
        [ForeignKey("LibraryBookId")]
        public LibraryBook LibraryBook { get; set; }


        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}