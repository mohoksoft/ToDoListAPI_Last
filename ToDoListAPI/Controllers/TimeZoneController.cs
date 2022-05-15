using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ToDoListAPI.Models;

namespace ToDoListAPI.Controllers
{
    public class TimeZoneController : ApiController
    {
        [HttpPost]
        [Route("api/timezone")]
        public ResponseModel Post(string Name, string Password, string Title, string Description, string DeadLine, string ListName)
        {
            ResponseModel resp = new ResponseModel
            {
                Error = "OK",
                isOK = true
            };

            return resp;
        }
    }
}