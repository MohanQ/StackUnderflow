using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;
using QuestionsAndAnswers.Managers;

namespace QuestionsAndAnswers.ControllersModel
{
    public class UserControllerModel
    {
        /// <summary>
        /// The user's profil
        /// </summary>
        public UserProfile UserProfile { get; set; }

        /// <summary>
        /// The user's rating
        /// </summary>
        public int UserRating { get; set; }

        /// <summary>
        /// List of the subcribed tag
        /// </summary>
        public List<Tag> SubcribedTag { get; set; }

    }
}