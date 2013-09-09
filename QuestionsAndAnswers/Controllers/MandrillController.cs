using HtmlAgilityPack;
using Newtonsoft.Json;
using QuestionsAndAnswers.Mailers;
using QuestionsAndAnswers.Managers;
using QuestionsAndAnswers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace QuestionsAndAnswers.Controllers
{
    public class MandrillController : Controller
    {
        private IUserMailer _userMailer = new UserMailer();
        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }


        [AcceptVerbs(HttpVerbs.Head), AllowAnonymous]
        public ActionResult Parse() { 
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false), AllowAnonymous]
        public ActionResult Parse(string mandrill_events)
        {
            //Get the email message from the mandrill
            var events = JsonConvert.DeserializeObject<IEnumerable<Mandrill.MailEvent>>(mandrill_events);
            string message=events.FirstOrDefault().Msg.Html;
            string email=events.FirstOrDefault().Msg.FromEmail;

            //Store the answer which will in the site
            string answerContent;

            
            if (message == null || message.Length == 0)
            {
                //Yahoo emails process
                message = events.FirstOrDefault().Msg.Text;
                if (message == null || message.Length == 0)
                {
                    UserMailer.FailedEmailAnswer(email);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                answerContent = Regex.Split(message, "-------------------------------------------").FirstOrDefault();
            }
            else
            {
                //Gmail and hotmail process
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(message);
                var firstdiv = doc.DocumentNode.SelectSingleNode("//div");
                if (firstdiv == null)
                {
                    UserMailer.FailedEmailAnswer(email);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                answerContent = firstdiv.InnerHtml;

                //Hotmail
                answerContent = Regex.Split(answerContent, "<hr id=\"stopSpelling\">").FirstOrDefault();
            }

           
            if (answerContent == null || answerContent.Length == 0)
            {
                UserMailer.FailedEmailAnswer(email);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            //Get the question and user id
            var subject=events.FirstOrDefault().Msg.Subject;
            string hash=Regex.Split(subject, "Id:").LastOrDefault();
            if (hash == null || hash.Length == 0)
            {
                UserMailer.FailedEmailAnswer(email);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            EmailIdentifier ei = UserManager.GetEmailDatasByHash(hash);
            if (ei == null)
            {
                UserMailer.FailedEmailAnswer(email);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            //Check answer length
            if (answerContent.Length > 25000 || answerContent.Length < 25)
            {
                UserMailer.FailedEmailAnswer(email);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            //Create the answer
            var ans = new Answer();
            ans.Content = answerContent;
            AnswerManager.AddAnswer(ans, ei.QuestionId, ei.UserId);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

    }
}
