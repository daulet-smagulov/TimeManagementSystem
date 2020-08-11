using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TimeManagementSystem.Models;
using TimeManagementSystem.Utils;

namespace TimeManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : UserManagerController
    {
        private readonly string[] _rolesSortedDesc = new[] { "Administrator", "Manager", "User" };

        // GET: Home
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            var currentUser = UserManager.FindById(id);

            var homeViewModel = new HomeViewModel
            {
                CurrentUser = currentUser,
                HasSubUsers = !UserManager.IsInRole(id, "User")
            };
            return View(homeViewModel);
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page";

            return View();
        }

        // GET: /home/GetUserById
        [HttpGet]
        public JsonResult GetUserById(string userId)
        {
            var user = UserManager.FindById(userId);
            return new JsonResult 
            { 
                Data = user,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: /home/GetSubUsers
        [HttpGet]
        [Authorize(Roles = "Administrator,Manager")]
        public JsonResult GetSubUsers(string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                ApplicationUser user = UserManager.FindById(userId);
                List<ApplicationUser> users = new List<ApplicationUser>();
                for (int i = 0; i < _rolesSortedDesc.Length - 1; i++)
                {
                    if (UserManager.IsInRole(userId, _rolesSortedDesc[i]))
                    {
                        var lowerRoles = _rolesSortedDesc.Skip(i + 1).ToArray();
                        foreach (var role in dbContext.Roles.ToArray())
                            if (lowerRoles.Contains(role.Name))
                                foreach (var userRole in role.Users)
                                    users.Add(UserManager.FindById(userRole.UserId));
                    }
                }
                users.Add(user);
                return new JsonResult
                {
                    Data = users,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        // GET: /home/GetEvents
        [HttpGet]
        public JsonResult GetEvents(string id)
        {
            IQueryable<Event> events = null;
            using (var dbContext = new ApplicationDbContext())
            {
                events = dbContext.Events.Where(ev => ev.UserId == id);
                return new JsonResult
                {
                    Data = events.ToList(),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        // POST: home/SaveEvent
        [HttpPost]
        public JsonResult SaveEvent(Event ev)
        {
            var saved = false;
            using (var dbContext = new ApplicationDbContext())
            {
                if (ev.EventId > 0)
                {
                    //Update the event
                    var existedEv = dbContext.Events.FirstOrDefault(a => a.EventId == ev.EventId);
                    if (existedEv != null)
                    {
                        existedEv.UserId = ev.UserId;
                        existedEv.Subject = ev.Subject;
                        existedEv.Start = ev.Start;
                        existedEv.End = ev.End;
                        existedEv.Description = ev.Description;
                        existedEv.IsFullDay = ev.IsFullDay;
                        existedEv.ThemeColor = ev.ThemeColor;
                        saved = true;
                    }
                }
                else
                {
                    dbContext.Events.Add(ev);
                    saved = true;
                }
                dbContext.SaveChanges();
            }
            return new JsonResult { Data = new { status = saved } };
        }

        // POST: /home/DeleteEvent
        [HttpPost]
        public JsonResult DeleteEvent(int id)
        {
            var deleted = false;
            using (var dbContext = new ApplicationDbContext())
            {
                var existedEv = dbContext.Events.FirstOrDefault(ev => ev.EventId == id);
                if (existedEv != null)
                {
                    dbContext.Events.Remove(existedEv);
                    dbContext.SaveChanges();
                    deleted = true;
                }
            }
            return new JsonResult { Data = new { status = deleted } };
        }

        // POST: /home/SetWorkingHours
        [HttpPost]
        public JsonResult SetWorkingHours(byte hours)
        {
            var set = false;
            using (var dbContext = new ApplicationDbContext())
            {
                string userId = User.Identity.GetUserId();
                dbContext.Users.FirstOrDefault(user => user.Id == userId).PreferredHoursPerDay = hours;
                dbContext.SaveChanges();
                set = true;
            }
            return new JsonResult { Data = new { status = set } };
        }

        // GET: /home/CompareDurations
        [HttpGet]
        public JsonResult CompareDurations(string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                byte preferredHoursPerDay = UserManager.FindById(userId).PreferredHoursPerDay;
                IQueryable<Event> events = dbContext.Events.Where(ev => ev.UserId == userId);
                SortedDictionary<string, DayNotes> notes = DayNotes.GetNotes(events);
                var output = notes.ToDictionary(item => item.Key, 
                    item => item.Value.TotalHours >= preferredHoursPerDay);
                return new JsonResult
                {
                    Data = output,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        // POST: /home/ExportToHtml
        [HttpGet]
        public JsonResult ExportToHtml(int[] selectedIds, string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                IQueryable<Event> events = dbContext.Events.Where(ev => selectedIds.Contains(ev.EventId));
                SortedDictionary<string, DayNotes> notes = DayNotes.GetNotes(events);
                var htmlString = EventExporter.GetHtml(notes);
                //byte[] content = Encoding.ASCII.GetBytes(htmlString);
                //string userName = UserManager.FindById(userId).FullName;
                //return File(content, "text/html", userName + ".html");
                return new JsonResult
                {
                    Data = htmlString,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        //// GET: /home/ExportToHtml
        //[HttpGet]
        //public JsonResult ExportToHtml(int[] selectedIds, string userId)
        //{
        //    using (var dbContext = new ApplicationDbContext())
        //    {
        //        IQueryable<Event> events = dbContext.Events.Where(ev => selectedIds.Contains(ev.EventId));
        //        var iii = events.Count();
        //        Dictionary<string, DayNotes> notes = DayNotes.GetNotes(events);
        //        var htmlString = EventExporter.GetHtml(notes);
        //        //byte[] content = Encoding.ASCII.GetBytes(htmlString);
        //        //string userName = UserManager.FindById(userId).FullName;
        //        return new JsonResult
        //        {
        //            Data = htmlString,
        //            JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //        };
        //    }
        //}
    }
}
