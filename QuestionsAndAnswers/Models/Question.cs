using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace QuestionsAndAnswers.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "YouHaveToGiveTitleForAQuestion", ErrorMessageResourceType = typeof(Resources.Global))]
        [StringLength(120, ErrorMessageResourceName = "YouHaveToGiveProperLengthTitleForAQuestion", ErrorMessageResourceType = typeof(Resources.Global), MinimumLength = 12)]
        public string Title { get; set; }

        [Required(ErrorMessageResourceName = "YouHaveToGiveContentForAQuestion", ErrorMessageResourceType = typeof(Resources.Global))]
        [StringLength(25000, ErrorMessageResourceName = "YouHaveToGiveProperLengthContentForAQuestion", ErrorMessageResourceType = typeof(Resources.Global), MinimumLength = 25)] 
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

        public virtual ICollection<QuestionHasTag> QuestionHasTags { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}
