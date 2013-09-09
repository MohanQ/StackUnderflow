using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuestionsAndAnswers.Models
{
    public class EmailIdentifier
    {
        [Key]
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int UserId { get; set; }

        public string Hash { get; set; }
    }
}