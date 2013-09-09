using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;
using QuestionsAndAnswers.Mailers;

namespace QuestionsAndAnswers.Managers
{
    public static class AnswerManager
    {
        /// <summary>
        /// Get the answer from the id
        /// </summary>
        /// <param name="id">Answer's id</param>
        /// <returns></returns>
        public static Answer GetAnswer(int id)
        {
            using (var db=new QaAContext())
            {
                var ans = HttpContext.Current.Cache.GetFromCache("GetAnswer"+id, () => db.Answers.Where(a=>a.Id==id).SingleOrDefault());

                return ans;
            }
        }

        /// <summary>
        /// List the answer based on the id of the questions
        /// </summary>
        /// <param name="id">Question ID</param>
        /// <returns></returns>
        public static List<Answer> GetAllAnswerToOneQuestion(int id)
        {
            using (var db = new QaAContext())
            {
                var q = HttpContext.Current.Cache.GetFromCache("AnswersToQuestion"+id, () =>(from ans in db.Answers where ans.QuestionId == id select ans).ToList());
                return q;
            }
        }

        /// <summary>
        /// Get the number of all answers from the id of the questions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetAllAnswerNumberToOneQuestion(int id)
        {
            using (var db = new QaAContext())
            {
                var q = HttpContext.Current.Cache.GetFromCache("AnswersToQuestion" + id, () => (from ans in db.Answers where ans.QuestionId == id select ans).ToList()).Count();
                return q;
            }
        }

        /// <summary>
        /// Get the answers'vote
        /// </summary>
        /// <param name="id">Answer Id</param>
        /// <returns></returns>
        public static int GetVote(int id)
        {
            using (var db = new QaAContext())
            {

                var vote = HttpContext.Current.Cache.GetFromCache("AnswerVote"+id, () => (from ans in db.AnswerHasVotes
                                  where ans.AnswerId == id
                                  select ans.Rating).ToList());
                //if there isn't vote
                if (vote.Count == 0)
                    return 0;
                return vote.Sum();

            }
        }



        /// <summary>
        /// Get all answer from one questions of the user
        /// </summary>
        /// <param name="questionid">Question ID</param>
        /// <param name="userid">User ID</param>
        /// <returns></returns>
        public static List<Answer> GetAllAnswerToOneQuestionFromOneUser(int questionid, int userid)
        {
            using (var db=new QaAContext())
            {
                var answers = HttpContext.Current.Cache.GetFromCache("AnswersToQuestion" + questionid+"User"+userid, () => (from a in db.Answers
                               where a.QuestionId == questionid && a.UserId == userid
                               select a).ToList());
                return answers;

            }
        }

        /// <summary>
        /// Add the answer to Database
        /// </summary>
        /// <param name="data">Answer</param>
        public static void AddAnswer(Answer data, int questionID, int userid)
        {
            using (var db = new QaAContext())
            {
                data.Date = DateTime.Now;
                data.UserId = userid;
                data.QuestionId = questionID;
                db.Answers.Add(data);
                db.SaveChanges();

                HttpContext.Current.Cache.UpdateCache("AnsweredQuestions" + userid, data);
                HttpContext.Current.Cache.UpdateCache("AnswersToQuestion"+questionID, data);
                HttpContext.Current.Cache.UpdateCache("AnswersToQuestion" + questionID+"User"+userid, data);
            
                //Send mails to the subscripted users
                var question = db.Questions.Where(q => q.Id == questionID).SingleOrDefault();
                IUserMailer usermailer = new UserMailer();
                var tagsOfQuestion = question.QuestionHasTags.Select(s => s.TagId).ToList();
                var tos = db.UserProfiles.Where(up => (up.Subscriptions.Any(u => tagsOfQuestion.Any(x => x == u.TagId)) && up.Email!= null && up.IsVerified==true)).ToList();

                foreach (var user in tos)
                {
                    usermailer.NewAnswer(question, user, data).SendAsync();
                }
            }
        }


        /// <summary>
        /// Answer edit
        /// </summary>
        /// <param name="data">Answer's data</param>
        public static void EditAnswer(Answer data)
        {
            using (var db = new QaAContext())
            {
                var q = from a in db.Answers where a.Id == data.Id select a;
                var ans = q.SingleOrDefault();
                ans.Content = data.Content;
                db.SaveChanges();
                HttpContext.Current.Cache.UpdateCache("AnswersToQuestion" + data.QuestionId, data);
                HttpContext.Current.Cache.UpdateCache("AnswersToQuestion" + data.QuestionId + "User" + data.UserId, data);
            }
        }


    }
}