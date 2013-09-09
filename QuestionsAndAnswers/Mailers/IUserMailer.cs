using Mvc.Mailer;
using OpenPop.Mime;
using QuestionsAndAnswers.Models;
using System.Collections.Generic;


namespace QuestionsAndAnswers.Mailers
{ 
    public interface IUserMailer
    {
            MvcMailMessage NewQuestion(Question question, UserProfile to);
            MvcMailMessage NewAnswer(Question question, UserProfile to, Answer answer);
            MvcMailMessage UserVerification(UserProfile up, string key);
            MvcMailMessage FailedEmailAnswer(string email);
    }
}