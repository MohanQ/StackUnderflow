using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using QuestionsAndAnswers.Managers;

namespace QuestionsAndAnswers.Helpers
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// Render all the users in partialView (We are using this feature in Jquery Reveal)
        /// </summary>
        /// <param name="html">HTML extended helper</param>
        /// <param name="id">User ID</param>
        /// <returns></returns>
        public static MvcHtmlString RenderAllTags(this HtmlHelper html, int id)
        {
            var model = UserManager.AllSubcribeTagToOneUser(id);
            html.ViewBag.UserName = UserManager.GetUserById(id).UserName;
            return html.Partial("GetAllTagToOneUser", model);

        }
    }
}