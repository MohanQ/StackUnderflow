using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;

namespace QuestionsAndAnswers.ControllersModel
{
    /// <summary>
    /// Model for the tag, which on the index
    /// </summary>
    public class TagListModel
    {
        /// <summary>
        /// The tag
        /// </summary>
        public Tag Tag { get; set; }

        /// <summary>
        /// Number of the question for the tag
        /// </summary>
        public int Questions { get; set; }

        /// <summary>
        /// Users'count, who subcibe to tag
        /// </summary>
        public int SubcribedUser { get; set; }
    }
}