using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using ToDoListAPI.Data;
using ToDoListAPI.Models;

namespace ToDoListAPI.Controllers
{
    public class AuthenticationController : ApiController
    {
        [HttpPost]
        [Route("Authentication")]

        public ResponseModel Post(string Name, string Password)
        {
            using (ToDoDBContext dbContext = new ToDoDBContext())
            {
                ResponseModel resp = new ResponseModel
                {
                    Error = "OK",
                    isOK = true
                };

                var user = dbContext.Users.SqlQuery("Select * FROM Users WHERE Name = '" + Name + "' AND Password = '" + Password + "'").ToList();
                if (user.Count == 0)
                {
                    resp.isOK = false;
                    resp.Error = "There is no user with this username and password.";
                }

                return resp;
            }
        }
    }
}

/* ***************************************************************************************************************
 * 
 * In the some other version of the software, user input would certainly be taken from the body of the POST method, 
 * but for these testing purposes it is much easier to enter through the swagger interface by taking user input through the arguments of the POST method. 

/*
 * public JObject AuthenticationService([FromBody] JObject authenticationJson)

        {
            JObject retJson = new JObject();
            string username = authenticationJson["username"].ToString();
            string password = authenticationJson["password"].ToString();

            if (username == "user" && password == "user")
            {
                retJson.Add(new JProperty("authentication ", "successful"));
            }
            else
            {
                retJson.Add(new JProperty("authentication ", "unsuccessful"));
            }
            return retJson;
        }
*/