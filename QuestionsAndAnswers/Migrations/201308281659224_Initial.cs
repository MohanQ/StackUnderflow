namespace QuestionsAndAnswers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Content = c.String(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Question", t => t.QuestionId)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.QuestionId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Question",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 120),
                        Content = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                   "dbo.UserProfile",
                   c => new
                   {
                       UserId = c.Int(nullable: false, identity: true),
                       UserName = c.String(nullable: false, maxLength: 120),
                       Email = c.String(),
                       IsVerified = c.Boolean(),
                   })
                   .PrimaryKey(t => t.UserId);

            //AddColumn("dbo.UserProfile", "Email", c=>c.String());
            //AddColumn("dbo.UserProfile", "IsVerified", c => c.Boolean());
          
            
            CreateTable(
                "dbo.UserHasSubscribe",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tag", t => t.TagId)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.TagId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestionHasTag",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Question", t => t.QuestionId)
                .ForeignKey("dbo.Tag", t => t.TagId)
                .Index(t => t.QuestionId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.AnswerHasVote",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        AnswerId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .ForeignKey("dbo.Answer", t => t.AnswerId)
                .Index(t => t.UserId)
                .Index(t => t.AnswerId);
            
            CreateTable(
                "dbo.Badge",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.QuestionHasVote",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rating = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .ForeignKey("dbo.Question", t => t.QuestionId)
                .Index(t => t.UserId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.TagHasUserProfile",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tag", t => t.TagId)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.TagId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EmailIdentifier",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Hash = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TagHasUserProfile", new[] { "UserId" });
            DropIndex("dbo.TagHasUserProfile", new[] { "TagId" });
            DropIndex("dbo.QuestionHasVote", new[] { "QuestionId" });
            DropIndex("dbo.QuestionHasVote", new[] { "UserId" });
            DropIndex("dbo.Badge", new[] { "UserId" });
            DropIndex("dbo.AnswerHasVote", new[] { "AnswerId" });
            DropIndex("dbo.AnswerHasVote", new[] { "UserId" });
            DropIndex("dbo.QuestionHasTag", new[] { "TagId" });
            DropIndex("dbo.QuestionHasTag", new[] { "QuestionId" });
            DropIndex("dbo.UserHasSubscribe", new[] { "UserId" });
            DropIndex("dbo.UserHasSubscribe", new[] { "TagId" });
            DropIndex("dbo.Question", new[] { "UserId" });
            DropIndex("dbo.Answer", new[] { "UserId" });
            DropIndex("dbo.Answer", new[] { "QuestionId" });
            DropForeignKey("dbo.TagHasUserProfile", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.TagHasUserProfile", "TagId", "dbo.Tag");
            DropForeignKey("dbo.QuestionHasVote", "QuestionId", "dbo.Question");
            DropForeignKey("dbo.QuestionHasVote", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.Badge", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.AnswerHasVote", "AnswerId", "dbo.Answer");
            DropForeignKey("dbo.AnswerHasVote", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.QuestionHasTag", "TagId", "dbo.Tag");
            DropForeignKey("dbo.QuestionHasTag", "QuestionId", "dbo.Question");
            DropForeignKey("dbo.UserHasSubscribe", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.UserHasSubscribe", "TagId", "dbo.Tag");
            DropForeignKey("dbo.Question", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.Answer", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.Answer", "QuestionId", "dbo.Question");
            DropTable("dbo.EmailIdentifier");
            DropTable("dbo.TagHasUserProfile");
            DropTable("dbo.QuestionHasVote");
            DropTable("dbo.Badge");
            DropTable("dbo.AnswerHasVote");
            DropTable("dbo.QuestionHasTag");
            DropTable("dbo.Tag");
            DropTable("dbo.UserHasSubscribe");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Question");
            DropTable("dbo.Answer");
        }
    }
}
