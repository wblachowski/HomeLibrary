using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HomeLibraryApp.Models
{
    public class Library
    {
        [Key]
        public int Id { get; set;}

        [Required(ErrorMessage = "Library name is required!")]
        [StringLength(250, ErrorMessage = "Maximal length of the name of a library is 250 characters!")]
        public string Name { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}