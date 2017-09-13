using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationDAL;
using ValidationSample.Repository;
using System.Linq.Dynamic;
using Google.Authenticator;

namespace ValidationSample.Controllers
{
    public class HomeController : Controller
    {
        ValidationRepository dal = new ValidationRepository();


        private const string key = "qaz123!@@)(*";

        // GET: Home

        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Login()
        {
            if(Session["Id"]!=null && Session["IsValid2FA"] != null && Convert.ToBoolean(Session["IsValid2FA"]))
            {
                //return RedirectToAction("GetAllTestCases");
                return Redirect("/Main/MainView");
            }
                      
            return View("Login");
            
        }
        [HttpPost]
        public ActionResult Login(Models.Admin admin)
        {
            Session["Id"] = null;
            string message = "";
            bool status = false;
            try
            {                
                string userId = admin.UserId;
                string password = admin.Password;
                var userName = dal.CheckUser(userId, password);
                if(userName!="")
                {
                    status = true;
                    message = "2FA Verification";
                    Session["Id"] = userName;
                    TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                    string userUniqueKey = userId + key;
                    Session["userUniqueKey"] = userUniqueKey;
                    var setupInfo = tfa.GenerateSetupCode("Validation", userId, userUniqueKey, 300, 300);
                    Session["BarcodeImageUrl"] = setupInfo.QrCodeSetupImageUrl;
                    Session["SetupCode"] = setupInfo.ManualEntryKey;
                    //return Redirect("/Home/GetAllTestCases");
                    Session["Message1"] = message;
                    Session["Status"] = status;
                    return View();
                }
                else
                {
                    message= "Invalid Credentials";
                    ViewBag.message = "Invalid Credentials, Please try again";
                    Session["Message1"] = message;
                    Session["Status"] = status;
                    return View("Login");
                }
               

            }
            catch
            {
                ViewBag.message = "Invalid Credentials, Please try again later";
                return View("Login");
            }
        }

        public ActionResult MyProfile()
        {
            if (Session["Id"] == null || Session["IsValid2FA"] == null || !(bool)Session["IsValid2FA"])
            {
                return RedirectToAction("Login");
            }
            Session["Message1"] = "Welcome " + Session["Id"].ToString();
            return View();
        }
        public ActionResult Verify2FA()
        {
            var token = Request["passcode"];
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string userUniqueKey = Session["userUniqueKey"].ToString();
            if (token.Trim() != "")
            {
                bool isValid = tfa.ValidateTwoFactorPIN(userUniqueKey, token);
                if (isValid)
                {
                    Session["IsValid2FA"] = true;
                    return RedirectToAction("MainView", "Main");
                }
                else
                {
                    Session["Message1"] = "";
                    Session["Status"] = false;
                    Session["Id"] = null;
                    TempData["Error"] = "Enter correct OTP";
                }
            }
            else
            {
                Session["Message1"] = "";
                Session["Status"] = false;
                Session["Id"] = null;
                TempData["Error"] = "Enter OTP";
                
            }
            
            return View("Login");
        }


        public ActionResult AddUser()
        {
            if (Session["Id"] != null)
            {
                return RedirectToAction("GetAllTestCases");
            }

            return View("AddUser");
        }
        [HttpPost]
        public ActionResult AddUser(FormCollection form)
        {
            try
            {
                string userName = form["name"];
                string password = form["pwd"];
                if(userName!="" && password!="")
                {
                    var userId = dal.AddUser(userName, password);
                    if (userId != "")
                    {
                        TempData["Id"] = userId;
                    }
                }
                else
                {
                    TempData["errmsg"] = "Please enter both username and password";
                    return View("AddUser");
                }
                
            }
            catch
            {
                TempData["errmsg"] = "Some Error Occured, Please Create account Again";
                return View("AddUser");
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult GetAllTestCases()
        {
            ValidationMapper<TestCase,Models.TestCase> mapObj= new ValidationMapper<TestCase, Models.TestCase>();
            
            var lstTestCase = dal.GetAllTestCases();
            List<Models.TestCase> lstModelTestCase = new List<Models.TestCase>();
            if (lstTestCase.Any())
            {
                foreach(var testCase in lstTestCase)
                {
                    lstModelTestCase.Add(mapObj.Translate(testCase));
                }
            }
            if(Session["Id"]!=null)
            {
                string name = Session["Id"].ToString();
                ViewBag.name = name;
                //return PartialView("_GetAllTestCasesPartialView", lstModelTestCase);
                return Json(new { data=lstModelTestCase }, JsonRequestBehavior.AllowGet);
                //return View(lstModelTestCase);
            }
            else
            {
                return Json("Login");
            }
            
        }
        [HttpGet]
        public JsonResult GetAllTestCaseNames(int testSuiteId=0,int pplStageId=0,int tstTypeId=0)
        {
            //ValidationMapper<TestCase, Models.TestCase> mapObj = new ValidationMapper<TestCase, Models.TestCase>();

            var lstTestCase = dal.GetAllTestCases();
            if(testSuiteId!=0)
            {
                lstTestCase = lstTestCase.Where(a=>(a.TestSuiteId==testSuiteId)).ToList();
            }
            if (pplStageId != 0)
            {
                lstTestCase = lstTestCase.Where(a => (a.PipelineStageId == pplStageId)).ToList();
            }
            if (tstTypeId != 0)
            {
                lstTestCase = lstTestCase.Where(a => (a.TestCaseTypeId == tstTypeId)).ToList();
            }
            var lstTestCaseNames = (from l in lstTestCase select l.TestCaseName).ToList();
            
            if (Session["Id"] != null)
            {
                
                return new JsonResult {  Data = lstTestCaseNames , JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                //return View(lstModelTestCase);
            }
            else
            {
                return Json("Login");
            }

        }
        public ActionResult Failure()
        {
            return View("Failure");
        }
        [HttpGet]
        public ActionResult GetAllTestCaseLogs(int testCaseId)
        {
            ValidationMapper<TestCaseLog, Models.TestCaseLog> mapObj = new ValidationMapper<TestCaseLog, Models.TestCaseLog>();

            var lstTestCaseLog = dal.GetAllTestCaseLogs().Where(a=>a.TestCaseId==testCaseId);
            List<Models.TestCaseLog> lstModelTestCaseLog = new List<Models.TestCaseLog>();
            if (lstTestCaseLog.Any())
            {
                foreach (var testCase in lstTestCaseLog)
                {
                    lstModelTestCaseLog.Add(mapObj.Translate(testCase));
                }
            }
            if (Session["Id"] != null)
            {
                string name = Session["Id"].ToString();
                ViewBag.name = name;
                //return PartialView("_GetAllTestCasesPartialView", lstModelTestCase);
                var jsonResult= Json(new { data = lstModelTestCaseLog }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
                
            }
            else
            {
                return Json("Login");
            }

        }
        [HttpPost]
        public ActionResult GetAllTestCaseLogs2()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var testInstance = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var testCase = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            ValidationMapper<TestCaseLog, Models.TestCaseLog> mapObj = new ValidationMapper<TestCaseLog, Models.TestCaseLog>();

            var lstTestCaseLog = dal.GetAllTestCaseLogs();
            List<Models.TestCaseLog> lstModelTestCaseLog = new List<Models.TestCaseLog>();
            if (lstTestCaseLog.Any())
            {
                foreach (var log1 in lstTestCaseLog)
                {
                    lstModelTestCaseLog.Add(mapObj.Translate(log1));
                }
            }
            if (!string.IsNullOrEmpty(testInstance))
            {
                lstModelTestCaseLog = lstModelTestCaseLog.Where(a => a.TestInstanceId==Convert.ToInt32(testInstance)).ToList();
            }
            if (!string.IsNullOrEmpty(testCase))
            {
                lstModelTestCaseLog = lstModelTestCaseLog.Where(a => a.TestCaseId ==Convert.ToInt32(testCase)).ToList();
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                lstModelTestCaseLog = lstModelTestCaseLog.OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            recordsTotal = lstModelTestCaseLog.Count();
            var data = lstModelTestCaseLog.Skip(skip).Take(pageSize).ToList();
            var jsonResult = Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

            
           

        }

        [HttpGet]
        //public ActionResult UpdateTestCase(Models.TestCase testCaseObj)
        public ActionResult UpdateTestCase(int testCaseId=0,int testSuiteId=0, int pipeLineStageId=0, int testCaseTypeId=0, string testCaseName="", string description="", bool isActive=false, bool isObsolete=false, string obsoleteReason="", DateTime? createdDate=null, string createdBy="")
        {
            if (Session["Id"] == null)
            {
                return RedirectToAction("Login");
            }
            if(testCaseId==0)
            {
                return RedirectToAction("GetAllTestCases");
            }
            Models.TestCase testCaseObj = new Models.TestCase();
            testCaseObj.TestCaseId = testCaseId;
            testCaseObj.TestSuiteId = testSuiteId;
            testCaseObj.PipelineStageId = pipeLineStageId;
            testCaseObj.TestCaseTypeId = testCaseTypeId;
            testCaseObj.TestCaseName = testCaseName;
            testCaseObj.Description = description;
            testCaseObj.IsActive = isActive;
            testCaseObj.IsObsolete = isObsolete;
            testCaseObj.ObsoleteReason = obsoleteReason;
            testCaseObj.CreatedDate = createdDate;
            testCaseObj.CreatedBy = createdBy;
            var lstTestSuites = dal.GetAllTestSuites();
            var lstTestCaseTypes = dal.GetAllTestCaseTypes();
            ViewBag.TestSuitesList = lstTestSuites;
            ViewBag.TestCaseTypeList = lstTestCaseTypes;
            var lstPipelineStages = dal.GetAllPipelineStages();
            ViewBag.PipelineStageList = lstPipelineStages;
            return View("UpdateTestCase",testCaseObj);
        }
        [HttpPost]
        public ActionResult SaveTestCase(Models.TestCase testCaseObj, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TestCase testObj = new TestCase();
                    testObj.IsActive = false;
                    testObj.IsObsolete = false;
                    testObj.TestCaseId = testCaseObj.TestCaseId;
                    testObj.TestSuiteId = Convert.ToInt32(form["testSuiteId"]);
                    testObj.PipelineStageId = testCaseObj.PipelineStageId;
                    testObj.TestCaseTypeId = testCaseObj.TestCaseTypeId;
                    testObj.TestCaseName = form["tstName"].ToString();
                    testObj.Description = form["desc"].ToString();
                    testObj.ObsoleteReason = form["obsRsn"];
                    if (form["status"].ToString()=="1")
                    {
                        testObj.IsActive = true;
                        testObj.ObsoleteReason = "";
                    }
                    else if(form["status"].ToString() == "2")
                    {
                        testObj.IsObsolete = true;
                    }
                    else if (form["status"].ToString() == "3")
                    {
                        testObj.ObsoleteReason = "";
                    }


                    //ValidationMapper<Models.TestCase, TestCase> mapObj = new ValidationMapper<Models.TestCase, TestCase>();
                    //var testCaseEntity = mapObj.Translate(testCaseObj);
                    var status = dal.UpdateTestCase(testObj);
                    if(status)
                    {
                        TempData["update"] = testObj.TestCaseId;
                        return RedirectToAction("GetAllTestCases");
                    }
                    else
                    {
                        TempData["update"] = "Update Failed";
                        return View("Failure");
                    }

                }
                else
                {
                    return View("Failure");
                }
            }
            catch
            {
                TempData["update1"] = "Update Failed";
                return View("Failure");
            }
            
        }

        public ActionResult AddTestCase()
        {
            if (Session["Id"] == null)
            {
                return RedirectToAction("Login");
            }
            var lstTestSuites = dal.GetAllTestSuites();
            var lstTestCaseTypes = dal.GetAllTestCaseTypes();
            ViewBag.TestSuitesList = lstTestSuites;
            ViewBag.TestCaseTypeList = lstTestCaseTypes;
            var lstPipelineStages = dal.GetAllPipelineStages();
            ViewBag.PipelineStageList = lstPipelineStages;
            return View("AddTestCasePartailView");
        }
        //[HttpPost]
        //public ActionResult AddTestCase(FormCollection form, Models.TestCase testCaseObj)
        //{
        //    this.AddTestCase();
        //    try
        //    {
        //        TestCase testObj = new TestCase();
        //        testObj.IsActive = false;
        //        testObj.IsObsolete = false;
                
        //        testObj.TestSuiteId = Convert.ToInt32(form["testSuiteId"]);
        //        testObj.PipelineStageId= testCaseObj.PipelineStageId;
        //        testObj.TestCaseTypeId = testCaseObj.TestCaseTypeId;
        //        testObj.TestCaseName = form["tstName"].ToString();
        //        testObj.Description= form["desc"].ToString();
                
        //        if (form["status"].ToString()=="1")
        //        {
        //            testObj.IsActive = true;
        //            testObj.ObsoleteReason = "";
        //        }
        //        else if(form["status"].ToString() == "2")
        //        {
        //            testObj.IsObsolete = true;
        //            testObj.ObsoleteReason = form["obsRsn"].ToString();
        //        }
        //        else if (form["status"].ToString() == "3")
        //        {
        //            testObj.ObsoleteReason = "";
        //        }

        //        var testCaseId = dal.AddTestCase(testObj, Session["Id"].ToString());
        //        if(testCaseId!=0)
        //        {
        //            TempData["testCsId"]= testCaseId;
        //            return RedirectToAction("GetAllTestCases");
        //        }
        //        else
        //        {
        //            TempData["testCsId1"] = "Some Error Occured. TestCase Not added";
        //            return RedirectToAction("GetAllTestCases");
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        TempData["testCsId"] = "Some Error Occured. Please Try Later";
        //        return RedirectToAction("GetAllTestCases");
        //    }
        //}

        //public ActionResult DeleteTestCase(int testCaseId = 0, int testSuiteId = 0, int pipeLineStageId = 0, int testCaseTypeId = 0, string testCaseName = "", string description = "", bool isActive = false, bool isObsolete = false, string obsoleteReason = "", DateTime? createdDate = null, string createdBy = "")
        //{
        //    Models.TestCase testCaseObj = new Models.TestCase();
        //    try
        //    {
        //        if (Session["Id"] == null)
        //        {
        //            return RedirectToAction("Login");
        //        }

        //        testCaseObj.TestCaseId = testCaseId;
        //        testCaseObj.TestSuiteId = testSuiteId;
        //        testCaseObj.PipelineStageId = pipeLineStageId;
        //        testCaseObj.TestCaseTypeId = testCaseTypeId;
        //        testCaseObj.TestCaseName = testCaseName;
        //        testCaseObj.Description = description;
        //        testCaseObj.IsActive = isActive;
        //        testCaseObj.IsObsolete = isObsolete;
        //        testCaseObj.ObsoleteReason = obsoleteReason;
        //        testCaseObj.CreatedDate = createdDate;
        //        testCaseObj.CreatedBy = createdBy;
                
        //    }
        //    catch(Exception e)
        //    {

        //    }
        //    return View("DeleteTestCase", testCaseObj);
        //}
        //[HttpPost]
        //public ActionResult DeleteTestCase(int testCaseId = 0)
        //{
        //    try
        //    {
        //        if(testCaseId!=0)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var status = dal.DeleteTestCase(testCaseId);
        //                if (status)
        //                {
        //                    TempData["tstId"] = testCaseId;
        //                    //return Json()
        //                    return RedirectToAction("GetAllTestCases");
        //                }
        //                else
        //                {
        //                    TempData["tstId"] = "Some Error Occured, Please try to delete again.";
        //                    return RedirectToAction("MainView","Main");
        //                }
        //            }
        //            else
        //            {
        //                return View("Failure");
        //            }
        //        }
        //        else
        //        {
        //            return View("Failure");
        //        }
        //        //Models.TestCase testObj = new Models.TestCase();
                
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["tstId"] = "Some Error Occured, Please try later.";
        //        return View("Failure");
        //    }

        //}
        
        [HttpGet]
        public ActionResult Save(int id=0)
        {
            if(Session["id"]==null)
            {
                return RedirectToAction("Login", "Home");
            }
            var tstCase = dal.GetAllTestCases().Where(a => a.TestCaseId == id).FirstOrDefault();
            ValidationMapper<TestCase, Models.TestCase> mapObj = new ValidationMapper<TestCase, Models.TestCase>();
            Models.TestCase testCaseObj = new Models.TestCase();
            if (tstCase!=null)
            {
                testCaseObj = mapObj.Translate(tstCase);
            }            
            
            var lstTestSuites = dal.GetAllTestSuites();
            var lstTestCaseTypes = dal.GetAllTestCaseTypes();
            ViewBag.TestSuitesList = lstTestSuites;
            ViewBag.TestCaseTypeList = lstTestCaseTypes;
            var lstPipelineStages = dal.GetAllPipelineStages();
            ViewBag.PipelineStageList = lstPipelineStages;
            return View(testCaseObj);

        }

        [HttpPost]
        public ActionResult Save(Models.TestCase testCaseObj,FormCollection form)
        {
            bool status = false;
            try
            {
                ValidationMapper<Models.TestCase, TestCase> mapObj = new ValidationMapper<Models.TestCase, TestCase>();
                if (ModelState.IsValid)
                {
                    if (form["status"].ToString() == "1")
                    {
                        testCaseObj.IsActive = true;
                        testCaseObj.ObsoleteReason = "";
                    }
                    else if (form["status"].ToString() == "2")
                    {
                        testCaseObj.IsObsolete = true;
                        testCaseObj.ObsoleteReason = form["obsRsn"].ToString();
                    }
                    else if (form["status"].ToString() == "3")
                    {
                        testCaseObj.ObsoleteReason = "";
                    }


                    if (testCaseObj.TestCaseId > 0)
                    {
                        //Edit
                        var testCase = dal.GetAllTestCases().Where(a => a.TestCaseId == testCaseObj.TestCaseId).FirstOrDefault();
                        if (testCase != null)
                        {

                            testCase = mapObj.Translate(testCaseObj);
                            status = dal.UpdateTestCase(testCase);
                        }

                    }
                    else
                    {
                        //Save
                        TestCase testObj = new TestCase();
                        testObj = mapObj.Translate(testCaseObj);
                        status = dal.AddTestCase(testObj, Session["Id"].ToString());

                    }


                }
            }
            catch(Exception e)
            {
                status = false;
            }
            
            
            
            return new JsonResult { Data = new { status = status } };
        }
        [HttpGet]
        public ActionResult Delete(int id=0)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var tstCase = dal.GetAllTestCases().Where(a => a.TestCaseId == id).FirstOrDefault();
            ValidationMapper<TestCase, Models.TestCase> mapObj = new ValidationMapper<TestCase, Models.TestCase>();
            Models.TestCase testCaseObj = new Models.TestCase();
            if(tstCase!=null)
            {
                testCaseObj = mapObj.Translate(tstCase);
                return View(testCaseObj);
            }
            else
            {
                return HttpNotFound();
            }
            
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteTestcase(int id)
        {
            bool status = false;

            var tstCase = dal.GetAllTestCases().Where(a => a.TestCaseId == id).FirstOrDefault();
            if (tstCase != null)
            {
                status = dal.DeleteTestCase(id);
            }
            if(status)
            {
                TempData["tstId"] = id;
            }
            return new JsonResult { Data = new { status = status } };
        }
        [HttpGet]
        public JsonResult TestCaseName(string prefix)
        {
            var lstTestCases = dal.GetAllTestCases().ToList();
            List<string> lstTestCaseNames = (from l in lstTestCases where l.TestCaseName.StartsWith(prefix) select l.TestCaseName).ToList();
            return Json(lstTestCaseNames, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }
}