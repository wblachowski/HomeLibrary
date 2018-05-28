using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryBookDetails
    {
        public Book Book { get; set; }
        public IEnumerable<LibraryComment> Comments { get; set; }

        [Display(Name = "Comment")]
        public String YourComment { get; set; }

        public int UserReadingId { get; set; }
        [ForeignKey("UserReadingId")]
        public UserReading UserReading { get; set; }

        public int LibraryLendingId { get; set; }
        [ForeignKey("LibraryLendingId")]
        public LibraryLending LibraryLending { get; set; }
    }
}