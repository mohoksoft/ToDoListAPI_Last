using System.Data.Entity;
using ToDoListAPI.Data;
using System.Linq;
using System;

/*
 * The DBTool class is used to work with the database
 */


namespace ToDoListAPI.Models
{
    
    public class DBTool
    {
        ToDoDBContext dbContext = new ToDoDBContext();

        // Sorting enumeration
        public enum Sort
        {
            Date = 0,
            Title = 1,
            UserName = 2
        };

        // Is there a user exists.It can be checked by name and password or by id.Returns 0 if not found, and returns the id of the user if found.
        public int UserExists(string Name,string Password)
        {
            int resp = 0;

            var usID = dbContext.Users.SqlQuery("Select * FROM Users WHERE Name = '" + Name + "' AND Password = '" + Password + "'").ToList();

            if(usID.Count > 0)
            {
                resp = usID[0].ID;
            }

            return resp;
        }

        public int UserExists(int UserID)
        {
            int resp = 0;

            var usID = dbContext.Users.SqlQuery("Select * FROM Users WHERE ID = " + UserID).ToList();

            if (usID.Count > 0)
            {
                resp = usID[0].ID;
            }

            return resp;
        }

        // Returns TaskID if exists, or 0 if not.
        public int TaskExists(int TaskID,int UserID)
        {
            int resp = 0;

            var tID = dbContext.Users.SqlQuery("Select * FROM Tasks WHERE ID = " + TaskID + " AND UserID = " + UserID).ToList();

            if (tID.Count > 0)
            {
                resp = tID[0].ID;
            }

            return resp;
        }

        public int TaskExists(string Title, string UserName, string Password,string DeadLine)
        {
            int resp = 0;

            var users = dbContext.Users.SqlQuery("Select * FROM Users WHERE Name = '" + UserName + "' AND Password = '" + Password + "'").ToList();

            if (users.Count > 0)
            {
                resp = users[0].ID;
            }

            var tID = dbContext.Tasks.SqlQuery("Select * FROM Tasks WHERE Title = '" + Title + "' AND UserID = " + resp + " AND DeadLine = '" + DeadLine + "'").ToList();

            if (tID.Count > 0)
            {
                resp = tID[0].KeyID;
            }

            return resp;
        }

        // Returns ListID if present, or 0 if not.
        public int ListExists(int ListID,int UserID)
        {
            int resp = 0;

            var lID = dbContext.Lists.SqlQuery("Select * FROM Lists WHERE ID = " + ListID + " AND UserID = " + UserID).ToList();

            if (lID.Count > 0)
            {
                resp = lID[0].KeyID;
            }

            return resp;
        }

        public int ListExists(string Title,string UserName,string Password)
        {
            int resp = 0;

            int userid = UserExists(UserName, Password);

            var lID = dbContext.Lists.SqlQuery("Select * FROM Lists WHERE Title = '" + Title + "' AND UserID = " + userid).ToList();

            if (lID.Count > 0)
            {
                resp = lID[0].KeyID;
            }

            return resp;
        }

        /*
         * returns the JSON string format of the page given in the page parameter with the page bounding constraint parameter pagelimit.
         * UserID -user ID
         * sOrder - 0 - Date, 1 - Title, 2 - UserName
         * fDate - filter Date
         * fTitle - filter Title
         */
        public string SList(int UserID,Sort sOrder,string fDate,string fTitle, int? pageLimit = null, int? page = null)
        {
            pageLimit = pageLimit ?? 10;

            using (ToDoDBContext dbContext = new ToDoDBContext())
            {

                string orderBY = "Date";

                switch(sOrder)
                {
                    case (Sort)1:
                        orderBY = "Title";
                        break;
                    case (Sort)2:
                        orderBY = "UserID";
                        break;
                }
                IQueryable<List> listS = dbContext.Lists.SqlQuery("SELECT * FROM Lists WHERE UserID = " + UserID + " AND Date = '" + fDate + "' AND Title = '" + fTitle + "' ORDER BY " + orderBY).AsQueryable();


                return new CNPagedList<List>(listS, page, pageLimit).items;
            }
        }

        public string STask(int UserID, Sort sOrder, bool fDone, string fDeadLine, int? pageLimit = null, int? page = null)
        {
            pageLimit = pageLimit ?? 2;

            using (ToDoDBContext dbContext = new ToDoDBContext())
            {

                string orderBY = "DeadLine";

                switch (sOrder)
                {
                    case (Sort)1:
                        orderBY = "Title";
                        break;
                    case (Sort)2:
                        orderBY = "UserID";
                        break;
                }
                int fd = 0;
                if (fDone == true)fd = 1;
                IQueryable<Task> taskS = dbContext.Tasks.SqlQuery("SELECT * FROM Tasks WHERE UserID = " + UserID + " AND DeadLine = '" + fDeadLine + "' AND Done = " + fd + " ORDER BY " + orderBY).AsQueryable();

                return new CNPagedList<Task>(taskS, page, pageLimit).items;
            }
        }
    }


}