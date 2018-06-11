using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryBooksWithStates
    {
        public IEnumerable<Book> BooksModel { get; set; }
        public IEnumerable<string> BooksStates { get; set; }
        public string LibId { get; set; }
    }
}