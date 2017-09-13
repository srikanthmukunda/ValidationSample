using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using ValidationSample.Repository;
using ValidationDAL;

namespace ValidationSample
{
    public class NotificationComponent
    {
        ValidationRepository dal = new ValidationRepository();
        public void RegisterNotification(DateTime currentTime)
        {
            string conStr = ConfigurationManager.ConnectionStrings["sqlConString"].ConnectionString;
            string sqlCommand = @"SELECT [EventId],[Subject],[Description],[StartTime],[EndTime],[ThemeColor],[IsFullDay] from [dbo].[Events] where [CreatedDate] > @AddedOn";
            //you can notice here I have added table name like this [dbo].[Contacts] with [dbo], its mendatory when you use Sql Dependency
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);
                cmd.Parameters.AddWithValue("@AddedOn", currentTime);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChange;
                //we must have to execute the command here
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here now
                }
            }
        }

        void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //or you can also check => if (e.Info == SqlNotificationInfo.Insert) , if you want notification only for inserted record
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;

                //from here we will send notification message to client
                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");
                //re-register notification
                RegisterNotification(DateTime.Now);
            }
        }

        public List<Models.Event> GetEvents(DateTime afterDate)
        {
            ValidationMapper<Event, Models.Event> mapObj = new ValidationMapper<Event, Models.Event>();

            var lstEvent = (dal.GetEvents().Where(a=>a.CreatedDate>afterDate).OrderByDescending(a=>a.CreatedDate)).ToList();
            List<Models.Event> lstModelEvent = new List<Models.Event>();
            if (lstEvent.Any())
            {
                foreach (var event1 in lstEvent)
                {
                    lstModelEvent.Add(mapObj.Translate(event1));
                }
            }
            return lstModelEvent;
        }
    }
}