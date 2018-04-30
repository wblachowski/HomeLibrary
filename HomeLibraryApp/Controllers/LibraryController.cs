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
            var userID = User.Identity.GetUserId();
            Library library = db.Libraries.First(x => x.UserId == userID);
            return View(library);
        }

        [HttpPost]
        public async Task<bool> Invite(string email)
        {
            try
            {
                EmailService service = new EmailService();
                var message = new IdentityMessage
                {
                    Destination = email,
                    Subject = "Invitation test",
                    Body = "This is even more advanced invitation test!"
                };
                await service.SendAsync(message);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}