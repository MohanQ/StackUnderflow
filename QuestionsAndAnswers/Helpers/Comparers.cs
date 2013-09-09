using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuestionsAndAnswers.Models;

namespace QuestionsAndAnswers
{
    /// <summary>
    /// Get the IEqualityComparer to question comparer
    /// </summary>
    public class QuestionComparer : IEqualityComparer<Question>
    {
        public bool Equals(Question x, Question y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Id == y.Id;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Question question)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(question, null)) return 0;

            return  question.Id.GetHashCode();
        }
    }
}