using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QuestionsAndAnswers.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<QuestionHasTag> QuestionHasTags { get; set; }

        public virtual ICollection<UserHasSubscribe> UserHasSubscribes { get; set; }
    }
}
