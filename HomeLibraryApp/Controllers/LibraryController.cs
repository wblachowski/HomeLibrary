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
        public ActionResult Index()
        {
            List<Library> libraries = new List<Library>();
            var userID = User.Identity.GetUserId();
            Library library = db.Libraries.First(x => x.UserId == userID);
            libraries.Add(library);
            List<LibraryUser> libraryUsers = db.LibraryUsers.Where(x => x.UserId == userID).ToList<LibraryUser>();
            foreach (LibraryUser libraryUser in libraryUsers) libraries.AddRange(db.Libraries.Where(x => x.Id == libraryUser.LibraryId));

            return View(libraries.AsEnumerable<Library>());
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

        // GET: /Library/ConfirmInvitation
        [Authorize]
        public async Task<ActionResult> ConfirmInvitation(string userId, string code)
        {
            var tokenCorrect = await UserManager.VerifyUserTokenAsync(userId, "ConfirmInvitation", code);
            if (tokenCorrect)
            {
                Library library = db.Libraries.First(x => x.UserId == userId);
                string callingUserId = User.Identity.GetUserId();
                if (User.Identity.GetUserId() != userId && db.LibraryUsers.First(x=>x.UserId== callingUserId && x.LibraryId==library.Id)==null)
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