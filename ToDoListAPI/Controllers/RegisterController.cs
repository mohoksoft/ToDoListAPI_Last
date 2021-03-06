using ToDoListAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoListAPI.Data;

/*
 * The registration controller enters a new user in the database, but before that it checks whether such a user exists.
 * also can edit user.
 * delete user also deletes all tasks and lists bilonged to this user
 */

namespace ToDoListAPI.Controllers
{
    public class RegisterController : ApiController
    {
        DBTool DBase = new DBTool();

        /*
         * Name - user name or mail
         * Password - user pasword
         * return error message if user already exists or OK otherwise
         * user ID is generated by SQL server
         */

        [HttpPost]
        [Route("api/user/add")]
        public ResponseModel PostAdd(string Name,string Password,string EMail,string sTimeZone)
        {
            using (ToDoDBContext dbContext = new ToDoDBContext())
            {
                ResponseModel resp = new ResponseModel
                {
                    Error = "OK",
                    isOK = true
                };

                int id = DBase.UserExists(Name, Password);

                
                

                if (id > 0)
                {
                    resp.isOK = false;
                    resp.Error = "A user with that name and password already exists.";
                }
                else
                {
                    int utc_h = 0;
                    int utc_m = 0;
                    foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
                    {
                        if(z.Id == sTimeZone)
                        {
                            utc_h = z.BaseUtcOffset.Hours;
                            utc_m = z.BaseUtcOffset.Minutes;
                            break;
                        }
                    }

                    // The user ID defines the software so that a new ID number is automatically assigned.

                    // Saving in to the database
                    dbContext.Database.ExecuteSqlCommand("INSERT INTO Users(Name,Password,EMail,TimeZone,UTCOffset_Hours,UTCOffset_Minutes) values('" + Name + "','" + Password + "','" + EMail + "','" + sTimeZone + "'," + utc_h + "," + utc_m + ")");
                }

                return resp;
            }
        }


        /*
         * Name - user name
         * Password - user password
         * 
         * when it is new time zone all tasks getting updates with new offsets
         */
        [HttpGet]
        [Route("api/user/edit")]
        public ResponseModel GetEdit(string Name, string Password,string NewName,string NewPassword,string NewEMail,string NewTimeZone)
        {
            using (ToDoDBContext dbContext = new ToDoDBContext())
            {
                ResponseModel resp = new ResponseModel
                {
                    Error = "OK",
                    isOK = true
                };

                int id = DBase.UserExists(Name, Password);

                if (id == 0)
                {
                    resp.isOK = false;
                    resp.Error = "A user with that name and password do not exists.";
                }
                else
                {
                    int utc_h = 0;
                    int utc_m = 0;

                    //find timezone and set new offset hours and minutes
                    foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
                    {
                        if (z.Id == NewTimeZone)
                        {
                            utc_h = z.BaseUtcOffset.Hours;
                            utc_m = z.BaseUtcOffset.Minutes;
                            break;
                        }
                    }

                    //get old timezone offsets
                    var otz = dbContext.Users.SqlQuery("SELECT * FROM Users WHERE ID = " + id).ToList();

                    // Update user's new data
                    dbContext.Database.ExecuteSqlCommand("UPDATE Users SET Name = '" + NewName + "', Password = '" + NewPassword + "',EMail = '" + NewEMail + "' WHERE ID = " + id);

                    // if NewTimeZone is different frm old one update, otherwise do nothing
                    if(otz[0].TimeZone != NewTimeZone)
                    {
                        //Update user's timezone with new ones
                        dbContext.Database.ExecuteSqlCommand("UPDATE Users SET TimeZone = '" + NewTimeZone + "',UTCOffset_Hours = " + utc_h + ",UTCOffset_Minutes = " + utc_m + " WHERE ID = " + id);

                        // calculate new difference between new ans old offset
                        utc_h -= otz[0].UTCOffset_Hours.Value;
                        utc_m -= otz[0].UTCOffset_Minutes.Value;

                        //Update all user's tasks with new timestamps
                        dbContext.Database.ExecuteSqlCommand("UPDATE Tasks SET DeadLine = DATEADD(hour," + utc_h + ",DeadLine) WHERE UserID = " + id);
                        dbContext.Database.ExecuteSqlCommand("UPDATE Tasks SET DeadLine = DATEADD(minute," + utc_m + ",DeadLine) WHERE UserID = " + id);
                    }
                    
                }

                return resp;
            }
        }


        /*
         * Name - user name
         * Password - user password
         * when deletes user, also deletes all his tasks and lists
         * there is no check if user exists. If user do not exists nothing will be deleted
         */
        [HttpGet]
        [Route("api/user/delete")]
        public ResponseModel GetDelete(string Name, string Password)
        {
            using (ToDoDBContext dbContext = new ToDoDBContext())
            {
                ResponseModel resp = new ResponseModel
                {
                    Error = "OK",
                    isOK = true
                };

                int id = DBase.UserExists(Name, Password);
                if (id == 0)
                {
                    resp.isOK = false;
                    resp.Error = "A user with that name and password do not exists.";
                }

                // Delete user from database
                dbContext.Database.ExecuteSqlCommand("DELETE FROM Users WHERE ID = " + id);
                dbContext.Database.ExecuteSqlCommand("DELETE FROM Tasks WHERE UserID = " + id);
                dbContext.Database.ExecuteSqlCommand("DELETE FROM Lists WHERE UserID = " + id);

                return resp;
            }
        }
    }
}