﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace QuestionsAndAnswers.Models
{
    public class TagHasUserProfile
    {
        [Key]
        public int Id { get; set; }

        public int TagId { get; set; }

        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile UserProfile { get; set; }

    }
}
