using HomeLibraryApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public ActionResult Index(string lib, string page)
        {
            LibraryMain model = new LibraryMain();

            List<Library> libraries = GetUserLibraries().ToList();
            Library library = libraries.FirstOrDefault();

            if (String.IsNullOrEmpty(lib))
            {//your home library
                lib = library.Id.ToString();
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

            List<string> booksStates = new List<string>();
            List<Book> books = new List<Book>();
            List<LibraryBook> libraryBooks = db.LibraryBooks.Where(x => x.LibraryId.ToString() == lib).ToList<LibraryBook>();


            foreach (LibraryBook libraryBook in libraryBooks)
            {
                Book book = db.Books.FirstOrDefault(x => x.Id == libraryBook.BookId);
                if (book == null) continue;
                books.Add(book);
                //determine state
                booksStates.Add(GetBookState(book, lib));
            }
            pagesNr = (int)Math.Ceiling(Convert.ToDouble(books.Count()) / Convert.ToDouble(pageSize));

            books = books.Skip((pageInt - 1) * pageSize).Take(pageSize).ToList();
            booksStates = booksStates.Skip((pageInt - 1) * pageSize).Take(pageSize).ToList();

            model.LibrariesModel = libraries.AsEnumerable<Library>();
            model.LibraryBooksWithStates = new LibraryBooksWithStates();
            model.LibraryBooksWithStates.BooksModel = books.AsEnumerable<Book>();
            model.LibraryBooksWithStates.BooksStates = booksStates.AsEnumerable<string>();
            model.LibraryBooksWithStates.LibId = lib;
            ViewBag.PagesNr = pagesNr;
            return View(model);
        }

        private string GetBookState(Book book, string lib)
        {
            LibraryBook lb = db.LibraryBooks.FirstOrDefault(x => x.BookId == book.Id && x.LibraryId.ToString() == lib);
            LibraryLending llIn = db.LibraryLendings.FirstOrDefault(ll => ll.CopyLibraryBookId == lb.Id && ll.EndDate == null);
            LibraryLending llOut = db.LibraryLendings.FirstOrDefault(ll => ll.LibraryBookId == lb.Id && ll.EndDate == null);
            if (llIn != null) return "in";
            if (llOut != null) return "out";
            return "ok";
        }

        [Authorize]
        public ActionResult Add(LibraryAdd model)
        {
            model.UserLibraries = GetUserLibraries();
            return View(model);
        }

        [Authorize]
        public ActionResult Book(LibraryBookDetails model, string lib, string bk)
        {
            Book book = db.Books.FirstOrDefault(x => x.Id.ToString() == bk);
            LibraryBook libraryBook = db.LibraryBooks.FirstOrDefault(x => x.BookId.ToString() == bk && x.LibraryId.ToString() == lib);

            //comments
            List<LibraryComment> comments = db.LibraryComments.Where(x => x.LibraryBookId == libraryBook.Id).ToList();
            List<LibraryComment> modelComments = new List<LibraryComment>();
            var q = (from lc in comments join us in db.Users on lc.UserId equals us.Id
                     select new {lc=lc.Comment,us});
            foreach(var t in q)
            {
                modelComments.Add(new LibraryComment() { Comment = t.lc, User = t.us });
            }

            //userreading
            var userID = User.Identity.GetUserId();
            UserReading userReading = db.UserReadings.FirstOrDefault(x => x.UserId == userID && x.BookId.ToString()==bk);

            //libraryLending
            LibraryLending libraryLending = db.LibraryLendings.FirstOrDefault(ll => ll.EndDate==null && (ll.LibraryBookId == libraryBook.Id || ll.CopyLibraryBookId==libraryBook.Id));

            ViewBag.Lending = GetBookState(book, lib);
            if (ViewBag.Lending=="out")
            {
                LibraryBook lb = db.LibraryBooks.FirstOrDefault(x => x.Id == libraryLending.CopyLibraryBookId);
                Library library = db.Libraries.FirstOrDefault(l => l.Id == lb.LibraryId);
                ApplicationUser borrowUser = db.Users.FirstOrDefault(usr => usr.Id == library.UserId);
                ViewBag.BorrowingUser = borrowUser;
            }else if (ViewBag.Lending == "in")
            {
                LibraryBook lb = db.LibraryBooks.FirstOrDefault(x => x.Id == libraryLending.LibraryBookId);
                Library library = db.Libraries.FirstOrDefault(l => l.Id == lb.LibraryId);
                ApplicationUser lendUser = db.Users.FirstOrDefault(usr => usr.Id == library.UserId);
                ViewBag.LendingUser = lendUser;
            }

            //Are you an owner?
            Library libraryOwner = db.Libraries.FirstOrDefault(l => l.Id == libraryBook.LibraryId);
            ViewBag.Owner = userID == libraryOwner.UserId ? true : false;

            
            model.Book = book;
            model.Comments = comments;
            model.UserReading = userReading;
            model.LibraryLending = libraryLending;
            return View(model);
        }

        [Authorize]
        public ActionResult Search(LibrarySearch model)
        {
            model.UserLibraries = GetUserLibraries();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(LibraryAdd model, string lib, string type, string bookId)
        {
            Book book = null;
            switch (type)
            {
                case "new": book = model.NewBookModel; break;
                case "goodreads": book = model.GoodreadsBookModel; break;
                case "existing": book = db.Books.FirstOrDefault(bk => bk.Id.ToString() == bookId); break;
            }

            if (!TryValidateModel(book))
            {
                ViewBag.ErrorMsg = "You have to fill in all fields";
                model.UserLibraries = GetUserLibraries();
                return View(model);
            }

            if (!AddBookToLibrary(book, lib))
            {
                ViewBag.ErrorMsg = "The book you are trying to add is already in this library";
                model.UserLibraries = GetUserLibraries();
                return View(model);
            }

            return RedirectToAction("Index", new { lib = lib });
        }

        [Authorize]
        public ActionResult GetSearchedBooks(string searchType, string query, int page, string libraryId, string selectLibrary)
        {
            if (String.IsNullOrEmpty(query))
            {
                return new EmptyResult();
            }

            List<Book> books = new List<Book>();
            Library library = db.Libraries.FirstOrDefault(lb => lb.Id.ToString() == selectLibrary);
            IEnumerable<Book> booksToScan = null;
            if (library == null)
            {
                booksToScan = db.Books;
            }
            else
            {
                List<LibraryBook> libraryBooks = db.LibraryBooks.Where(lb => lb.LibraryId == library.Id).ToList();
                List<Book> bks = new List<Book>();
                foreach (LibraryBook libraryBook in libraryBooks) bks.Add(db.Books.FirstOrDefault(bk=>bk.Id==libraryBook.BookId));
                booksToScan = bks;
            }

            int pagesNr = 1;
            int pageSize = 10;
            query = query.ToLower().Trim();
            switch (searchType)
            {
                case "All": books = booksToScan.Where(book => (book.AuthorFirstname + book.AuthorLastname + book.Title + book.Publisher).ToLower().Contains(query)).ToList(); break;
                case "Title": books = booksToScan.Where(book => book.Title.ToLower().Contains(query)).ToList(); break;
                case "Author": books = booksToScan.Where(book => (book.AuthorFirstname + book.AuthorLastname).ToLower().Contains(query)).ToList(); break;
                case "Publisher": books = booksToScan.Where(book => book.Publisher.ToLower().Contains(query)).ToList(); break;

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

        [Authorize]
        [HttpPost]
        public ActionResult LendBook(string userOrEmail, string lib, string bk)
        {
            LibraryBook libraryBook = db.LibraryBooks.FirstOrDefault(lb => lb.LibraryId.ToString() == lib && lb.BookId.ToString() == bk);
            bool isEmail = new EmailAddressAttribute().IsValid(userOrEmail);

            ApplicationUser user = isEmail ? UserManager.FindByEmail(userOrEmail) : UserManager.FindByName(userOrEmail);
            if (user != null && user.Id!=User.Identity.GetUserId())
            {
                //user library
                Library newUserLibrary = db.Libraries.FirstOrDefault(lb => lb.UserId == user.Id);
                //Copy book to new user library
                LibraryBook copyLibraryBook =new LibraryBook() {BookId=Convert.ToInt32(bk),LibraryId=newUserLibrary.Id };
                db.LibraryBooks.Add(copyLibraryBook);
                db.SaveChanges();
                db.LibraryLendings.Add(new LibraryLending() { LibraryBookId = libraryBook.Id, CopyLibraryBookId=copyLibraryBook.Id, StartDate = DateTime.Now });
                db.SaveChanges();
            }
            return RedirectToAction("Book", new { lib = lib,bk=bk });
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReturnBook(string lib, string bk)
        {
            LibraryBook libraryBook = db.LibraryBooks.First(lb => lb.LibraryId.ToString() == lib && lb.BookId.ToString() == bk);
            LibraryLending libraryLending = db.LibraryLendings.FirstOrDefault(ll => ll.CopyLibraryBookId == libraryBook.Id);
            libraryLending.EndDate = DateTime.Now;
            db.LibraryBooks.Remove(libraryBook);
            db.SaveChanges();
            return RedirectToAction("Index", new { lib = lib});

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

        [Authorize]
        [HttpPost]
        public ActionResult AddComment(LibraryBookDetails model,string lib, string bk)
        {
            LibraryBook libraryBook = db.LibraryBooks.FirstOrDefault(x => x.LibraryId.ToString() == lib && x.BookId.ToString() == bk);
            LibraryComment libraryComment = new LibraryComment() { LibraryBookId = libraryBook.Id, Comment = model.YourComment, Date = DateTime.Now, UserId = User.Identity.GetUserId() };
            db.LibraryComments.Add(libraryComment);
            db.SaveChanges();
            return RedirectToAction("Book",new { lib = lib,bk = bk});
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddReadingState(string state, string lib,string bk)
        {
            string userId = User.Identity.GetUserId();
            UserReading userReading = db.UserReadings.FirstOrDefault(x => x.UserId == userId && x.BookId.ToString() == bk);
            if (userReading == null)
            {
                int bookId = Convert.ToInt32(bk);
                userReading = new UserReading() { BookId = bookId, UserId = userId };
                db.UserReadings.Add(userReading);
            }
            if (state == "0")
            {
                db.UserReadings.Remove(userReading);
            }else if (state == "1")
            {
                userReading.StartDate = DateTime.Now;
                userReading.EndDate = null;
            }else if (state == "2")
            {
                userReading.StartDate = userReading.StartDate == null ? DateTime.Now : userReading.StartDate;
                userReading.EndDate = DateTime.Now;
            }
            db.SaveChanges();
            return RedirectToAction("Book", new { lib = lib, bk = bk });
        }


        private bool AddBookToLibrary(Book book, string id)
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

            Book sameBook = db.Books.FirstOrDefault(bk => bk.Title == book.Title && bk.AuthorFirstname == book.AuthorFirstname && bk.AuthorLastname == book.AuthorLastname && bk.PublicationDate == book.PublicationDate && bk.Publisher == book.Publisher);
            if (sameBook == null)
            {
                db.Books.Add(book);
                db.LibraryBooks.Add(new LibraryBook { Book = book, Library = library });
                db.SaveChanges();
                return true;
            }
            else
            {
                if (db.LibraryBooks.Any(lb => lb.LibraryId == library.Id && lb.BookId == sameBook.Id))
                {
                    return false;
                }
                db.LibraryBooks.Add(new LibraryBook { Book = sameBook, Library = library });
                db.SaveChanges();
                return true;
            }

        }

        private IEnumerable<Library> GetUserLibraries()
        {
            List<Library> libraries = new List<Library>();
            var userID = User.Identity.GetUserId();
            Library library = db.Libraries.First(x => x.UserId == userID);
            libraries.Add(library);
            List<LibraryUser> libraryUsers = db.LibraryUsers.Where(x => x.UserId == userID).ToList<LibraryUser>();
            foreach (LibraryUser libraryUser in libraryUsers) libraries.AddRange(db.Libraries.Where(x => x.Id == libraryUser.LibraryId));
            return libraries;
        }
    }
}