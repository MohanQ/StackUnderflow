using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;

namespace QuestionsAndAnswers.Managers
{
    public static class TagManager
    {

        /// <summary>
        /// Get the tag based on ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Tag GetTagById(int id)
        {
            using (var db=new QaAContext())
            {
                var q = HttpContext.Current.Cache.GetFromCache("GetTag"+id, () => (from t in db.Tags
                         where t.Id == id
                         select t).SingleOrDefault());
                return q;
            }
        }

        /// <summary>
        /// List the all tag
        /// </summary>
        /// <returns></returns>
        public static List<Tag> GetAllTag()
        {
            using (var db = new QaAContext())
            {
                var q = HttpContext.Current.Cache.GetFromCache("Tags", () => ( from t in db.Tags select t).ToList());
                return q;
            }
        }

        /// <summary>
        /// Get the tags based on question ID
        /// </summary>
        /// <param name="id">question Id</param>
        /// <returns></returns>
        public static List<Tag> GetAllTagToOneQuestion(int id)
        {
            using (var db=new QaAContext())
            {
            
                var q = db.QuestionHasTags.Where(t => t.QuestionId == id).Select(t => t.Tag).ToList();
                return q;
            }
        }


        /// <summary>
        /// Get the Tag by tagname
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Tag GetTagByName(string name)
        {
            using (var db = new QaAContext())
            {
                var q = (from t in db.Tags where t.Name == name select t).SingleOrDefault();
                return q;
            }
        }


        /// <summary>
        /// Get the list of tags, where the tag contains tagpart
        /// </summary>
        /// <param name="tagpart"></param>
        /// <returns></returns>
        public static List<Tag> GetContainsTag(string tagpart)
        {
            using (var db = new QaAContext())
            {
                var q = (from t in db.Tags where t.Name.Contains(tagpart) select t).ToList();
                return q;
            }
        }


        /// <summary>
        /// Get the list of tags, where the tag starts with tagpart
        /// </summary>
        /// <param name="tagpart"></param>
        /// <returns></returns>
        public static List<Tag> GetStartwithTag(string tagpart)
        {
            using (var db = new QaAContext())
            {
                var q = (from t in db.Tags where t.Name.StartsWith(tagpart) select t).ToList();
                return q;
            }
        }

        /// <summary>
        /// Get the list of tags, where the tag starts with tagpart
        /// </summary>
        /// <param name="tagpart"></param>
        /// <returns></returns>
        public static List<Tag> GetEndwithTag(string tagpart)
        {
            using (var db = new QaAContext())
            {
                var q = (from t in db.Tags where t.Name.EndsWith(tagpart) select t).ToList();
                return q;
            }
        }
    }
}