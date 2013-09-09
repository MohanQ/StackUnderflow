using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;

namespace QuestionsAndAnswers.ControllersModel
{
    public class GetQuestionModel
    {
        /// <summary>
        /// Actual questions
        /// </summary>
        public Question CurrentQuestion { get; set; }

        /// <summary>
        /// Actual questions's vote
        /// </summary>
        public int Vote { get; set; }

        /// <summary>
        /// The questioner's profil
        /// </summary>
        public UserProfile QuestionUser { get; set; }

        /// <summary>
        /// The tags for the questions
        /// </summary>
        public List<Tag> QuestionTags { get; set; }
        /// <summary>
        /// Answers for the questions
        /// </summary>
        public List<Answer> Answers { get; set; }

        public int AllAnswersNumber { get; set; }
        /// <summary>
        /// Votes for the answers
        /// </summary>
        public Dictionary<Answer,int> AnswerVotes { get; set; }

        /// <summary>
        /// Responders' name
        /// </summary>
        public Dictionary<Answer,UserProfile> AnswerUser { get; set; }

        /// <summary>
        /// Actual answer
        /// </summary>
        public Answer ActualAnswer { get; set; }

    }
}