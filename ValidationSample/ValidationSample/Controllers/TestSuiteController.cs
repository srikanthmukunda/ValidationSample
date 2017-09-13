using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationDAL;
using AutoMapper;
using ValidationSample.Repository;

namespace ValidationSample.Controllers
{
    public class TestSuiteController : Controller
    {
        ValidationRepository dal = new ValidationRepository();
        // GET: TestSuite
        public ActionResult Index(FormCollection frm)
        {
            
            if(frm["srch"]!=null)
            {
                string srch = frm["srch"].ToString().ToLower();
                var lstTstSuites = dal.GetAllTestSuites().Where(c => (c.TestSuite1.ToLower().Contains(srch)));
                ValidationMapper<TestSuite, Models.TestSuite> mapObj = new ValidationMapper<TestSuite, Models.TestSuite>();
                List<Models.TestSuite> lstModelTestSuite = new List<Models.TestSuite>();
                if (lstTstSuites.Any())
                {
                    foreach (var testSuite in lstTstSuites)
                    {
                        lstModelTestSuite.Add(mapObj.Translate(testSuite));
                    }
                    return View("GetAllTestSuites", lstModelTestSuite);
                }
                else
                {
                    return View("GetAllTestSuites");
                }
            }
            else
            {
                return RedirectToAction("GetAllTestSuites","TestSuite");
            }
            
            //lstTstSuites=dal.GetAllTestSuites().Where(c => (c.TestSuite1.ToLower().Contains(frm["srch"].ToLower())));
            

            
        }

        public ActionResult GetAllTestSuites()
        {
            ValidationMapper<TestSuite, Models.TestSuite> mapObj = new ValidationMapper<TestSuite, Models.TestSuite>();

            var lstTestSuite = dal.GetAllTestSuites();
            List<Models.TestSuite> lstModelTestSuite = new List<Models.TestSuite>();
            if (lstTestSuite.Any())
            {
                foreach (var testSuite in lstTestSuite)
                {
                    lstModelTestSuite.Add(mapObj.Translate(testSuite));
                }
            }
            if (Session["Id"] != null)
            {
                string name = Session["Id"].ToString();
                ViewBag.name = name;
                return View(lstModelTestSuite);
            }
            else
            {
                return RedirectToAction("Login","Home");
            }

        }

        public ActionResult AddTestSuite()
        {
            if (Session["Id"] == null)
            {
                return RedirectToAction("Login","Home");
            }
            var lstTestSuites = dal.GetAllTestSuites();
            var lstTestCaseTypes = dal.GetAllTestCaseTypes();
            ViewBag.TestSuitesList = lstTestSuites;
            ViewBag.TestCaseTypeList = lstTestCaseTypes;
            var lstPipelineStages = dal.GetAllPipelineStages();
            ViewBag.PipelineStageList = lstPipelineStages;
            return View("AddTestSuite");
        }
        [HttpPost]
        public ActionResult AddTestSuite(FormCollection form, Models.TestSuite testSuiteObj)
        {
            this.AddTestSuite();
            try
            {
                TestSuite testObj = new TestSuite();
                testObj.IsActive = false;
               

                testObj.TestSuite1 = form["tstStName"].ToString();
                testObj.TestSuiteDescription = form["desc"].ToString();

                if (form["status"].ToString() == "1")
                {
                    testObj.IsActive = true;                    
                }
                testObj.TestSuiteOwner= form["tstOwn"].ToString();

                var testSuiteId = dal.AddTestSuite(testObj, Session["Id"].ToString());
                if (testSuiteId != 0)
                {
                    TempData["testStId"] = testSuiteId;
                    return RedirectToAction("GetAllTestSuites");
                }
                else
                {
                    TempData["testStId"] = "Some Error Occured. TestSuite Not added";
                    return RedirectToAction("AddTestSuite");
                }
            }
            catch (Exception e)
            {
                TempData["testCsId"] = "Some Error Occured. Please Try Later";
                return RedirectToAction("AddTestSuite");
            }
        }

        public ActionResult DeleteTestSuite(int testSuiteId = 0)
        {
            try
            {
                if (testSuiteId != 0)
                {
                    if (ModelState.IsValid)
                    {
                        var status = dal.DeleteTestSuite(testSuiteId);
                        if (status)
                        {
                            TempData["tstId"] = testSuiteId;
                            //return Json()
                            return RedirectToAction("GetAllTestSuites");
                        }
                        else
                        {
                            TempData["tstId"] = "Some Error Occured, Please try to delete again.";
                            return RedirectToAction("GetAllTestCases");
                        }
                    }
                    else
                    {
                        return View("Failure");
                    }
                }
                else
                {
                    return View("Failure");
                }
                //Models.TestCase testObj = new Models.TestCase();

            }
            catch (Exception e)
            {
                TempData["tstId"] = "Some Error Occured, Please try later.";
                return View("Failure");
            }

        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }


}