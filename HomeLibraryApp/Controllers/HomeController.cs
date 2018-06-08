using HomeLibraryApp.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeLibraryApp.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            Home home = new Home();
            if (User.Identity.IsAuthenticated)
            {
                home.Stats = GetReadingStats();
                home.LastBookReading = GetLastUserReading();
                home.LastBook = GetLastBook(home.LastBookReading);
                home.CurrentBooks = GetCurrentBooks();
                home.BorrowedLibraryBooks = GetBorrowedLibraryBooks();
                home.BorrowedBooks = GetBooksDetails(home.BorrowedLibraryBooks);
                home.BorrowedNames = GetNames(home.BorrowedLibraryBooks,"borrowed");
                home.LentLibraryBooks = GetLentLibraryBooks();
                home.LentBooks = GetBooksDetails(home.LentLibraryBooks);
                home.LentNames = GetNames(home.LentLibraryBooks, "lent");
            }
            else
            {
                home.RegisterViewModel = new RegisterViewModel();
            }
            return View(home);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private double GetReadingStats()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.FirstOrDefault(us => us.Id.ToString() == userId);
            if (user == null) return 0;
            double noOfBooks = db.UserReadings.Where(ur => ur.UserId == userId && ur.StartDate != null && ur.EndDate != null).Count();
            double noOfMonths = Math.Ceiling((DateTime.Now - user.Created).TotalDays / 30.0);
            return noOfBooks / noOfMonths;
        }

        private Book GetLastBook(UserReading userReading)
        {
            if (userReading == null)
            {
                return null;
            }
            else
            {
                Book book = db.Books.FirstOrDefault(bk => bk.Id == userReading.BookId);
                return book;
            }
        }

        private UserReading GetLastUserReading()
        {
            string userId = User.Identity.GetUserId();
            return db.UserReadings.Where(ur => ur.UserId == userId && ur.StartDate != null && ur.EndDate != null)
                .OrderByDescending(ur => ur.EndDate).FirstOrDefault();
        }

        private IEnumerable<Book> GetCurrentBooks()
        {
            string userId = User.Identity.GetUserId();
            List<UserReading> userReadings = db.UserReadings.Where(ur => ur.UserId == userId && ur.StartDate != null && ur.EndDate == null).OrderByDescending(ur => ur.StartDate).ToList<UserReading>();
            List<Book> books = new List<Book>();
            foreach(UserReading ur in userReadings)
            {
                books.Add(db.Books.FirstOrDefault(bk => bk.Id == ur.BookId));
            }
            return books;
        }

        private IEnumerable<LibraryBook> GetBorrowedLibraryBooks()
        {
            string userId = User.Identity.GetUserId();
            //user library
            Library library = db.Libraries.FirstOrDefault(l => l.UserId == userId);
            List<LibraryBook> libraryBooks = db.LibraryBooks.Where(lb => lb.LibraryId == library.Id).ToList<LibraryBook>();
            List<LibraryBook> borrowedLibraryBooks = new List<LibraryBook>();
            foreach(LibraryBook lb in libraryBooks)
            {
                if(db.LibraryLendings.FirstOrDefault(ll=>ll.CopyLibraryBookId==lb.BookId && ll.StartDate!=null && ll.EndDate == null) != null)
                {
                    borrowedLibraryBooks.Add(lb);
                }
            }
            return borrowedLibraryBooks;
        }

        private IEnumerable<string> GetNames(IEnumerable<LibraryBook> libraryBooks, string type)
        {
            List<string> names = new List<string>();
            foreach(LibraryBook lb in libraryBooks)
            {
                LibraryLending lending = null;
                switch (type) {
                    case "borrowed": lending= db.LibraryLendings.FirstOrDefault(ll => ll.CopyLibraryBookId == lb.Id);break;
                    case "lent": lending = db.LibraryLendings.FirstOrDefault(ll => ll.LibraryBookId == lb.Id); break;
                } 
                if(!String.IsNullOrEmpty(lending.ExternalBorrower) || !String.IsNullOrEmpty(lending.ExternalLender))
                {
                    switch (type)
                    {
                        case "borrowed":names.Add(lending.ExternalLender);break;
                        case "lent": names.Add(lending.ExternalBorrower); break;
                    }
                }
                else
                {
                    Library library = null;
                    switch (type)
                    {
                        case "borrowed": library = db.Libraries.FirstOrDefault(x => x.Id == lending.LibraryBookId);break;
                        case "lent": library = db.Libraries.FirstOrDefault(x => x.Id == lending.CopyLibraryBookId); break;
                    }
                    ApplicationUser user = db.Users.FirstOrDefault(usr => usr.Id == library.UserId);
                    names.Add(user.UserName);
                }
                
            }
            return names;
        }

        private IEnumerable<Book> GetBooksDetails(IEnumerable<LibraryBook> libraryBooks)
        {
            string userId = User.Identity.GetUserId();
            List<Book> books = new List<Book>();
            foreach(LibraryBook lb in libraryBooks)
            {
                Book book = db.Books.FirstOrDefault(bk => bk.Id == lb.BookId);
                if (book != null) books.Add(book);
            }
            return books;
        }

        private IEnumerable<LibraryBook> GetLentLibraryBooks()
        {
            string userId = User.Identity.GetUserId();
            //user library
            Library library = db.Libraries.FirstOrDefault(l => l.UserId == userId);
            List<LibraryBook> libraryBooks = db.LibraryBooks.Where(lb => lb.LibraryId == library.Id).ToList<LibraryBook>();
            List<LibraryBook> lentLibraryBooks = new List<LibraryBook>();
            foreach (LibraryBook lb in libraryBooks)
            {
                if (db.LibraryLendings.FirstOrDefault(ll => ll.LibraryBookId == lb.BookId && ll.StartDate != null && ll.EndDate == null) != null)
                {
                    lentLibraryBooks.Add(lb);
                }
            }
            return lentLibraryBooks;
        }
    }
}