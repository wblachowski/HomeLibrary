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

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Author's first name")]
        public string AuthorFirstname { get; set; }

        [Required]
        [Display(Name = "Author's last name")]
        public string AuthorLastname { get; set; }

        [Required]
        [Display(Name = "Publication date")]
        [DataType(DataType.DateTime)]
        public DateTime? PublicationDate { get; set; } = null;

        [Required]
        [Display(Name = "Publisher")]
        public string Publisher { get; set; }


    }
}