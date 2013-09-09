using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;

namespace QuestionsAndAnswers.Managers
{
    public static class VoteManager
    {

        /// <summary>
        /// Get the true, if user voted to question or else false
        /// </summary>
        /// <param name="questionid">Question Id</param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        public static bool  IsVotedForQuestion(int questionid, int userid)
        {
            using (var db=new QaAContext())
            {
                var q = HttpContext.Current.Cache.GetFromCache("QuestionVote" + questionid, () => (from v in db.QuestionHasVotes
                         where v.QuestionId == questionid && v.UserId == userid
                         select v).SingleOrDefault());
                if (q == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Get the true, if user voted to ansert or else false
        /// </summary>
        /// <param name="answerid">Válasz Id</param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        public static bool IsVotedForAnswer(int answerid, int userid)
        {
            using (var db = new QaAContext())
            {
                var q = HttpContext.Current.Cache.GetFromCache("AnswerVote" + answerid, () => (from v in db.AnswerHasVotes
                         where v.AnswerId == answerid && v.UserId == userid
                         select v).SingleOrDefault());
                if (q == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Write the user's vote to the question
        /// </summary>
        /// <param name="questionid">question Id</param>
        /// <param name="userid">User Id</param>
        /// <param name="vote">Vote value</param>
        public static void Vote(int questionid, int userid, int vote)
        {
            using (var db=new QaAContext())
            {
                QuestionHasVote add = new QuestionHasVote();
                add.QuestionId=questionid;
                add.UserId=userid;
                add.Rating=vote;
                db.QuestionHasVotes.Add(add);
                db.SaveChanges();
                HttpContext.Current.Cache.UpdateCache("QuestionVote" + questionid, add);

            }
        }

        /// <summary>
        /// Write the user's vote to the answer
        /// </summary>
        /// <param name="answerid">answer Id</param>
        /// <param name="userid">User Id</param>
        /// <param name="vote">Vote</param>
        public static void VoteAnswer(int answerid, int userid, int vote)
        {
            using (var db = new QaAContext())
            {
                AnswerHasVote add = new AnswerHasVote();
                add.AnswerId = answerid;
                add.UserId = userid;
                add.Rating = vote;
                db.AnswerHasVotes.Add(add);
                db.SaveChanges();

                HttpContext.Current.Cache.UpdateCache("AnswerVote" + answerid, add);
            }
        }
    }
}