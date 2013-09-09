using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;

namespace QuestionsAndAnswers.ControllersModel
{
    /// <summary>
    /// For the index's questions
    /// </summary>
    public class QuestionIndexModel
    {
        /// <summary>
        /// The questions
        /// </summary>
        public Question Question { get; set; }

        /// <summary>
        /// Votes for the questions
        /// </summary>
        public int Vote { get; set; }

        /// <summary>
        /// Number of answers of the question
        /// </summary>
        public int Answers { get; set; }
        
        /// <summary>
        /// The tags for the questions
        /// </summary>
        public List<Tag> Tags { get; set; }


        /// <summary>
        /// The author of the question
        /// </summary>
        public UserProfile QuestionUser { get; set; }
    }
}