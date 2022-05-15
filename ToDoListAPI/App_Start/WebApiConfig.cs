using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Timers;
using System.Web;
using System.Web.Http;
using ToDoListAPI.Data;
using ToDoListAPI.Models;

namespace ToDoListAPI
{
    public static class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            StartMailTimer(60000);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private static System.Timers.Timer MailTimer;
        public static void StartMailTimer(int Interval_ms)
        {
            MailTimer = new System.Timers.Timer();
            MailTimer.Elapsed += OnTimedEvent;
            MailTimer.Interval = Interval_ms; // in miliseconds => 1 minut
            MailTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            MailTimer.Stop();
            string time = DateTime.Now.ToString("HH:mm");
            if (time == "00:00")
            {
                ToDoDBContext dbContext = new ToDoDBContext();
                var tDoneUsers = dbContext.Tasks.SqlQuery("Select * FROM Tasks WHERE Done = 'true' AND DeadLine = '" + DateTime.Now.ToString("yyyy-M-dd") + "' ORDER BY UserID").ToList();

                if (tDoneUsers.Count() > 0)
                {
                    EMailUserModel EMUser = new EMailUserModel();

                    int iUID = 0;
                    for (int n = 0; n < tDoneUsers.Count() - 1; n++)
                    {

                        if (iUID != tDoneUsers[n].UserID)
                        {
                            var tUser = dbContext.Users.SqlQuery("Select * FROM Users WHERE ID = " + tDoneUsers[n].UserID).ToList();
                            EMUser.EMail = tUser[0].EMail;
                            EMUser.Name = tUser[0].Name;

                            var tDoneTasks = dbContext.Tasks.SqlQuery("Select * FROM Tasks WHERE Done = 'true' AND DeadLine = '" + DateTime.Now.ToString("yyyy-M-dd") + "' AND UserID = " + tDoneUsers[n].UserID).ToList();
                            string sTasks = "";

                            for (int t = 0; t < tDoneTasks.Count() - 1; t++)
                            {
                                sTasks += tDoneTasks[t].Title + tDoneTasks[t].Description + 0x0D + 0x0A;

                            }
                            EMUser.TaskList = sTasks;
                            EMUser.Subject = "ToDo";

                            EMUser.SendEmail();
                        }

                        iUID = tDoneUsers[n].UserID;
                    }
                }
            }
            MailTimer.Start();
        }

        
        
    }

    
}
