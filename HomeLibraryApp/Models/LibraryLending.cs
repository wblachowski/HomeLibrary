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

        //which book - external
        public int? OutBookId { get; set; }
        [ForeignKey("OutBookId")]
        public Book OutBook { get; set; }

        //which book - internal
        public int? LibraryBookId { get; set; }
        [ForeignKey("LibraryBookId")]
        public LibraryBook LibraryBook { get; set; }

        //to whom - if internal
        public int? CopyLibraryBookId { get; set; }
        [ForeignKey("CopyLibraryBookId")]
        public LibraryBook CopyLibraryBook { get; set; }

        //to whom - if external
        public string ExternalPerson { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}