using HomeLibraryApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace HomeLibraryApp
{
    public class AccountCleanUp
    {
        public static void StartThread()
        {
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 60 * 60 * 1000;    //1 hour
            timer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            foreach (ApplicationUser user in context.Users)
            {
                if (!user.EmailConfirmed && (DateTime.Now - user.Created).TotalHours > 24.0)
                {
                    context.Users.Remove(user);
                }
            }
            context.SaveChanges();
        }
    }
}