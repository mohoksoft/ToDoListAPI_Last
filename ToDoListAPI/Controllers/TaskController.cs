using ToDoListAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoListAPI.Data;

/*
 * The task controller enters a new task in the database, but before that it checks whether such a task exists.
 * also list, edit and delete tasks
 */

namespace ToDoListAPI.Controllers
{
    public class TaskController : ApiController
    {
        DBTool DBase = new DBTool();

        /*
         * Name - user name
         * Password - user password
         * Title - task title
         * Description - task description
         * DeadLine - task date
         * ListName - the list to which the task belongs 
         */
        [HttpPost]
        [Route("api/task/add")]
        public ResponseModel Post(string Name, string Password, string Title,string Description, string DeadLine, string ListName)
        {
            using (ToDoDBContext dbContext = new ToDoDBContext())
            {
                ResponseModel resp = new ResponseModel
                {
                    Error = "OK",
                    isOK = true
                };
                int userID = DBase.UserExists(Name, Password);

                if(userID == 0)
                {
                    resp.isOK = false;
                    resp.Error = "User not exists.";

                    return resp;
                }
                int listid = DBase.ListExists(ListName, Name, Password);
                // Saving in to the database
                dbContext.Database.ExecuteSqlCommand("INSERT INTO Tasks(UserID,Title,Description,DeadLine,Done,ListID) values(" + userID + ",'" + Title + "','" + Description + "','" + DeadLine + "',0," + listid + ")");

                return resp;
            }
        }

        /*
         * Name - user name
         * Password - user password
         * DeadLine - task deadline filter by
         * WhichPage - current page to send
         * sOrder - 0 - order by Date,1 - order by Title, 2 - order by UserName
         * fDone - filter 
         * fDeadLine - filter
         */
        [HttpPost]
        [Route("api/task/listall")]
        public string PostAll(string Name, string Password, int WhichPage, DBTool.Sort sOrder,bool fDone,string fDeadLine)
        {
            using (ToDoDBContext dbContext = new ToDoDBContext())
            {
                string resp = "OK";

                int userID = DBase.UserExists(Name, Password);

                if (userID == 0)
                {
                    resp = "ERROR: User not exists.";

                    return resp;
                }

                if (WhichPage == 0)
                {
                    resp = "ERROR: Pages are not zero based.";

                    return resp;
                }

                var sl = DBase.STask(userID, sOrder, fDone, fDeadLine, 1, WhichPage);

                return sl;
            }
        }

        /*
         * Name - user name
         * Password - user password
         * DeadLine - task deadline 
         * Description - task description
         * Title - task title
         */
        [HttpPost]
        [Route("api/task/edit")]
        public ResponseModel PostEdit(string Name, string Password, string Title, string Description, string DeadLine,string NewTitle,string NewDescription,string NewDeadLine)
        {
            using (ToDoDBContext dbContext = new ToDoDBContext())
            {
                ResponseModel resp = new ResponseModel
                {
                    Error = "OK",
                    isOK = true
                };

                int userID = DBase.UserExists(Name, Password);

                if (userID == 0)
                {
                    resp.Error = "ERROR: User not exists.";
                    resp.isOK = false;
                    return resp;
                }

                int taskID = DBase.TaskExists(Title, Name, Password,DeadLine);

                if (taskID == 0)
                {
                    resp.Error = "ERROR: Task not exists.";
                    resp.isOK = false;
                    return resp;
                }

                dbContext.Database.ExecuteSqlCommand("UPDATE Tasks SET Title = '" + NewTitle + "', Description = '" + NewDescription + "', DeadLine = '" + NewDeadLine + "' WHERE KeyID = " + taskID);

                return resp;
            }
        }

        /*
         * Name - user name
         * Password - user password
         * Title - task title
         * Description - task description
         * DeadLine - task deadline 
         */
        [HttpPost]
        [Route("api/task/delete")]
        public ResponseModel PostDelete(string Name, string Password, string Title, string Description, string DeadLine)
        {
            using (ToDoDBContext dbContext = new ToDoDBContext())
            {
                ResponseModel resp = new ResponseModel
                {
                    Error = "OK",
                    isOK = true
                };

                int userID = DBase.UserExists(Name, Password);

                if (userID == 0)
                {
                    resp.Error = "ERROR: User not exists.";
                    resp.isOK = false;
                    return resp;
                }

                int taskID = DBase.TaskExists(Title, Name, Password,DeadLine);

                if (taskID == 0)
                {
                    resp.Error = "ERROR: Task not exists.";
                    resp.isOK = false;
                    return resp;
                }

                dbContext.Database.ExecuteSqlCommand("DELETE FROM Tasks WHERE KeyID = " + taskID);

                return resp;
            }
        }
    }
}