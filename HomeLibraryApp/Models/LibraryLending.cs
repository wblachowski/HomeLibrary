﻿using System;
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

        public int? OutBookId { get; set; }
        [ForeignKey("OutBookId")]
        public Book OutBook { get; set; }

        //which book
        public int? LibraryBookId { get; set; }
        [ForeignKey("LibraryBookId")]
        public LibraryBook LibraryBook { get; set; }

        //to whom
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}