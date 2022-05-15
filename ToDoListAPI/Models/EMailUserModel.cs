using System;
using System.IO;
using System.Web;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Net.Configuration;
using System.Configuration;
using System.Net;

namespace ToDoListAPI.Models
{

    public class EMailUserModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string TaskList { get; set; }
        public string Subject { get; set; }

        public string SendEmail()
        {
            //Read SMTP section from Web.Config.
            SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            using (MailMessage mm = new MailMessage(smtpSection.From, EMail))
            {
                mm.Subject = Subject;
                mm.Body = TaskList;

                mm.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = smtpSection.Network.Host;
                    smtp.EnableSsl = smtpSection.Network.EnableSsl;
                    NetworkCredential networkCred = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = smtpSection.Network.Port;
                    smtp.Send(mm);
                }
            }

            return "Email sent sucessfully.";
        }
    }

}