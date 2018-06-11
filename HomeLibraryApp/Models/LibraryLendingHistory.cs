using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibraryLendingHistory
    {
        public Book Book { get; set; }

        public IEnumerable<LibraryLending> Lendings { get; set; }

        public IEnumerable<String> LenderName { get; set; }

        public IEnumerable<String> BorrowerName { get; set; }
    }
}