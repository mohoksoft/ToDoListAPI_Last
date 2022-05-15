using ToDoListAPI.Models;
using System.Web.Http;
using ToDoListAPI.Data;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using System;

/*
 * The List controller which is used to work with lists.
 * 
 */

namespace ToDoListAPI.Controllers
{
    public class ListController : ApiController
    {
        DBTool DBase = new DBTool();

        /*
         * adds a new list to the database
         * Name - username
         * Password - user's password
         * Title - title of the list
         * Description - description of the list
         * dDate - date of the list
         * 
         * returns - "OK" for successiful adding list in db or "User not exists." if user not saved in db
         * 
         */
        [HttpGet]
        [Route("api/list/add")]
        public ResponseModel Get(string Name, string Password, string Title, string Description, string dDate)
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
                    resp.isOK = false;
                    resp.Error = "User not exists.";

                    return resp;
                }
                // How much does it make sense to check if a list with the same name exists?
                //int listid = DBase.ListExists(Title, Name, Password);

                // Saving in to the database
                
                dbContext.Database.ExecuteSqlCommand("INSERT INTO Lists(UserID,Title,Description,Date) values(" + userID + ",'" + Title + "','" + Description + "','" + dDate + "')");

                return resp;
            }
        }

        /*
         * return JSON format data with number of pages and current page
         * Name - username
         * Password - user's password
         * WhichPage - current page to send
         * sOrder - 0 - order by Date,1 - order by Title, 2 - order by UserName
         */
        [HttpPost]
        [Route("api/list/listall")]
        public string PostAll(string Name, string Password,int WhichPage, DBTool.Sort sOrder, string fDate, string fTitle)
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

                var sl = DBase.SList(userID,sOrder,fDate,fTitle, 1,WhichPage);
                
                return sl;
            }
        }

        /*
         * edit list with given parameters
         */
        [HttpPost]
        [Route ("api/list/edit")]
        public ResponseModel PostEdit(string Name, string Password,string Title,string Description,string dDate, string NewTitle, string NewDescription, string NewDate)
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

                int listID = DBase.ListExists(Title, Name, Password);

                if (listID == 0)
                {
                    resp.Error = "ERROR: List not exists.";
                    resp.isOK = false;
                    return resp;
                }

                dbContext.Database.ExecuteSqlCommand("UPDATE Lists SET Title = '" + NewTitle + "', Description = '" + NewDescription + "', Date = '" + NewDate + "' WHERE KeyID = " + listID);

                return resp;
            }
        }

        /*
         * Name - username
         * Password - user's password
         * Title - title of the list
         * Description - description of the list
         * dDate - date of the list
         * 
         * At this point should decide what to do with tasks belongs to this deleted list
         * 1. deleting all tasks of this list
         * 2. not deleting tasks, but update field in Tasks table ListID = 0
         */

        [HttpPost]
        [Route("api/list/delete")]
        public ResponseModel PostDelete(string Name, string Password, string Title, string Description, string dDate)
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

                int listID = DBase.ListExists(Title, Name, Password);

                if (listID == 0)
                {
                    resp.Error = "ERROR: List not exists.";
                    resp.isOK = false;
                    return resp;
                }

                dbContext.Database.ExecuteSqlCommand("DELETE FROM Lists WHERE KeyID = " + listID);

                return resp;
            }
        }

    }
    
}