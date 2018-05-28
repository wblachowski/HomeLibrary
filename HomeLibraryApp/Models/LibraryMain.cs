using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryMain
    {
        public Book NewBookModel { get; set; }
        public IEnumerable<Library> LibrariesModel { get; set; }
        public LibraryBooksWithStates LibraryBooksWithStates { get; set; }
    }
}