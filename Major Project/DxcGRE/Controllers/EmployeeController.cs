using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DxcGRE.Models;
using System.Web.Security;

namespace DxcGRE.Controllers
{
    public class EmployeeController : Controller
    {
        Major_ProjectEntities context = new Major_ProjectEntities();
        static int ID;
        // GET: Employee
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(employee login)
        {
            if (ModelState.IsValid)
            {
                bool isValid = context.employees.Any(x => x.Email == login.Email && x.Password == login.Password);
                if (isValid)
                {
                    employee data = context.employees.FirstOrDefault(x => x.Email == login.Email);
                    ID = (int)data.EmpID;
                    FormsAuthentication.SetAuthCookie(data.EmpName, false);
                    return RedirectToAction("EmployeeDashboard");
                }
                ViewBag.Error = "Invalid User ID or Password";
                return View();
            }
            return View();
        }
        [Authorize]
        public ActionResult EmployeeDashboard()
        {
            return View();
        }

        public ActionResult TrainingRooms()
        {
            ViewBag.Rooms = Rooms();
            return View();
        }

        [HttpPost]
        public ActionResult TrainingRooms(dashboard data, FormCollection form)
        {
            ViewBag.Rooms = Rooms();
            data.EmpID = ID;
            data.BookingStatus = "Pending";
            data.BookingType = "Training Room";
            int daysdifference = ((TimeSpan)(data.DateTo - data.DateFrom)).Days;
            if (data.DateTo >= data.DateFrom && daysdifference <= 93)
            {
                bool svalid = context.dashboards.Any(x => x.RoomName == data.RoomName);
                if (svalid)
                {
                    if (data.DateFrom == data.DateTo)
                    {
                        bool evalid = context.dashboards.Any(x => x.DateFrom == data.DateFrom);
                        if (!evalid)
                        {

                            context.dashboards.Add(data);
                            context.SaveChanges();
                            return RedirectToAction("EmployeeDashboard");
                        }
                    }

                    bool zcalid = context.dashboards.Any(x => x.DateFrom == data.DateFrom || x.DateFrom == data.DateTo || x.DateTo == data.DateFrom || x.DateTo == data.DateTo);
                    if (zcalid)
                    {
                        ModelState.AddModelError("RoomName", " training room  " + data.RoomName + " has already been booked on " + data.DateFrom + " or " + data.DateTo);
                    }
                    else
                    {
                        bool fvalid = context.dashboards.Any(x => x.DateFrom < data.DateFrom && x.DateTo > data.DateFrom || x.DateFrom < data.DateTo && x.DateTo > data.DateTo);
                        bool dvalid = context.dashboards.Any(x => x.DateFrom > data.DateFrom && x.DateFrom < data.DateTo && x.DateTo > data.DateFrom && x.DateTo < data.DateTo);
                        if (dvalid)
                        {
                            ModelState.AddModelError("RoomName", "Training Room "+data.RoomName+"already been booked in between the given dates");
                            return View();

                        }
                        else if (fvalid)
                        {
                            ModelState.AddModelError("RoomName", "Training Room " + data.RoomName + "already been booked in between the given dates");
                        }
                        else
                        {
                            context.dashboards.Add(data);
                            context.SaveChanges();
                            return RedirectToAction("EmployeeDashboard");
                        }
                    }
                }
                else
                {
                    context.dashboards.Add(data);
                    context.SaveChanges();
                    return RedirectToAction("EmployeeDashboard");
                }
            }
            else
            {
                ModelState.AddModelError("RoomName", "*Invalid Dates OR Booking Period Must Be Less Than 3 Months");
            }
            return View();


        }

        public ActionResult CleaningServices()
        {
            ViewBag.Rooms = Rooms();
            ViewBag.ServiceType = ServiceType();
            return View();
        }

        [HttpPost]
        public ActionResult CleaningServices(dashboard data, FormCollection form)
        {
            ViewBag.Rooms = Rooms();
            ViewBag.ServiceType = ServiceType();
            dashboard dbdata = new dashboard();
            dbdata.EmpID = ID;
            dbdata.RoomName = form["RoomName"].ToString();
            dbdata.BookingType = form["BookingType"].ToString();
            dbdata.BookingStatus = "Pending";
            dbdata.DateTo = data.DateFrom;
            dbdata.DateFrom = data.DateFrom;
            context.dashboards.Add(dbdata);
            context.SaveChanges();
            return RedirectToAction("EmployeeDashboard");
        }

        public ActionResult ServicesBooked()
        {

            var BookedServices = context.dashboards.Where(x => x.EmpID == ID);
            return View(BookedServices);
        }

        public ActionResult Feedback()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Feedback(feedback fback, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                fback.Empid = ID;
                fback.Usability = form[0];
                fback.Service = form[1];
                fback.User_Experience = form[2];
               
                context.feedbacks.Add(fback);
                context.SaveChanges();
                return RedirectToAction("FeedbackResponse");
               
            }
            return View();
        }

        public ActionResult FeedbackResponse()
        {
            return View();
        }

        public ActionResult RemoveBooking(int id)
        {
            dashboard data = context.dashboards.Find(id);
            context.dashboards.Remove(data);
            context.SaveChanges();
            return RedirectToAction("ServicesBooked");
        }

        public ActionResult Availabilty()
        {
            ViewBag.Rooms = Rooms();
            return View(); 
        }

        public ActionResult _AvailabilityPartial(string room)
        {
            var availabledates = context.dashboards.Where(x => x.RoomName == room);
            ViewBag.availabledates = availabledates.ToList();
            return PartialView("_AvailabilityPartial");
        }

        public List<string> Rooms()
        {
            List<string> roomnames = new List<string>() { "Purdue", "Columbia", "Oxford", "Helsinki", "Kyoto", "Yale", "Carneige mellon", "Jhon Hopkins", "Cambridge", "Berkely" };
            return roomnames;
        }
        public List<string> ServiceType()
        {
            List<string> Servicetype = new List<string>() { "Furniture Cleaning", "Carpet Cleaning", "Glass Cleaning", "Deep Cleaning", "Room Freshener Service" };
            return Servicetype;
        }
    }
}