using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeLibraryApp.Models
{
    public class LibrarySearch
    {
        public IEnumerable<Library> UserLibraries { get; set; }

        [Display(Name = "Search")]
        public string q { get; set; }

        public string ErrorMsg { get;set; }
    }
}