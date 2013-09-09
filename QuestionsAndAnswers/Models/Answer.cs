using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace QuestionsAndAnswers.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessageResourceName = "YouHaveToGiveContentForAnAnswer", ErrorMessageResourceType = typeof(Resources.Global))]
        [StringLength(25000, ErrorMessageResourceName = "YouHaveToGiveProperLengthContentForAnAnswer", ErrorMessageResourceType = typeof(Resources.Global), MinimumLength = 25)] 
        public string Content { get; set; }

        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

    }
}
