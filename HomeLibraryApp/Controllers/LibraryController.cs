﻿using HomeLibraryApp.Models;
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
            string sender = User.Identity.GetUserName();
            string senderId = User.Identity.GetUserId();
            string code =await  UserManager.GenerateUserTokenAsync("ConfirmInvitation", senderId);
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
                
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}