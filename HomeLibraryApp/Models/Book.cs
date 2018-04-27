using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HomeLibraryApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        
    }
}