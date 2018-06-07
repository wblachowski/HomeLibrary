using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryAdd
    {
        public Book NewBookModel { get; set; }
        public Book GoodreadsBookModel { get; set; }

        [Display(Name = "Goodreads book link")]
        public string GoodreadsUrl { get; set; }

        public IEnumerable<Library> UserLibraries { get; set; }

        public string LenderFirstname { get; set; }
        public string LenderLastname { get; set; }
    }
}