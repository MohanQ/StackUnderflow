
using Mvc.Mailer;
using OpenPop.Mime;
using OpenPop.Pop3;
using QuestionsAndAnswers.Managers;
using QuestionsAndAnswers.Models;
using System;
using System.Collections.Generic;
using WebMatrix.WebData;

namespace QuestionsAndAnswers.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer 	
	{
		public UserMailer()
		{
			MasterName="_Layout";
		}


        public virtual MvcMailMessage NewQuestion(Question question, UserProfile to)
        {
            string subject;
            if (EmailConfig.IsReceivingEmailSetted)
            {
                var hash = UserManager.SentEmailHash(to.UserId, question.Id);
                subject = Resources.Global.NewQuestionSubject + " - Id:" + hash;
            }
            else
            {
                subject = Resources.Global.NewQuestionSubject;
            }

            ViewBag.UserName = to.UserName;
            ViewBag.Question = question;

            return Populate(x =>
            {
                x.Subject = subject;
                x.ViewName = "NewQuestion";
                x.To.Add(to.Email);
            });
        }


        public virtual MvcMailMessage NewAnswer(Question question, UserProfile to, Answer answer)
        {
            string subject;
            if (EmailConfig.IsReceivingEmailSetted)
            {
                var hash = UserManager.SentEmailHash(question.Id, to.UserId);
                subject = Resources.Global.NewAnswerSubject+" - Id:" + hash;
            }
            else
            {
                subject = Resources.Global.NewAnswerSubject;
            }

            ViewBag.User = to;
            ViewBag.Question = question;
            ViewBag.Answer = answer;

            return Populate(x =>
            {
                x.Subject = subject;
                x.ViewName = "NewAnswer";
                x.To.Add(to.Email);
            });
        }

        public virtual MvcMailMessage UserVerification(UserProfile up, string key)
        {
            ViewBag.User = up;
            
            ViewBag.Key = key;
            return Populate(x =>
            {
                x.Subject = Resources.Global.EmailVerifySubject;
                x.ViewName = "EmailVerification";
                x.To.Add(up.Email);
            });
        }

        public virtual MvcMailMessage FailedEmailAnswer(string email)
        {
            return Populate(x =>
            {
                x.Subject = Resources.Global.FailedEmailReply;
                x.ViewName = "FailedEmailAnswer";
                x.To.Add(email);
            });
        }

 	}
}