using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.Mvc;
using DxcGRE.Models;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace DxcGRE.Controllers
{
    public class ManagerController : Controller
    {
        Major_ProjectEntities context = new Major_ProjectEntities();
        // GET: Manager
        
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(gre_manager login)
        {
            if (ModelState.IsValid)
            {
                bool isValid = context.gre_manager.Any(x => x.Email == login.Email && x.Password == login.Password);
                
                if (isValid)
                {
                    gre_manager data = context.gre_manager.FirstOrDefault(x=>x.Email==login.Email);
                    FormsAuthentication.SetAuthCookie(data.ManagerName, false);
                    return RedirectToAction("Dashboard");
                }
                ViewBag.Error = "Invalid User ID or Password";
                return View();
            }
            return View();
        }
        [Authorize]
        public ActionResult Dashboard()
        {
            var BookedServices = context.dashboards.ToList();
            BookedServices.Reverse();
            return View(BookedServices);
        }

        [Authorize]
        public ActionResult Feedback()
        {
            var Feedbacks = context.feedbacks.ToList();
            Feedbacks.Reverse();
            return View(Feedbacks);
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            return Redirect("~/Home/Index");
        }

        public ActionResult Approve(int id)
        {
            dashboard data = context.dashboards.Find(id);
            data.BookingStatus = "Approved";
            int ID = (int)data.EmpID;
            var employee = context.employees.FirstOrDefault(x=>x.EmpID==ID);
            var Email = employee.Email;
            MailMessage mailcontext = new MailMessage("dxcgre@gmail.com", Email);
            mailcontext.Subject = "Booking Status";
            mailcontext.Body = "Your Booking has benn confirmed";
            mailcontext.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("dxcgre@gmail.com", "DXCGREADMIN");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mailcontext);
            context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public ActionResult Reject(int id)
        {
            dashboard data = context.dashboards.Find(id);
            data.BookingStatus = "Declined";
            int ID = (int)data.EmpID;
            var employee = context.employees.FirstOrDefault(x => x.EmpID == ID);
            var Email = employee.Email;
            MailMessage mailcontext = new MailMessage("dxcgre@gmail.com", Email);
            mailcontext.Subject = "Booking Status";
            mailcontext.Body = "Sorry, Your Booking has benn declined!";
            mailcontext.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("dxcgre@gmail.com", "DXCGREADMIN");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mailcontext);
            context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}