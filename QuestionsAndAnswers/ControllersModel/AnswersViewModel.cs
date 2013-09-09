using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;

namespace QuestionsAndAnswers.ControllersModel
{
    public class AnswersViewModel
    {
        
        /// <summary>
        /// Answers for the questions
        /// </summary>
        public List<Answer> Answers { get; set; }

        /// <summary>
        /// Votes for the answers
        /// </summary>
        public Dictionary<Answer, int> AnswerVotes { get; set; }

        /// <summary>
        /// Responders'name
        /// </summary>
        public Dictionary<Answer, UserProfile> AnswerUser { get; set; }
        /// <summary>
        /// Actual answer
        /// </summary>
        public Answer ActualAnswer { get; set; }

    }
}