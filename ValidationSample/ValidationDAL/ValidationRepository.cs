using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationDAL
{
    public class ValidationRepository
    {
        private ValidationDBContext Context;
        public ValidationRepository()
        {
            Context = new ValidationDBContext();
        }

        public List<TestSuite> GetAllTestSuites()
        {
            List<TestSuite> lstTestSuite = new List<TestSuite>();
            try
            {
                lstTestSuite = (from c in Context.TestSuites select c).ToList();
            }
            catch(Exception e)
            {
                lstTestSuite = null;
            }
            return lstTestSuite;
        }

        public List<TestCase> GetAllTestCases()
        {
            List<TestCase> lstTestCase = new List<TestCase>();
            try
            {
                lstTestCase = (from c in Context.TestCases select c).ToList();
            }
            catch (Exception e)
            {
                lstTestCase = null;
            }
            return lstTestCase;
        }

        public List<TestInstance> GetAllTestInstances()
        {
            List<TestInstance> lstTestInstance = new List<TestInstance>();
            try
            {
                lstTestInstance = (from c in Context.TestInstances select c).ToList();
            }
            catch (Exception e)
            {
                lstTestInstance = null;
            }
            return lstTestInstance;
        }

        public List<TestCaseLog> GetAllTestCaseLogs()
        {
            List<TestCaseLog> lstTestCaseLog = new List<TestCaseLog>();
            try
            {
                lstTestCaseLog = (from c in Context.TestCaseLogs select c).ToList();
            }
            catch (Exception e)
            {
                lstTestCaseLog = null;
            }
            return lstTestCaseLog;
        }

        public List<TestCaseType> GetAllTestCaseTypes()
        {
            List<TestCaseType> lstTestCaseType = new List<TestCaseType>();
            try
            {
                lstTestCaseType = (from c in Context.TestCaseTypes select c).ToList();
            }
            catch (Exception e)
            {
                lstTestCaseType = null;
            }
            return lstTestCaseType;
        }

        public List<PipelineStage> GetAllPipelineStages()
        {
            List<PipelineStage> lstPipelineStage = new List<PipelineStage>();
            try
            {
                lstPipelineStage = (from c in Context.PipelineStages select c).ToList();
            }
            catch (Exception e)
            {
                lstPipelineStage = null;
            }
            return lstPipelineStage;
        }

        public bool UpdateTestCase(TestCase testCaseObj)
        {
            var status = false;

            try
            {
                var obj = (from c in Context.TestCases where c.TestCaseId == testCaseObj.TestCaseId select c).FirstOrDefault();
                obj.TestSuiteId = testCaseObj.TestSuiteId;
                obj.PipelineStageId = testCaseObj.PipelineStageId;
                obj.TestCaseTypeId = testCaseObj.TestCaseTypeId;
                obj.TestCaseName = testCaseObj.TestCaseName;
                obj.Description = testCaseObj.Description;
                obj.IsActive = testCaseObj.IsActive;
                obj.IsObsolete = testCaseObj.IsObsolete;
                obj.ObsoleteReason = testCaseObj.ObsoleteReason;
                Context.SaveChanges();
                status = true;

            }
            catch (Exception e)
            {
                status = false;
            }
            return status;
        }

        public bool DeleteTestCase(int testCaseId)
        {
            var status = false;

            try
            {
                var obj = (from c in Context.TestCases where c.TestCaseId == testCaseId select c).ToList();
                if(obj[0]!=null)
                {
                    Context.TestCases.Remove(obj[0]);
                    Context.SaveChanges();
                    status = true;
                }
                

            }
            catch
            {
                status = false;
            }
            return status;
        }

        public string CheckUser(string userid, string password)
        {
            var userName = "";

            try
            {
                var obj = (from c in Context.Admins where c.UserId == userid && c.Password == password select c).FirstOrDefault();
                if (obj != null)
                {
                    userName = obj.UserName;
                }


            }
            catch
            {
                userName = "";
            }
            return userName;
        }

        public string AddUser(string userName, string password)
        {
            string userId = "";

            try
            {
                var retval = Context.usp_RegisterUser(userName, password);
                var lstUser = (from c in Context.Admins select c).ToList();
                foreach(var item in lstUser)
                {
                    userId = item.UserId;
                }
                


            }
            catch (Exception e)
            {
                userId = "";
            }
            return userId;
        }
        public bool AddTestCase(TestCase testObj,string userName)
        {
            bool status = false;
            int testCaseId = 0;
            try
            {
                var retval = Context.usp_AddTestCase(testObj.TestSuiteId, testObj.PipelineStageId, testObj.TestCaseTypeId, testObj.TestCaseName, testObj.Description, testObj.IsActive, testObj.IsObsolete, testObj.ObsoleteReason, DateTime.Now, userName);
                var lstTstCase = (from c in Context.TestCases select c).OrderByDescending(c=>c.TestCaseId).ToList();
                testCaseId = lstTstCase[0].TestCaseId;
                status = true;
                    
            }
            catch (Exception e)
            {
                testCaseId = 0;
                status = false;
            }
            return status;
        }
        //testsuite
        public int AddTestSuite(TestSuite testObj, string userName)
        {
            testObj.TestSuiteId = GenerateTestSuiteID();
            testObj.CreatedDate = DateTime.Now;
            testObj.CreatedBy = userName;
            Context.TestSuites.Add(testObj);
            Context.SaveChanges();
            return testObj.TestSuiteId;
        }

        public int GenerateTestSuiteID()
        {
            var lstTstSuite = (from c in Context.TestSuites select c).OrderByDescending(c => c.TestSuiteId).ToList();
            int testSuiteId = lstTstSuite[0].TestSuiteId+1;
            return testSuiteId;
        }
        public bool DeleteTestSuite(int testSuiteId)
        {
            var status = false;

            try
            {
                var obj = (from c in Context.TestSuites where c.TestSuiteId == testSuiteId select c).ToList();
                Context.TestSuites.Remove(obj[0]);
                Context.SaveChanges();
                status = true;

            }
            catch
            {
                status = false;
            }
            return status;
        }

        public List<TestCase> Search(string term)
        {
            return this.GetAllTestCases().Where(p => (p.TestCaseName.StartsWith(term, StringComparison.OrdinalIgnoreCase) || p.CreatedBy.StartsWith(term,StringComparison.OrdinalIgnoreCase))).Select(p => p).ToList();
        }

        public List<Event> GetEvents()
        {
            List<Event> lstEvent = new List<Event>();
            try
            {
                lstEvent = (from c in Context.Events select c).ToList();
            }
            catch (Exception e)
            {
                lstEvent = null;
            }
            return lstEvent;
        }

        public bool CreateEvent(Event eObj)
        {
            eObj.CreatedDate = DateTime.Now;

            if (eObj.RecurrenceCount != null)
            {
                eObj.RecurrenceCount = "COUNT=" + eObj.RecurrenceCount;
            }
            if (eObj.RecurrenceInterval != null)
            {
                eObj.RecurrenceInterval = "INTERVAL=" + eObj.RecurrenceInterval;
            }
            if (eObj.Recurrence != null)
            {
                eObj.Recurrence = "FREQ=" + eObj.Recurrence.ToUpper();
            }
            else
            {
                eObj.RecurrenceCount = null;
                eObj.RecurrenceInterval = null;
            }
            Context.Events.Add(eObj);
            Context.SaveChanges();
            return true;
        }
        public bool EditEvent(Event eObj)
        {
            bool status = false;
            try
            {
                var event1 = (from c in Context.Events where c.EventId == eObj.EventId select c).FirstOrDefault();
                if(event1!=null)
                {
                    event1.Subject = eObj.Subject;
                    event1.Description = eObj.Description;
                    event1.IsFullDay = eObj.IsFullDay;
                    event1.ThemeColor = eObj.ThemeColor;
                    event1.StartTime = eObj.StartTime;
                    event1.EndTime = eObj.EndTime;

                    if (eObj.RecurrenceCount != null)
                    {
                        event1.RecurrenceCount = "COUNT=" + eObj.RecurrenceCount;
                    }
                    if (eObj.RecurrenceInterval != null)
                    {
                        event1.RecurrenceInterval = "INTERVAL=" + eObj.RecurrenceInterval;
                    }
                    if (eObj.Recurrence != null)
                    {
                        event1.Recurrence = "FREQ=" + eObj.Recurrence.ToUpper();
                    }
                    else
                    {
                        event1.Recurrence = null;
                        event1.RecurrenceCount = null;
                        event1.RecurrenceInterval = null;
                    }
                    event1.Location = eObj.Location;
                    Context.SaveChanges();
                    status = true;
                }
            }
            catch(Exception e)
            {
                status = false;
            }
            return status;
        }
        public bool DeleteEvent(Event e)
        {
            Context.Events.Remove(e);
            Context.SaveChanges();
            return true;
        }
        public List<TestCaseLog> GetTestcaseLogs(int pageNumber,int pageSize)
        {
            var lstTestcaseLogs = Context.spGetTestcaseLogs(pageNumber, pageSize).ToList();
            List<TestCaseLog> lstEntityTestcaseLogs = new List<TestCaseLog>();
            foreach(var log1 in lstTestcaseLogs)
            {
                TestCaseLog log2 = new TestCaseLog();
                log2.TestCaseLogId = log1.TestCaseLogId;
                log2.TestInstanceId = log1.TestInstanceId;
                log2.TestCaseId = log1.TestCaseId;
                log2.ResultType = log1.ResultType;
                log2.EventStartTime = log1.EventStartTime;
                log2.EventEndTime = log1.EventEndTime;
                log2.Exception = log1.Exception;
                log2.CreatedDate = log1.CreatedDate;
                log2.CreatedBy = log1.CreatedBy;
                lstEntityTestcaseLogs.Add(log2);
            }
            return lstEntityTestcaseLogs;
        }
        
    }
}
