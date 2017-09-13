using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ValidationDAL;
using ValidationSample.Repository;
using System.Linq.Dynamic;
using System.Web.Script.Serialization;
namespace ValidationSample.Controllers
{
    public class MainController : Controller
    {
        ValidationRepository dal = new ValidationRepository();
        
        public ActionResult MainView()
        {
           
            
            if (Session["Id"] == null || Session["IsValid2FA"] == null || !(bool)Session["IsValid2FA"])
            {
                return RedirectToAction("Login", "Home");
            }
            return View();

        }

        public ActionResult GetAllEvents()
        {
            if(Session["id"]==null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View("GetAllEventsKendo");
        }


        public JsonResult GetEvents()
        {
            ValidationMapper<Event, Models.Event> mapObj = new ValidationMapper<Event, Models.Event>();

            var lstEvent = dal.GetEvents();
            List<Models.Event> lstModelEvent = new List<Models.Event>();
            if (lstEvent.Any())
            {
                foreach (var event1 in lstEvent)
                {
                    lstModelEvent.Add(mapObj.Translate(event1));
                }
            }
            foreach(var event1 in lstModelEvent)
            {
                event1.RecurrencePattern = event1.Recurrence + ";" + event1.RecurrenceInterval + ";" + event1.RecurrenceCount;
            }
            if (Session["Id"] != null)
            {
                string name = Session["Id"].ToString();
                ViewBag.name = name;
                
                return new JsonResult { Data = lstModelEvent , JsonRequestBehavior= JsonRequestBehavior.AllowGet };
                
            }
            else
            {
                return Json("Login");
            }
        }
        public JsonResult GetTestcaseLogs()
        {
            ValidationMapper<TestCaseLog, Models.TestCaseLog> mapObj = new ValidationMapper<TestCaseLog, Models.TestCaseLog>();

            var lstTestcaseLogs = dal.GetAllTestCaseLogs();
            List<Models.TestCaseLog> lstModelTestcaseLogs = new List<Models.TestCaseLog>();
            if (lstTestcaseLogs.Any())
            {
                foreach (var log1 in lstTestcaseLogs)
                {
                    lstModelTestcaseLogs.Add(mapObj.Translate(log1));
                }
            }
            
            if (Session["Id"] != null)
            {
                string name = Session["Id"].ToString();
                ViewBag.name = name;

                var jsonResult= new JsonResult { Data = lstModelTestcaseLogs, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else
            {
                return Json("Login");
            }
        }
        
        public JsonResult GetTestcaseLogs2(int pageNumber, int pageSize)
        {
            ValidationMapper<TestCaseLog, Models.TestCaseLog> mapObj = new ValidationMapper<TestCaseLog, Models.TestCaseLog>();

            var lstTestcaseLogs = dal.GetTestcaseLogs(pageNumber,pageSize);
            List<Models.TestCaseLog> lstModelTestcaseLogs = new List<Models.TestCaseLog>();
            if (lstTestcaseLogs.Any())
            {
                foreach (var log1 in lstTestcaseLogs)
                {
                    lstModelTestcaseLogs.Add(mapObj.Translate(log1));
                }
            }
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //js.Serialize(lstModelTestcaseLogs);
            var jsonResult = new JsonResult { Data = lstModelTestcaseLogs, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        [HttpGet]
        public JsonResult GetTestcaseLogsReact(string sortColumnName = "TestCaseLogId", string sortOrder = "asc", int pageSize = 50, int currentPage = 1)
        {
            ValidationMapper<TestCaseLog, Models.TestCaseLog> mapObj = new ValidationMapper<TestCaseLog, Models.TestCaseLog>();
            int totalPage = 0;
            int totalRecord = 0;
            var lstTestcaseLogs = dal.GetAllTestCaseLogs();
            List<Models.TestCaseLog> lstModelTestcaseLogs = new List<Models.TestCaseLog>();
            List<Models.TestCaseLog> List = new List<Models.TestCaseLog>();
            if (lstTestcaseLogs.Any())
            {
                foreach (var log1 in lstTestcaseLogs)
                {
                    lstModelTestcaseLogs.Add(mapObj.Translate(log1));
                }
            }
            totalRecord = lstModelTestcaseLogs.Count();
            if (pageSize > 0)
            {
                totalPage = totalRecord / pageSize + ((totalRecord % pageSize) > 0 ? 1 : 0);
                List = lstModelTestcaseLogs.OrderBy(sortColumnName + " " + sortOrder).Skip(pageSize * (currentPage - 1)).Take(pageSize).ToList();
            }
            else
            {
                List = lstModelTestcaseLogs.ToList();
            }
            
                

                var jsonResult = new JsonResult { Data = new { List = List, totalPage = totalPage, sortColumnName = sortColumnName, sortOrder = sortOrder, currentPage = currentPage },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;         
            
        }

        public JsonResult GetTestcaseLogData(int page)
        {
            ValidationMapper<TestCaseLog, Models.TestCaseLog> mapObj = new ValidationMapper<TestCaseLog, Models.TestCaseLog>();
            List<Models.TestCaseLog> lstModelTestcaseLogs = new List<Models.TestCaseLog>();
            List<Models.TestCaseLog> lstModelTestcaseLogs2 = new List<Models.TestCaseLog>();
            int totalPage = 0;
            int totalRecord = 0;
            int pageSize = 50;
            var lstTestcaseLogs = dal.GetAllTestCaseLogs().OrderBy(a=>a.TestCaseLogId).ToList();
            totalRecord = lstTestcaseLogs.Count;
            totalPage = (totalRecord / pageSize) + ((totalRecord % pageSize) > 0 ? 1 : 0);
            if (lstTestcaseLogs.Any())
            {
                foreach (var log1 in lstTestcaseLogs)
                {
                    lstModelTestcaseLogs.Add(mapObj.Translate(log1));
                }
            }
            lstModelTestcaseLogs2=lstModelTestcaseLogs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new JsonResult { Data = new { List = lstModelTestcaseLogs2, currentPage = page, totalPage = totalPage }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult GetAllTestcaseLogs()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View("GetAllTestcaseLogsReact");
        }
        [HttpPost]
        public JsonResult SaveEvent(Models.Event e1)
        {
            bool status = false;
            try
            {
                //if (e1.Recurrence == null)
                //{
                //    e1.Recurrence = "Single Day";
                //}
                ValidationMapper<Models.Event, Event> mapObj = new ValidationMapper<Models.Event, Event>();
               
                
                var event1 = dal.GetEvents().Where(a => a.EventId == e1.EventId).FirstOrDefault();
                if(event1!=null)
                {
                                               
                    event1 = mapObj.Translate(e1);
                    status = dal.EditEvent(event1);
                }
                
                
            }
            catch(Exception e)
            {
                status = false;
            }

            return new JsonResult { Data = new { status = status, JsonRequestBehavior = JsonRequestBehavior.AllowGet } };
        }
        public JsonResult AddEvent(Models.Event e1)
        {
            bool status = false;
            try
            {
                //if (e1.Recurrence == null)
                //{
                //    e1.Recurrence = "Single Day";
                //}
                ValidationMapper<Models.Event, Event> mapObj = new ValidationMapper<Models.Event, Event>();
                var event1 = mapObj.Translate(e1);
                status = dal.CreateEvent(event1);




            }
            catch (Exception e)
            {
                status = false;
            }

            return new JsonResult { Data = new { status = status, JsonRequestBehavior = JsonRequestBehavior.AllowGet } };
        }
        [HttpPost]
        public JsonResult DeleteEvent(int eventId)
        {
            bool status = false;
            try
            {
                var event1 = dal.GetEvents().Where(a => a.EventId == eventId).FirstOrDefault();
                if(event1!=null)
                {
                    status = dal.DeleteEvent(event1);
                }
            }
            catch(Exception e)
            {
                status = false;
            }

            return new JsonResult { Data = new { status = status, JsonRequestBehavior = JsonRequestBehavior.AllowGet } };
        }
       
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetNotificationEvents()
        {
            var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
            NotificationComponent NC = new NotificationComponent();
            var list = NC.GetEvents(notificationRegisterTime);
            //update session here for get only new added contacts (notification)
            Session["LastUpdate"] = DateTime.Now;
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}