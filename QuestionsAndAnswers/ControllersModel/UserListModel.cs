using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;

namespace QuestionsAndAnswers.ControllersModel
{
    
    public class UserListModel
    {
        /// <summary>
        /// The User
        /// </summary>
        public UserProfile User { get; set; }

        /// <summary>
        /// The user's rating
        /// </summary>
        public int Rating { get; set; }
    }
}