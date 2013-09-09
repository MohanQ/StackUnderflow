using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using QuestionsAndAnswers.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace QuestionsAndAnswers.Models
{
    public class QaAContext : DbContext
    {
        public DbSet<Answer> Answers { get; set; }
        public DbSet<AnswerHasVote> AnswerHasVotes { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionHasTag> QuestionHasTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<QuestionHasVote> QuestionHasVotes { get; set; }
        public DbSet<TagHasUserProfile> TagHasAuthors { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserHasSubscribe> UserHasSubscribes { get; set; }
        public DbSet<EmailIdentifier> EmailIdentifiers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            
        }
    }
}
