using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class Home
    {
        public RegisterViewModel RegisterViewModel { get; set; }
        public double Stats { get; set; }

        public Book LastBook { get; set; }
        public UserReading LastBookReading { get; set; }

        public IEnumerable<Book> CurrentBooks { get; set; }
        public IEnumerable<LibraryBook> CurrentLibraryBooks { get; set; }

        public IEnumerable<Book> LentBooks { get; set; }
        public IEnumerable<LibraryBook> LentLibraryBooks { get; set; }
        public IEnumerable<String> LentNames { get; set; }

        public IEnumerable<Book> BorrowedBooks { get; set; }
        public IEnumerable<LibraryBook> BorrowedLibraryBooks { get; set; }
        public IEnumerable<String> BorrowedNames { get; set; }

    }
}