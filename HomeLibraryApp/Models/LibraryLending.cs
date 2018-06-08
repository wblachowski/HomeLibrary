using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryLending
    {
        [Key]
        public int Id { get; set; }

        //which book         
        public int? LibraryBookId { get; set; }
        [ForeignKey("LibraryBookId")]
        public LibraryBook LibraryBook { get; set; }

        //to whom - if internal
        public int? CopyLibraryBookId { get; set; }
        [ForeignKey("CopyLibraryBookId")]
        public LibraryBook CopyLibraryBook { get; set; }

        //from whom - if external
        public string ExternalLender { get; set; }

        //to whom - if external
        public string ExternalBorrower { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}