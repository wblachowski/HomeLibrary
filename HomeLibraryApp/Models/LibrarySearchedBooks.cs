using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibrarySearchedBooks
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Library> Libraries { get; set; }
    }
}