using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;

namespace QuestionsAndAnswers.Managers
{
    public static class UserManager
    {
        /// <summary>
        /// List the all user
        /// </summary>
        /// <returns></returns>
        public static List<UserProfile> GetAllUsers()
        {
            using (var db =new QaAContext())
            {
                var q = from u in db.UserProfiles
                        select u;

                return q.ToList();
            }
        }

        /// <summary>
        /// Get the UserProfil based on ID
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        public static UserProfile GetUserById(int id)
        {
            using (var db=new QaAContext())
            {
                var q = (from u in db.UserProfiles
                         where u.UserId == id
                         select u).SingleOrDefault();
                return q;
            }

        }

        /// <summary>
        /// Get the reputation of the user (Reputation=Rating of User's Questions + Rating of User's Answers)
        /// </summary>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        public static int GetUserRating(int userid)
        {
            using (var db=new QaAContext())
            {

                var qrating = db.QuestionHasVotes.Where(qhv => qhv.Question.UserId == userid).Select(s => s.Rating).ToList();
                var arating = db.AnswerHasVotes.Where(ahv => ahv.Answer.UserId == userid).Select(s => s.Rating).ToList();
                return (qrating == null ? 0 : qrating.Sum()) + (arating == null ? 0 : arating.Sum());

            }
        }

        /// <summary>
        /// User subcribe to tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="user"></param>
        public static void SubcribeToTag(int tag, int user)
        {
            using (var db = new QaAContext())
            {   
                //Check the subcribe
                var q = (from t in db.UserHasSubscribes where (t.TagId == tag && t.UserId == user) select t).FirstOrDefault();
                //if it does not exist yet
                if (q == null)
                {
                    var subc = new UserHasSubscribe
                    {
                        TagId = tag,
                        UserId = user
                    };
                    db.UserHasSubscribes.Add(subc);
                    db.SaveChanges();
                }

            }
        }

        /// <summary>
        /// User subcribes to more tags
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="user"></param>
        public static void SubcribeToMoreTags(List<Tag> tags, int user)
        {
            using (var db = new QaAContext())
            {
                foreach (var item in tags)
                {
                    //Check the subcribe
                    var q = (from t in db.UserHasSubscribes where (t.TagId == item.Id && t.UserId == user) select t).FirstOrDefault();
                    //if it does not exist yet
                    if (q == null)
                    {
                        var subc = new UserHasSubscribe
                        {
                            TagId = item.Id,
                            UserId = user
                        };
                        db.UserHasSubscribes.Add(subc);
                        db.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// List all subcribed tag to one user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static List<Tag> AllSubcribeTagToOneUser(int userid)
        {
            using (var db = new QaAContext())
            {
                var q = db.UserHasSubscribes.Where(uhs => uhs.UserId == userid).Select(t => t.Tag).ToList();
                return q;
            }
        }

        /// <summary>
        /// Get all subcribed tag's count to one user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static int AllSubcribeTagsCountToOneUser(int userid)
        {
            using (var db = new QaAContext())
            {
                var q = db.UserHasSubscribes.Where(uhs => uhs.UserId == userid).Select(t => t.Tag).ToList();
                return q.Count;
            }
        }

        /// <summary>
        /// List all subcribed user to one tag
        /// </summary>
        /// <param name="tagid"></param>
        /// <returns></returns>
        public static List<UserProfile> AllSubcribeUserToOneTag(int tagid)
        {
            using (var db = new QaAContext())
            {
                var q = db.UserHasSubscribes.Where(uhs => uhs.TagId == tagid).Select(u => u.UserProfile).ToList();
                return q;
            }
        }

        /// <summary>
        /// List all subcribed user's count to one tag
        /// </summary>
        /// <param name="tagid"></param>
        /// <returns></returns>
        public static int AllSubcribeUsersCountToOneTag(int tagid)
        {
            using (var db = new QaAContext())
            {
                var q = db.UserHasSubscribes.Where(uhs => uhs.TagId == tagid).Select(u => u.UserProfile).ToList();               
                return q.Count;
            }
        }

        /// <summary>
        /// List 6 tag to one user. 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static List<Tag> SixTagToOneUser(int userid)
        {
            using (var db = new QaAContext())
            {
                var q = db.UserHasSubscribes.Where(uhs => uhs.UserId == userid).Select(t => t.Tag).Take(6).ToList();
                return q;
            }
        }




        /// <summary>
        /// Add email to the user
        /// </summary>
        /// <param name="up">UserProfile</param>
        public static void AddEmail(UserProfile up)
        {
            using (var db=new QaAContext())
            {
                var user = db.UserProfiles.Where(u => u.UserId == up.UserId).SingleOrDefault();
                user.Email = up.Email;
                user.IsVerified = false;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Verify the email of the user
        /// </summary>
        /// <param name="id">UserID</param>
        public static void VerifyEmail(int id)
        {
            using (var db=new QaAContext())
            {
                var user = db.UserProfiles.Where(u => u.UserId == id).SingleOrDefault();
                if (user == null) return;
                user.IsVerified = true;
                db.SaveChanges();
            }
        }

        public static string SentEmailHash(int userid, int questionid)
        {
            using (var db=new QaAContext())
            {
                var hash=MyHelpers.MD5Encode("u" + userid.ToString() + "q" + questionid.ToString());
                db.EmailIdentifiers.Add(new EmailIdentifier { UserId = userid, QuestionId = questionid, Hash = hash });
                db.SaveChanges();
                return hash;
            }
        }

        public static EmailIdentifier GetEmailDatasByHash(string hash)
        {
            using (var db=new QaAContext())
            {
                var data=db.EmailIdentifiers.Where(ei => ei.Hash == hash).FirstOrDefault();
                
                return data;
            }

        }
    }
}