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
                home.LastBook = GetLastBook();
                home.CurrentBooks = GetCurrentBooks();
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
            double noOfBooks = db.UserReadings.Where(ur => ur.UserId == userId && ur.StartDate != null && ur.EndDate != null).Count();
            double noOfMonths = Math.Ceiling((DateTime.Now - user.Created).TotalDays / 30.0);
            return noOfBooks / noOfMonths;
        }

        private Book GetLastBook()
        {
            string userId = User.Identity.GetUserId();
            UserReading userReading = db.UserReadings.Where(ur => ur.UserId == userId && ur.StartDate != null && ur.EndDate != null)
                .OrderByDescending(ur => ur.EndDate).FirstOrDefault();
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
    }
}