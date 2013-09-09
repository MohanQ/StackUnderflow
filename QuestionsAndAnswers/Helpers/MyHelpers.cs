using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.ControllersModel;
using QuestionsAndAnswers.Models;
using QuestionsAndAnswers.Managers;
using System.Web.Caching;
using PagedList;
using System.Text;
using System.Security.Cryptography;

namespace QuestionsAndAnswers
{
    public static class MyHelpers
    {
        /// <summary>       
        /// Get the difference current date and given date
        /// </summary>
        /// <param name="d">Given date</param>
        /// <returns></returns>
        public static string ElapsedTime(DateTime d)
        {
            var elapsed = DateTime.Now - d;
            string ret = "";
            if (elapsed.TotalSeconds < 60)
            {
                if (elapsed.Seconds == 0)
                {
                    ret = Resources.Global.Now;
                }
                else
                {
                    ret = elapsed.Seconds.ToString() + " " + Resources.Global.seconds;
                }

            }
            else if (elapsed.TotalMinutes < 60)
            {
                ret = elapsed.Minutes.ToString() + " " + Resources.Global.minutes;
            }
            else if (elapsed.TotalHours < 24)
            {
                ret = elapsed.Hours.ToString() + " " + Resources.Global.hours + " " + elapsed.Minutes.ToString() + " " + Resources.Global.minutes;
            }
            else
            {
                ret += elapsed.Days.ToString() + " " + Resources.Global.days+" " + elapsed.Hours.ToString() + " " + Resources.Global.hours;
            }
            return ret;
        }

        /// <summary>
        /// From list of the question make a QuestionIndexModel list to index page
        /// </summary>
        /// <param name="questions">Question list</param>
        /// <returns></returns>
        public static List<QuestionIndexModel> ToQuestionIndexModel(IEnumerable<Question> questions)
        {
            List<QuestionIndexModel> qim = new List<QuestionIndexModel>();
            foreach (var item in questions)
            {
                var add = new QuestionIndexModel();
                add.Question = item;
                add.Answers = AnswerManager.GetAllAnswerToOneQuestion(item.Id).Count;
                add.Vote = QuestionManager.GetVote(item.Id);
                add.Tags = TagManager.GetAllTagToOneQuestion(item.Id);
                add.QuestionUser = UserManager.GetUserById(item.UserId);

                qim.Add(add);
            }

            return qim;

        }

        public static List<GetQuestionModel> ToGetQuestionModels(IEnumerable<Question> questions, int userid)
        {
            //Create a list with the answered questions
            var answeredquestions = new List<GetQuestionModel>();
            foreach (var item in questions)
            {

                var actansweredquestion = new GetQuestionModel();
                //Add the current question to the model
                actansweredquestion.CurrentQuestion = item;
                //Add the answers by the user of the current question to the model
                actansweredquestion.Answers = AnswerManager.GetAllAnswerToOneQuestionFromOneUser(item.Id, userid);
                //Add the vote of current question to the model
                actansweredquestion.Vote = QuestionManager.GetVote(item.Id);
                //Add the tags of the current question to the model
                actansweredquestion.QuestionTags = TagManager.GetAllTagToOneQuestion(item.Id);

                actansweredquestion.AllAnswersNumber = AnswerManager.GetAllAnswerNumberToOneQuestion(item.Id);

                //Create a dictionary for the answers and the votes of them
                actansweredquestion.AnswerVotes = new Dictionary<Answer, int>();

                //Upload the dictionary with the votes
                foreach (var answer in actansweredquestion.Answers)
                {
                    actansweredquestion.AnswerVotes.Add(answer, AnswerManager.GetVote(answer.Id));
                }

                //Add the author of the question to the model
                actansweredquestion.QuestionUser = UserManager.GetUserById(item.UserId);

                //Sort the answers by the votes of them
                actansweredquestion.Answers = actansweredquestion.Answers.OrderByDescending(d => actansweredquestion.AnswerVotes[d]).ToList();

                //Add this QuestionModel to the list
                answeredquestions.Add(actansweredquestion);
            }

            return answeredquestions;
        }


        public static string Encode(string encodeable)
        {
            UnicodeEncoding uEncode = new UnicodeEncoding();
            byte[] bytD2e = uEncode.GetBytes(encodeable);
            SHA256Managed sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(bytD2e);
            return Convert.ToBase64String(hash);
        }

        public static string MD5Encode(string encodeable)
        {
            UnicodeEncoding uEncode = new UnicodeEncoding();
            byte[] bytD2e = uEncode.GetBytes(encodeable);
            MD5Cng sha = new MD5Cng();
            byte[] hash = sha.ComputeHash(bytD2e);
            return Convert.ToBase64String(hash);
        }
    }

    
    public static class CacheTrick
    {

        public static T GetFromCache<T>(this Cache cache, string key, Func<T> loadFunction, TimeSpan slidingExpiration) where T : class
        {

            return GetFromCache<T>(cache, key, loadFunction, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration);

        }



        public static T GetFromCache<T>(this Cache cache, string key, Func<T> loadFunction, DateTime absoluteExpiration) where T : class
        {

            return GetFromCache<T>(cache, key, loadFunction, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);

        }



        public static T GetFromCache<T>(this Cache cache, string key, Func<T> loadFunction, bool allowNullEntries = false) where T : class
        {

            return GetFromCache<T>(cache, key, loadFunction, DateTime.Now.AddMinutes(60), System.Web.Caching.Cache.NoSlidingExpiration, allowNullEntries);

        }

        /// <summary>

        ///Overwrite cache's content with given object, and setting, that it keep for 60 minutes
        /// </summary>

        /// <typeparam name="T">Object's type</typeparam>

        /// <param name="cache">cache</param>

        /// <param name="key">Storing object's key</param>

        /// <param name="newData">New string object</param>

        public static void UpdateCache<T>(this Cache cache, string key, T newData) where T : class
        {

            UpdateCache<T>(cache, key, newData, DateTime.Now.AddMinutes(60), System.Web.Caching.Cache.NoSlidingExpiration);

        }

        public static T GetFromCache<T>(this Cache cache, string key, Func<T> loadFunction, DateTime absoluteExpiration, TimeSpan slidingExpiration, bool allowNullEntries = false) where T : class
        {

            object obj = cache[key];



            T data;

            // If there isn't such key in the cache

            if (obj == null)
            {

                data = loadFunction();

                if (data == null && allowNullEntries)
                {

                    cache.Insert(key, new object(), null, absoluteExpiration, slidingExpiration);

                    return null;

                } //If query'result is T type item

                else if (data != null)
                {

                    cache.Insert(key, data, null, absoluteExpiration, slidingExpiration);

                }

                return data;

            }



            //If there is the key's element in the cache (so cahce's value is T tpye object)

            if (obj is T)
            {

                data = obj as T;

                //If the key's value is null

                if (data == null)
                {

                    data = loadFunction();

                    
                    //If the query's result is null and we afford to store null, then we have to put a object, because it not afford to store null

                    if (data == null && allowNullEntries)
                    {

                        cache.Insert(key, new object(), null, absoluteExpiration, slidingExpiration);

                        return null;

                    } // If the query'result is T tpye object

                    else if (data != null)
                    {

                        cache.Insert(key, data, null, absoluteExpiration, slidingExpiration);

                    }

                }

                return data;

            } // If there is a key in the cahce, but it is object

            else
            {

                data = loadFunction();

                
                //If the query's result is null and we afford to stroe null, the we have to put a obcjet, because  it not afford to store null

                if (data == null && allowNullEntries)
                {

                    cache.Insert(key, new object(), null, absoluteExpiration, slidingExpiration);

                    return null;

                } // If the query'result is T type object

                else if (data != null)
                {

                    cache.Insert(key, data, null, absoluteExpiration, slidingExpiration);

                }

                return data;

            }



        }

        public static void UpdateCache<T>(this Cache cache, string key, T newData, DateTime absoluteExpiration, TimeSpan slidingExpiration) where T : class
        {

            T data = cache[key] as T;

            if (data != null)
            {

                cache.Remove(key);

                //data = newData;

                cache.Insert(key, newData, null, absoluteExpiration, slidingExpiration);

            }

            else
            {

                cache.Insert(key, newData, null, absoluteExpiration, slidingExpiration);

            }

        }

    }

     internal static class Constants
    {
        internal static string ROUTE_NAME = "Localization";
       internal static string ROUTE_PARAMNAME_LANG = "lang";
  }

}