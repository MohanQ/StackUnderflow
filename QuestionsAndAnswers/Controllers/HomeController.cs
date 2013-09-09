using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuestionsAndAnswers.Models;
using QuestionsAndAnswers.Managers;
using QuestionsAndAnswers.ControllersModel;
using WebMatrix.WebData;
using PagedList;
using PagedList.Mvc;
using DevTrends.MvcDonutCaching;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using QuestionsAndAnswers.Mailers;
using Mvc.Mailer;
using OpenPop.Mime;
using System.Security.Cryptography;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace QuestionsAndAnswers.Controllers
{

    public class HomeController : Controller
    {

        private IUserMailer _userMailer = new UserMailer();
        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }

        #region Subroutines for the main page

        /// <summary>
        /// Displays the home page
        /// </summary>
        /// <returns></returns>
        [DonutOutputCache(Duration = 60)]
        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// Displays the login header for the layout
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginHeader()
        {
            
            return PartialView("_LoginHeader");
        }

        /// <summary>
        /// List the latest questions, their votes and the number of answers for them
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexQuestions()
        {

            List<Question> question = QuestionManager.GetXLatestQuestion(10);
            
            //Create the viewmodel from the questions
            List<QuestionIndexModel> qim = MyHelpers.ToQuestionIndexModel(question);
            
         
            return PartialView("_ShowQuestions", qim);
        }

        #endregion


        #region Subroutines for the Questions

        /// <summary>
        /// Displays the questions which contains one of the favorite tags of the user
        /// </summary>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        public ActionResult QuestionsToUser()
        {
            ViewBag.QuestionTitle = Resources.Global.LatestUnansweredQuestionsWithYourFavoriteSpeciality;
            var questions = QuestionManager.GetQuestionsFitToUser(WebSecurity.CurrentUserId);
            if (questions.Count == 0)
            {
                return new EmptyResult();
            }
            else
            {
                return PartialView("_ShowQuestions", MyHelpers.ToQuestionIndexModel(QuestionManager.GetQuestionsFitToUser(WebSecurity.CurrentUserId)));
            }
        }        

        /// <summary>
        /// Displays all questions at the site by pages
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        public ActionResult GetAllQuestions(int? page)
        {
            var actpage = page ?? 1;
            int total;
            var questions = QuestionManager.AllQuestionsToPagedList(actpage, 10, out total);
            ViewBag.OnePageOfProducts = new StaticPagedList<Question>(questions, actpage, 10, total);
            ViewBag.Page = actpage;
            ViewBag.MorePage = total - (ViewBag.OnePageOfProducts as IEnumerable<Question>).Count();
            return View();
        }

        /// <summary>
        /// Displays questions for the actual page
        /// </summary>
        /// <param name="page">Page number(actual)</param>
        /// <param name="questions">List of questions</param>
        /// <returns></returns>
        public ActionResult GetAllQuestionsResult(int page, IPagedList<Question> questions)
        {
            return PartialView("_ShowQuestions", MyHelpers.ToQuestionIndexModel(questions));
        }

        /// <summary>
        /// Lists the questions via Ajax
        /// </summary>
        /// <param name="prefix">Prefix of the question's title which we look for in tags</param>
        /// <returns></returns>
        public ActionResult FindQuestion(string prefix)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            //Remove the characters which can't included in the tags
            prefix = rgx.Replace(prefix, "");
            //Get the wordlist
            var wordlist = prefix.Split(' ');

            List<Question> quesList = null;
            foreach (var item in wordlist)
            {
                if (quesList != null)
                {

                    var temp = QuestionManager.AllQuestionToTag(item);
                    if (temp != null)
                    {
                        quesList = quesList.Concat(temp).ToList();
                    }
                }
                else
                {
                    quesList = QuestionManager.AllQuestionToTag(item);
                }
            }
            //Remove the duplications from the questions
            List<Question> questions = null;
            if(quesList!=null)
                 questions = quesList.GroupBy(q => q.Id).Select(g => g.First()).ToList();

            //Create a KeyValue list
            var allQues = new List<KeyValuePair<int, string>>();
            if (questions != null)
            {
                foreach (var item in questions)
                {
                    allQues.Add(new KeyValuePair<int, string>(item.Id, item.Title));
                }


                var result = (from l in allQues select l).ToList();

                result = (from l in allQues
                          orderby l.Value
                          select l).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        

        /// <summary>
        /// Edit questions
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public ActionResult EditQuestion(string actId)
        {
            int actualId = Int32.Parse(actId);
            //Get the actual question
            var model = QuestionManager.GetQuestion(actualId);

            if (model.UserId == WebSecurity.CurrentUserId)
            {
                return View(model);
            }
            //If the user not eligble to edit the question, we returns to home page
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Edit question form post method
        /// </summary>
        /// <param name="data">Question model</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditQuestion(Question data, string QuesID)
        {
            data.Id = Int32.Parse(QuesID);
            data.UserId = WebSecurity.CurrentUserId;
            if (ModelState.IsValid)
            {
                QuestionManager.EditQuestion(data);
            }

            return RedirectToAction("Index");
        }



        /// <summary>
        /// Returns the details for a question
        /// </summary>
        /// <param name="questionid">Question ID</param>
        /// <returns></returns>
        public ActionResult GetQuestion(int id)
        {

            var actQues = QuestionManager.GetQuestion(id);
            var model = new GetQuestionModel();
            model.CurrentQuestion = actQues;

            //Get the vote of the question
            model.Vote = QuestionManager.GetVote(model.CurrentQuestion.Id);
            //List the tags of the question
            model.QuestionTags = TagManager.GetAllTagToOneQuestion(id);

            model.QuestionUser = UserManager.GetUserById(model.CurrentQuestion.UserId);


           
            return View(model);
        }

        /// <summary>
        /// Get answers for the one questions
        /// </summary>
        /// <param name="id">Question ID</param>
        /// <param name="sortby">Sort parameter</param>
        /// <returns></returns>
        public ActionResult GetAnswersForAQuestion(int id, string sortby)
        {
            AnswersViewModel model = new AnswersViewModel();


            //Get the answers which belong to this question
            model.Answers = AnswerManager.GetAllAnswerToOneQuestion(id);

            //Get the votes of the answers
            Dictionary<Answer, int> ansvotes = new Dictionary<Answer, int>();
            Dictionary<Answer, UserProfile> answeruser = new Dictionary<Answer, UserProfile>();
            foreach (var item in model.Answers)
            {
                int vote = AnswerManager.GetVote(item.Id);
                UserProfile userprof = UserManager.GetUserById(item.UserId);
                answeruser.Add(item, userprof);
                ansvotes.Add(item, vote);
            }
            //Create the viewmodel
            model.AnswerVotes = ansvotes;
            model.AnswerUser = answeruser;

            //Order the answers by the given method
            if (sortby == "time")
            {
                model.Answers = model.Answers.OrderBy(d => d.Date).ToList();

            }
            else if (sortby == "reversedtime")
            {
                model.Answers = model.Answers.OrderByDescending(d => d.Date).ToList();

            }
            else
            {
                model.Answers = model.Answers.OrderByDescending(d => model.AnswerVotes[d]).ToList();

            }

            return PartialView("_ShowAnswers", model);
        }

        /// <summary>
        /// Get numbers of answer for one question
        /// </summary>
        /// <param name="id">Question ID</param>
        /// <returns></returns>
        public int GetNumberOfAnswersForOneQuestion(int id)
        {
            int num = AnswerManager.GetAllAnswerNumberToOneQuestion(id);
            return num;
        }

        /// <summary>
        /// Create a question
        /// </summary>
        /// <returns></returns>
        [DonutOutputCache(Duration = 60)]
        public ActionResult CreateQuestion()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            //If the user not logged in, we redirect him to home page
            else
            {
                return RedirectToAction("MustLogIn");
            }
        }


        /// <summary>
        /// Create a new question
        /// </summary>
        /// <param name="data">Question object</param>
        /// <param name="tags">Tag's values</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateQuestion(Question data, string tags)
        {
            if (Request.IsAuthenticated && (!tags.Equals("")))
            {
                //Create a list for the tags
                var tagList = new List<String>();

                //Split the given string by commas
                var boxtags = tags.Split(','); 

                //If the last character is a comma, create a list without that
                if (boxtags[boxtags.Length - 1].Equals(" "))
                    tagList = boxtags.Take(boxtags.Length - 1).ToList();
                else
                    tagList = boxtags.ToList();

                //Remove all leading and trailing whitespaces from the tage
                var tagsWithoutWhitespaces = new List<String>();
                foreach (var item in tagList)
                {
                    tagsWithoutWhitespaces.Add(item.Trim());
                }

                //If the created question is valid, add a new question to the database
                if (ModelState.IsValid)
                {
                    QuestionManager.AddQuestion(data, WebSecurity.GetUserId(User.Identity.Name), tagsWithoutWhitespaces);
                }
            }
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Lists questions which related to one other. It based on the tags.
         /// </summary>
        /// <param name="questionid">Questions Id</param>
        /// <returns></returns>
        public ActionResult GetRelatedQuestions(int questionid)
        {
            //Create a partial view, which contains the related questions
            return PartialView("_GetRelatedQuestions", QuestionManager.GetRelatedQuestions(questionid));
        }



        /// <summary>
        /// Votes for a questions
        /// </summary>
        /// <param name="questionid">Question Id</param>
        /// <param name="vote">Vote: +1 or -1</param>
        /// <returns></returns>
        public ActionResult VoteForQuestion(int questionid, int vote)
        {
            if (!Request.IsAuthenticated)
            {
                //If the user is not logged in, we create an error message
                return Json(Resources.Global.YouHaveToLoginBeforeVote);
            }
            else
            {
                //Check if the user already voted for this question
                bool isvoted = VoteManager.IsVotedForQuestion(questionid, WebSecurity.GetUserId(User.Identity.Name));
                if (isvoted)
                {
                    return Json(Resources.Global.YouAlreadyVotedForThisQuestion);
                }
                //Check if the user want to vote for his own question
                if (QuestionManager.GetQuestion(questionid).UserId == WebSecurity.GetUserId(User.Identity.Name))
                {
                    return Json(Resources.Global.YouCantVoteForYourOwnQuestion);
                }
                //Store the vote
                VoteManager.Vote(questionid, WebSecurity.GetUserId(User.Identity.Name), vote);

                //Returns the new vote of the question
                return Json(QuestionManager.GetVote(questionid));

            }

        }

        #endregion

        
        #region Subroutines for the Tags

        /// <summary>
        /// List the tags to the home page
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTags()
        {
            string email = "No";
            string isAuth = "yes";
            string isVerify = "no";


            if (WebSecurity.IsAuthenticated)
            {
                var user = UserManager.GetUserById(WebSecurity.CurrentUserId);
                if (user.Email != null)
                {
                    email = user.Email;
                    if (user.IsVerified == true)
                        isVerify = "yes";
                }
                else
                {
                    email = "No";
                    if (user.IsVerified == true)
                        isVerify = "yes";
                }
            }
            else
            {
                isAuth = "no";
            }
            //We send email's info and Authentication's info
            
            ViewBag.Email = email;
            ViewBag.isAuth = isAuth;
            ViewBag.isVerify = isVerify;
            return PartialView("_GetTags", TagManager.GetAllTag());
        }

        /// <summary>
        /// Function to list the tags
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <returns></returns>
        public ActionResult FindTags(string prefix)
        {
            var tagList = TagManager.GetAllTag();

            //Create a list with the name of tags
            var tagName = new List<string>();
            foreach (var tag in tagList)
            {
                tagName.Add(tag.Name);
            }

            //Create a list with the tags and a temporary id for them
            var allTags = new List<KeyValuePair<int, string>>();
            int i = 1;
            foreach (var name in tagName)
            {
                allTags.Add(new KeyValuePair<int, string>(i, name));
                i++;
            }

            //Select the tags which contains the typed prefix
            var result = (from l in allTags
                      where l.Value.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)
                      orderby l.Value
                      select l).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Shows the page of a tag
        /// </summary>
        /// <param name="id">Tag id</param>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        public ActionResult Tag(int id, int? page)
        {
            var actpage = page ?? 1;
            int total;
            var allresult = QuestionManager.AllQuestionToOneTagToPagedList(id, actpage, 10, out total);
            //Viewbag objects
            ViewBag.OnePageOfProducts = new StaticPagedList<Question>(allresult, actpage, 10, total);
            ViewBag.Page = actpage;
            ViewBag.Tag = TagManager.GetTagById(id);
            ViewBag.MorePage = total - allresult.Count();

            return View();
        }

        /// <summary>
        /// Lists the questions which contains the given tag
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="questions">Questions</param>
        /// <returns></returns>
        public ActionResult GetQuestionsByTag(int page, IPagedList<Question> questions)
        {

            return PartialView("_ShowQuestions", MyHelpers.ToQuestionIndexModel(questions));
        }

        /// <summary>
        /// Lists the tags
        /// </summary>
        /// <returns></returns>
        public ActionResult ListAllTags()
        {
            //Manager classes
            var tags = TagManager.GetAllTag();
            //We list the tags
            var retlist = new List<TagListModel>();
            foreach (var item in tags)
            {
                var add = new TagListModel();
                add.Tag = item;
                int count;
                QuestionManager.AllQuestionToOneTagToPagedList(item.Id, 1, 1, out count);
                add.Questions = count;
                add.SubcribedUser = UserManager.AllSubcribeUsersCountToOneTag(item.Id);
                retlist.Add(add);
            }

            
            
            return View(retlist.OrderByDescending(q => q.Questions).ToList());
        }

        #endregion


        #region Subroutines for the User

        /// <summary>
        /// Shows the page of a user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        public ActionResult GetUser(int id, int? questionpage, int? answerpage)
        {
            UserControllerModel ucm = new UserControllerModel();
            ViewBag.AskedQuestionPage = questionpage ?? 1;
            ViewBag.AnsweredQuestionPage = answerpage ?? 1;

            ucm.UserProfile = UserManager.GetUserById(id);

            ucm.UserRating = UserManager.GetUserRating(id);
            ucm.SubcribedTag = UserManager.SixTagToOneUser(id);
            ViewBag.CountTag = UserManager.AllSubcribeTagsCountToOneUser(id);
            return View(ucm);
        }

        /// <summary>
        /// Shows the asked questions by a user 
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="askedquestionpage">Asked questions page number</param>
        /// <param name="answeredquestionpage">Answered questions page number</param>
        /// <returns></returns>
        public ActionResult GetAskedQuestionsByUser(int id, int askedquestionpage, int answeredquestionpage)
        {

            //Get the questions for the actual page
            int totalquestions;
            var retrievedQuestions = QuestionManager.AllQuestionToOneUserToPagedList(id, askedquestionpage, 10, out totalquestions);
            //Convert it to a StaticPagedList for the pager
            var actualQuestions = new StaticPagedList<Question>(retrievedQuestions, askedquestionpage, 10, totalquestions);

            var questionIndexModels = MyHelpers.ToQuestionIndexModel(actualQuestions);

            ViewBag.QuestionsForPager = actualQuestions;
            ViewBag.AskedQuestionPage = askedquestionpage;
            ViewBag.AnsweredQuestionPage = answeredquestionpage;
            ViewBag.UserId = id;

            return PartialView("_ShowAskedQuestions", questionIndexModels);
        }

        /// <summary>
        /// Shows the answered questions by a user 
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="askedquestionpage">Asked questions page number</param>
        /// <param name="answeredquestionpage">Answered questions page number</param>
        public ActionResult GetAnsweredQuestionsByUser(int id, int askedquestionpage, int answeredquestionpage)
        {
            //Get the questions for the actual page
            int totalquestions;
            var retrievedQuestions = QuestionManager.AnsweredQuestionsToPagedList(id, answeredquestionpage, 5, out totalquestions);
            //Convert it to a StaticPagedList for the pager
            var actualQuestions = new StaticPagedList<Question>(retrievedQuestions, answeredquestionpage, 5, totalquestions);

            var model=MyHelpers.ToGetQuestionModels(actualQuestions, id);

            ViewBag.QuestionsForPager = actualQuestions;
            ViewBag.AskedQuestionPage = askedquestionpage;
            ViewBag.AnsweredQuestionPage = answeredquestionpage;
            ViewBag.UserId = id;

            return PartialView("_ShowAnsweredQuestions", model);
        }


        /// <summary>
        /// Lists the users
        /// </summary>
        /// <returns></returns>
        public ActionResult ListAllUsers()
        {
            //Manager classes
            var users = UserManager.GetAllUsers();
            //List of the users
            var retlist = new List<UserListModel>();
            foreach (var item in users)
            {
                var add = new UserListModel();
                add.User = item;
                add.Rating = UserManager.GetUserRating(item.UserId);
                retlist.Add(add);
            }
            return View(retlist.OrderByDescending(q => q.Rating).ToList());
        }

        /// <summary>
        /// Register email address
        /// </summary>
        /// <returns></returns>
        public ActionResult EmailRegister()
        {
            if (!WebSecurity.IsAuthenticated)
            {
                return RedirectToAction("MustLogIn");
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Register email for a user
        /// </summary>
        /// <param name="data">User profile</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmailRegister(UserProfile data)
        {
            
            if (ModelState.IsValid)
            {
                data.UserId = WebSecurity.CurrentUserId;
                data.UserName = WebSecurity.CurrentUserName;
                UserManager.AddEmail(data);
                string key = MyHelpers.Encode(data.Email);
                UserMailer.UserVerification(data,key).SendAsync(); 
                return View("EmailSent");
            }
            return RedirectToAction("EmailRegister");
        }

        /// <summary>
        /// Process the email verification method
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="token">Generated token</param>
        /// <returns></returns>
        public ActionResult EmailVerification(int id, string token)
        {
            var userprofile = UserManager.GetUserById(id);
            if (MyHelpers.Encode(userprofile.Email) == token)
            {
                UserManager.VerifyEmail(id);
                return View("SuccessfulVerification");
            }
            else
            {
                return View("UnsuccessfulVerification");
            }
        }

        #endregion


        #region Subroutines for the Answer

        /// <summary>
        /// Votes for answers
        /// </summary>
        /// <param name="answerid">Answer Id</param>
        /// <param name="vote">Vote</param>
        /// <returns></returns>
        public ActionResult VoteForAnswer(int answerid, int vote)
        {
            if (!Request.IsAuthenticated)
            {
                //Check if the user logged in
                return Json(Resources.Global.YouHaveToLoginBeforeVote);
            }
            else
            {
                //Check if the user already voted for this answer
                bool isVoted = VoteManager.IsVotedForAnswer(answerid, WebSecurity.GetUserId(User.Identity.Name));
                if (isVoted)
                {
                    return Json(Resources.Global.YouAlreadyVotedForThisAnswer);
                }


                //Check if the user want to vote for his own question
                if (AnswerManager.GetAnswer(answerid).UserId == WebSecurity.GetUserId(User.Identity.Name))
                {
                    return Json(Resources.Global.YouCantVoteForYourOwnAnswer);
                }
                //Store the vote
                VoteManager.VoteAnswer(answerid, WebSecurity.GetUserId(User.Identity.Name), vote);

                return Json(AnswerManager.GetVote(answerid));

            }

        }


        /// <summary>
        /// Adds an answer to the site
        /// </summary>
        /// <param name="data">Data of the answer</param>
        /// <param name="questionID">Question id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public void AddAnswer(GetQuestionModel data, string questionID)
        {
            //Id of the actual question
            int actid = Int32.Parse(questionID);
            int userid = 0;
            var model_state = ModelState.IsValid;
            if (Request.IsAuthenticated)
                userid = WebSecurity.CurrentUserId;
            else
                model_state = false;
            if (model_state)
            {
                AnswerManager.AddAnswer(data.ActualAnswer, actid, userid);
            }

            return;

        }



        /// <summary>
        /// Edits an answer
        /// </summary>
        /// <param name="actId">Answer id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditAnswer(string actId)
        {
            int actualId = Int32.Parse(actId);

            var model = AnswerManager.GetAnswer(actualId);
            //If the user is authorized to edit this answer, it returns the edit page
            if (model.UserId == WebSecurity.CurrentUserId)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        /// <summary>
        /// Edit post submit function
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditAnswer(Answer data, string AnsId)
        {
            data.Id = Int32.Parse(AnsId);
            data.UserId = WebSecurity.CurrentUserId;
            if (ModelState.IsValid)
            {
                AnswerManager.EditAnswer(data);
            }
            return RedirectToAction("Index");
        }

        #endregion


        #region Others

        /// <summary>
        /// Shows a page to the user if he have to log in for a function
        /// </summary>
        [DonutOutputCache(Duration = 60)]
        public ActionResult MustLogIn()
        {
            return View();
        }


        #endregion

        //******************************************************************************************
        //******************************************************************************************

        /// <summary>
        /// Subcribe to tag(s) via Ajax
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SubcribeToTags(string tags)
        {
            //Checking the sign in
            if (Request.IsAuthenticated)
            {

                int user=WebSecurity.CurrentUserId;
                List<Tag> list_tag=null;
                char last_charachter='d';
                char first_charachter='d';
                //Check the last charachter and first charachter
                if (tags.Length > 1)
                {
                     last_charachter = tags[tags.Length - 1];
                     first_charachter = tags[0];
                }

                //In the case: using contains (*string*)
                if (last_charachter == '*' && first_charachter=='*')
                {
                    tags=tags.Remove(0,1);
                    tags=tags.Remove(tags.Length-1);
                    list_tag = TagManager.GetContainsTag(tags);
                    if (list_tag.Count > 0)
                    {
                        //Subcribe
                        UserManager.SubcribeToMoreTags(list_tag, user);
                        //concatenate the tags' name
                        string subcribed_tags = "";
                        foreach (var item in list_tag)
                        {
                            subcribed_tags = subcribed_tags + item.Name + "; ";   
                        }
                        return Json(subcribed_tags, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Problem", JsonRequestBehavior.AllowGet);
                    }
                }
                //In the case: using startwith (string*)
                else if (last_charachter == '*')
                {
                    tags = tags.Remove(tags.Length - 1);
                    list_tag = TagManager.GetStartwithTag(tags);
                    if (list_tag.Count > 0)
                    {
                        //Subcribe
                        UserManager.SubcribeToMoreTags(list_tag, user);
                        //concatenate the tags' name
                        string subcribed_tags = "";
                        foreach (var item in list_tag)
                        {
                            subcribed_tags = subcribed_tags + item.Name + "; ";
                        }
                        return Json(subcribed_tags, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Problem", JsonRequestBehavior.AllowGet);
                    }
                }
                //In the case: using endwith (*string)
                else if (first_charachter == '*')
                {
                    tags = tags.Remove(0,1);
                    list_tag = TagManager.GetEndwithTag(tags);
                    if (list_tag.Count > 0)
                    {
                        //Subcribe
                        UserManager.SubcribeToMoreTags(list_tag, user);
                        //concatenate the tags' name
                        string subcribed_tags = "";
                        foreach (var item in list_tag)
                        {
                            subcribed_tags = subcribed_tags + item.Name + "; ";
                        }
                        return Json(subcribed_tags, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Problem", JsonRequestBehavior.AllowGet);
                    }
                }

                //In the case, there is only a element
                else
                {

                    //Search the tag from name
                    var tag = TagManager.GetTagByName(tags);
                    //If the tag exist
                    if (tag != null)
                    {
                        UserManager.SubcribeToTag(tag.Id, user);
                        return Json(tags, JsonRequestBehavior.AllowGet);
                    }
                    //If the tag doesn't exist
                    else
                    {
                        return Json("Problem", JsonRequestBehavior.AllowGet);
                    }
                }
            }

            else
            {
                return Json("Problem", JsonRequestBehavior.AllowGet);
            }

           
        }


        /// <summary>
        /// Get the subcribed users, who subcribe to the tag
        /// </summary>
        /// <param name="tagid">TagID</param>
        /// <returns></returns>
        public ActionResult GetSubcribedUser(int tagid)
        {
            //TagName
            ViewBag.tagname = (TagManager.GetTagById(tagid)).Name;
            var model = UserManager.AllSubcribeUserToOneTag(tagid);

            return PartialView("_GetSubcribedUser", model);
        }

    }


    
}


