using HomeLibraryApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HomeLibraryApp.Controllers
{
    public class LibraryController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public LibraryController()
        {

        }

        public LibraryController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Library
        [Authorize]
        public ActionResult Index(string id, string page)
        {
            LibraryMain model = new LibraryMain();

            List<Library> libraries = new List<Library>();
            var userID = User.Identity.GetUserId();
            Library library = db.Libraries.First(x => x.UserId == userID);
            libraries.Add(library);
            List<LibraryUser> libraryUsers = db.LibraryUsers.Where(x => x.UserId == userID).ToList<LibraryUser>();
            foreach (LibraryUser libraryUser in libraryUsers) libraries.AddRange(db.Libraries.Where(x => x.Id == libraryUser.LibraryId));

            if (String.IsNullOrEmpty(id))
            {//your home library
                id = library.Id.ToString();
            }
            int pageInt;
            int pageSize = 10;
            int pagesNr = 1;
            if (String.IsNullOrEmpty(page))
            {
                pageInt = 1;
            }
            else
            {
                pageInt = Int32.Parse(page);
            }

            List<Book> books = new List<Book>();
            List<LibraryBook> libraryBooks = db.LibraryBooks.Where(x => x.LibraryId.ToString() == id).ToList<LibraryBook>();
            foreach (LibraryBook libraryBook in libraryBooks) books.AddRange(db.Books.Where(x => x.Id == libraryBook.BookId));
            pagesNr = (int)Math.Ceiling(Convert.ToDouble(books.Count()) / Convert.ToDouble(pageSize));
            books = books.OrderBy(book => book.Title).Skip((pageInt - 1) * pageSize).Take(pageSize).ToList();

            model.LibrariesModel = libraries.AsEnumerable<Library>();
            model.BooksModel = books.AsEnumerable<Book>();

            ViewBag.PagesNr = pagesNr;
            return View(model);
        }

        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

        [Authorize]
        public ActionResult GetBooks(string id)
        {
            if (String.IsNullOrEmpty(id))
            {//your home library
                var userID = User.Identity.GetUserId();
                Library library = db.Libraries.First(x => x.UserId == userID);
                id = library.Id.ToString();
            }
            List<Book> books = new List<Book>();
            List<LibraryBook> libraryBooks = db.LibraryBooks.Where(x => x.LibraryId.ToString() == id).ToList<LibraryBook>();
            foreach (LibraryBook libraryBook in libraryBooks) books.AddRange(db.Books.Where(x => x.Id == libraryBook.BookId));
            return PartialView("_BooksPartial", books);
        }

        [Authorize]
        public ActionResult GetSearchedBooks(string searchType, string query, int page, string libraryId)
        {
            if (String.IsNullOrEmpty(query))
            {
                return new EmptyResult();
            }

            List<Book> books = new List<Book>();
            int pagesNr=1;
            int pageSize = 10;
            switch (searchType)
            {
                case "All": books= db.Books.Where(book => (book.AuthorFirstname + book.AuthorLastname + book.Title + book.Publisher).Contains(query)).ToList();break;
                case "Title": books = db.Books.Where(book => book.Title.Contains(query)).ToList();break;
                case "Author": books = db.Books.Where(book => (book.AuthorFirstname + book.AuthorLastname).Contains(query)).ToList();break;
                case "Publisher": books = db.Books.Where(book => book.Publisher.Contains(query)).ToList(); break;

            }

            pagesNr = (int)Math.Ceiling(Convert.ToDouble(books.Count()) / Convert.ToDouble(pageSize));
            books = books.OrderBy(book => book.Title).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PagesNr = pagesNr;
            ViewBag.CurrentPage = page;
            ViewBag.LibraryId = libraryId;
            return PartialView("_BooksSearchPartial", books);
        }

        [HttpPost]
        public async Task<bool> Invite(string email)
        {
            string sender = User.Identity.GetUserName();
            string senderId = User.Identity.GetUserId();
            string code = await UserManager.GenerateUserTokenAsync("ConfirmInvitation", senderId);
            var callbackUrl = Url.Action("ConfirmInvitation", "Library",
             new { userId = senderId, code = code }, protocol: Request.Url.Scheme);
            try
            {
                EmailService service = new EmailService();
                var message = new IdentityMessage
                {
                    Destination = email,
                    Subject = "New library invitation",
                    Body = "You have been invited to " + sender + "'s library! To confirm the invitation click <a href=\"" + callbackUrl + "\">here</a>"
                };
                await service.SendAsync(message);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddNewBook(LibraryAdd model, string id)
        {/*
            if (!ModelState.IsValid)
            {
                return false;
            }*/

            AddBookToLibrary(model.NewBookModel, id);
            return RedirectToAction("Index", new { id = id });
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddGoodreadsBook(LibraryAdd model, string lel,string id)
        {/*
            if (!ModelState.IsValid)
            {
                return false;
            }*/
            AddBookToLibrary(model.GoodreadsBookModel, id);
            return RedirectToAction("Index", new { id = id });
        }

        private void AddBookToLibrary(Book book, string id)
        {
            Library library;
            if (id == null)  //your home library
            {
                var userId = User.Identity.GetUserId();
                library = db.Libraries.First(x => x.UserId == userId);
            }
            else
            {
                library = db.Libraries.First(x => x.Id.ToString() == id.ToString());
            }
            db.Books.Add(book);
            db.LibraryBooks.Add(new LibraryBook { Book = book, Library = library });
            db.SaveChanges();
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddExistingBook(string id, string bookId)
        {
            Library library;
            if (id == null)  //your home library
            {
                var userId = User.Identity.GetUserId();
                library = db.Libraries.First(x => x.UserId == userId);
            }
            else
            {
                library = db.Libraries.First(x => x.Id.ToString() == id.ToString());
            }
            Book book = db.Books.First(x => x.Id.ToString() == bookId);
            db.LibraryBooks.Add(new LibraryBook { Book = book, Library = library });
            db.SaveChanges();
            return RedirectToAction("Index", new { id = id });
        }

        // GET: /Library/ConfirmInvitation
        [Authorize]
        public async Task<ActionResult> ConfirmInvitation(string userId, string code)
        {
            var tokenCorrect = await UserManager.VerifyUserTokenAsync(userId, "ConfirmInvitation", code);
            if (tokenCorrect)
            {
                Library library = db.Libraries.First(x => x.UserId == userId);
                string callingUserId = User.Identity.GetUserId();
                if (User.Identity.GetUserId() != userId && db.LibraryUsers.FirstOrDefault(x => x.UserId == callingUserId && x.LibraryId == library.Id) == null)
                {
                    db.LibraryUsers.Add(new LibraryUser { UserId = callingUserId, LibraryId = library.Id });
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}