using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;
using QuestionsAndAnswers.Mailers;

namespace QuestionsAndAnswers.Managers
{

    public static class QuestionManager
    {
        /// <summary>
        ///Get the all questions from chronology(descending)
        /// </summary>
        /// <returns></returns>
        public static List<Question> GetAllQuestion()
        {
            using (var db = new QaAContext())
            {
                var q = HttpContext.Current.Cache.GetFromCache("AllQuestions", () => (from qs in db.Questions select qs).OrderByDescending(v => v.Date).ToList());
                return q;
            }
        }

        /// <summary>
        /// Get the x number of the latest questions 
        /// </summary>
        /// <param name="x">Number of the question</param>
        /// <returns></returns>
        public static List<Question> GetXLatestQuestion(int x)
        {
            using (var db = new QaAContext())
            {
                var q = HttpContext.Current.Cache.GetFromCache("LatestQuestions", () => db.Questions.OrderByDescending(v => v.Date).Take(x).ToList());
                return q;
            }
        }

        /// <summary>
        /// Return order set questions (chronology, descending), which there are on the appropriate page and return number of the all questions
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="total">Number of all questions</param>
        /// <returns></returns>
        public static List<Question> AllQuestionsToPagedList(int pageNumber, int pageSize, out int total)
        {
            using (var db = new QaAContext())
            {
                List<Question> questions;
                //If the pagenumber is more, than 5 -> take the questions from database
                if (pageNumber > 5)
                {
                    questions = db.Questions.OrderByDescending(q => q.Date).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    //If the pagenumber isn't more, than 5 -> take the questions from cache (if there are the questions in cache)
                    questions = HttpContext.Current.Cache.GetFromCache("PageQuestions" + pageNumber, () => db.Questions.OrderByDescending(q => q.Date).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
                }
                //Query number of all question
                total = db.Questions.Count();
                return questions;
            }
        }


        /// <summary>
        /// Get the list of question from the page, which the user answered
        /// </summary>
        /// <param name="userid">User ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="total">Number of all such question</param>
        /// <returns></returns>
        public static List<Question> AnsweredQuestionsToPagedList(int userid, int pageNumber, int pageSize, out int total)
        {
            using (var db = new QaAContext())
            {
                //The user answered question
                var ques = HttpContext.Current.Cache.GetFromCache("AnsweredQuestions" + userid, () => (db.Questions.Where(q => q.Answers.Any(a => a.UserId == userid)).OrderByDescending(quest => quest.Date).Select(t => t).ToList()));
                //Take the questions according to the page
                ques = ques.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                //Take number of user's questions, which the user answered
                total = db.Questions.Where(q => q.Answers.Any(a => a.UserId == userid)).Count();

                return ques;
            }
        }


        /// <summary>
        /// List the questions of the user from the page
        /// </summary>
        /// <param name="userid">User ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="total">Number of all such question</param>
        /// <returns></returns>
        public static List<Question> AllQuestionToOneUserToPagedList(int userid, int pageNumber, int pageSize, out int total)
        {
            using (var db = new QaAContext())
            {
                
                var questions = HttpContext.Current.Cache.GetFromCache("QuestionsByUser" + userid, () => db.Questions.Where(q => q.UserId == userid).OrderByDescending(q => q.Date).ToList());
                //Take the questions according to the page
                questions = questions.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                //Get number of such questions
                total = db.Questions.Where(q => q.UserId == userid).Count();
                return questions;
            }
        }

        /// <summary>
        /// Get the questions, which pass to user
        /// </summary>
        /// <param name="userid">User ID</param>
        /// <returns></returns>
        public static List<Question> GetQuestionsFitToUser(int userid)
        {
            using (var db = new QaAContext())
            {
                
                //Query the tags of user's answered question and count and then we tahe 3 most answered tags 

                var usertags = db.Tags.Where(d => d.QuestionHasTags.Any(q => q.Question.Answers.Any(a => a.UserId == userid))).Select(d => new
                {
                    Tag = d,
                    Num = d.QuestionHasTags.Count()
                }).OrderByDescending(o => o.Num).Take(3).Select(t => t.Tag);

                
                //Take the current time minus 7 day, because we want to know which question come in the last week 
                var lastday = DateTime.Now.AddDays(-7);

               
                //Query the questions,which the last week they were and No replies to them and there are such tag, which tag is user's favourite. Finally take the 10 latest tag 
                var fitquestions = db.Questions.Where(q => (q.Date > lastday &&  q.Answers.Count() == 0 && q.QuestionHasTags.Any(qht => usertags.Contains(qht.Tag)) )).OrderByDescending(x => x.Date).Take(10).ToList();

                return fitquestions;
            }
        }

        /// <summary>
        /// List the issue in question
        /// </summary>
        /// <param name="questionid">question ID</param>
        /// <returns></returns>
        public static List<Question> GetRelatedQuestions(int questionid)
        {
            using (var db = new QaAContext())
            {

                //List all tag based on question ID
                var tags = HttpContext.Current.Cache.GetFromCache("QuestionTags" + questionid, () => TagManager.GetAllTagToOneQuestion(questionid).Select(x => x.Id).ToList());
                        
                //Take the questions, where the question has got tag (which there is in tags), and take the most recent of 10
                var releateds = HttpContext.Current.Cache.GetFromCache("ReleatedQuestions" + questionid, () => db.Questions.Where(q => q.QuestionHasTags.Any(qht => tags.Contains(qht.TagId)) && q.Id != questionid).OrderByDescending(d => d.Date).Take(10).ToList());
                return releateds;
            }

        }
        /// <summary>
        /// Get the questions based on Question ID
        /// </summary>
        /// <param name="id"> Question ID</param>
        /// <returns></returns>
        public static Question GetQuestion(int id)
        {
            using (var db = new QaAContext())
            {
                var ques = HttpContext.Current.Cache.GetFromCache("GetQuestion" + id, () => (db.Questions.Where(q => q.Id == id).SingleOrDefault()));
                return ques;
            }

        }

        /// <summary>
        /// Get the vote's summa based on QuestionID
        /// </summary>
        /// <param name="id">Question Id</param>
        /// <returns></returns>
        public static int GetVote(int id)
        {
            using (var db = new QaAContext())
            {
                //Get the question's vote based on Question ID
                var vote = HttpContext.Current.Cache.GetFromCache("QuestionVote" + id, () => (from q in db.QuestionHasVotes
                                                                                              where q.QuestionId == id
                                                                                              select q.Rating).ToList());
                //If the rating equals ZERO  -> return 0
                if (vote.Count == 0)
                    return 0;
                //if the rating doesn't equal ZERO -> return vote's summa
                return vote.Sum();

            }

        }


        /// <summary>
        /// List the questions basod on tag
        /// </summary>
        /// <param name="tagid">Tag ID</param>
        /// <returns></returns>
        ///   
        public static List<Question> AllQuestionToOneTagToPagedList(int tagid, int pageNumber, int pageSize, out int total)
        {
            using (var db = new QaAContext())
            {
                //Make the list to the questions
                List<Question> questions;

                //If the pagenumber is more, than 5 -> take the questions from database ELSE from the cache
                if (pageNumber > 5)
                {
                    questions = db.Questions.Where(q => q.QuestionHasTags.Any(qht => qht.TagId == tagid)).OrderByDescending(q => q.Date).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                }
                else
                {
                    questions = HttpContext.Current.Cache.GetFromCache("QuestionsByTag" + tagid + "Page" + pageNumber, () => db.Questions.Where(q => q.QuestionHasTags.Any(qht => qht.TagId == tagid)).OrderByDescending(q => q.Date).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
                }
                
                //Query all questions, where the question have got given tagID
                total = db.Questions.Where(q => q.QuestionHasTags.Any(qht => qht.TagId == tagid)).Count();
                return questions;
            }
        }


        /// <summary>
        /// Add Question to the Database
        /// </summary>
        /// <param name="data">Question's data</param>
        public static void AddQuestion(Question data, int userID, List<string> tagList)
        {
            using (var db = new QaAContext())
            {
                //Set the actual datetime
                data.Date = DateTime.Now;
                data.UserId = userID;
                db.Questions.Add(data);
                db.SaveChanges();



                //Tag handling
                //Check the tag's existence, if tag does not exist, then we insert the tag
                Tag tag = null;
                foreach (var item in tagList)
                {
                    var q = (from t in db.Tags where item.Equals(t.Name) select t).ToArray();
                    //Insert                 
                    if (q.Length == 0)
                    {
                        tag = new Tag();
                        tag.Name = item;
                        db.Tags.Add(tag);
                        db.SaveChanges();

                        //Add tag to the question
                        var qt = new QuestionHasTag();
                        qt.QuestionId = data.Id;
                        qt.TagId = tag.Id;
                        db.QuestionHasTags.Add(qt);
                        db.SaveChanges();
                    }
                    //In the case, the tag exist 
                    else
                    {
                        var query = from t in db.Tags where item.Equals(t.Name) select t;
                        tag = query.Single();
                        var qt = new QuestionHasTag();
                        qt.QuestionId = data.Id;
                        qt.TagId = tag.Id;
                        db.QuestionHasTags.Add(qt);
                        db.SaveChanges();
                    }
                    HttpContext.Current.Cache.UpdateCache("Tags", item);
                }

                db.SaveChanges();


                int pageCache = 5;

                HttpContext.Current.Cache.UpdateCache("AllQuestions", data);
                HttpContext.Current.Cache.UpdateCache("LatestQuestions", data);
                HttpContext.Current.Cache.UpdateCache("QuestionsByUser" + data.UserId, data);

                for (int i = 1; i < pageCache; i++)
                {
                    HttpContext.Current.Cache.UpdateCache("PageQuestions" + i, data);
                }

                foreach (var qht in data.QuestionHasTags)
                {
                    for (int i = 1; i < pageCache; i++)
                    {
                        HttpContext.Current.Cache.UpdateCache("QuestionsByTag" + qht.TagId + "Page" + i, data);
                    }
                }

                HttpContext.Current.Cache.UpdateCache("QuestionsByUser" + data.UserId, data);

                //Send mails to the subscripted users
                IUserMailer usermailer = new UserMailer();
                var tagsOfQuestion = data.QuestionHasTags.Select(s=>s.TagId).ToList();
                var tos = db.UserProfiles.Where(up => (up.Subscriptions.Any(u => tagsOfQuestion.Any(x => x == u.TagId)) && up.Email!=null && up.IsVerified==true)).ToList();
                
                foreach (var user in tos)
                {
                    usermailer.NewQuestion(data, user).SendAsync();
                }
            }
        }


        /// <summary>
        /// Question edit
        /// </summary>
        /// <param name="data">Question's data</param>
        public static void EditQuestion(Question data)
        {
            using (var db = new QaAContext())
            {
                var q = from question in db.Questions where (question.Id == data.Id) select question;
                var editableData = q.SingleOrDefault();
                editableData.Title = data.Title;
                editableData.Content = data.Content;
                db.SaveChanges();
                HttpContext.Current.Cache.UpdateCache("GetQuestion" + data.Id, data);
            }
        }

        /// <summary>
        /// Get the title based on key (to Ajax request)
        /// </summary>
        /// <param name="key">keyparameter</param>
        /// <returns></returns>
        public static List<Question> AjaxSearchTitle(string key)
        {
            using (var db = new QaAContext())
            {
                var questions = (from q in db.Questions where q.Title.Contains(key) select q).ToList();
                return questions;
            }
        }

        /// <summary>
        /// List all question based on tag's name
        /// </summary>
        /// <param name="tagName">tagName</param>
        /// <returns></returns>
        public static List<Question> AllQuestionToTag(string tagName)
        {
            using (var db = new QaAContext())
            {
                List<Question> rl = null;
                var queryId = db.Tags.Where(tags => tags.Name.Equals(tagName)).SingleOrDefault();
                if (queryId != null)
                {
                    rl = db.Questions.Where(q => q.QuestionHasTags.Any(qht => qht.TagId == queryId.Id)).ToList();
                }
                
                return rl;
            }
        }


    }
}