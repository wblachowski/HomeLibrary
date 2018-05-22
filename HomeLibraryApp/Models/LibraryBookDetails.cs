using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryBookDetails
    {
        public Book Book { get; set; }
        public IEnumerable<LibraryComment> Comments { get; set; }
    }
}