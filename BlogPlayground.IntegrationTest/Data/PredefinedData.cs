using BlogPlayground.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogPlayground.IntegrationTest.Data
{
    public static class PredefinedData
    {
        public static string Password = @"!Covfefe123";

        public static ApplicationUser[] Profiles = new[] {
            new ApplicationUser { Email = "tester@test.com", UserName = "tester@test.com", FullName = "Tester" },
            new ApplicationUser { Email = "author@test.com", UserName = "author@test.com", FullName = "Tester" }
        };

        public static Article[] Articles = new[] {
            new Article { ArticleId = 111, Title = "Test Article 1", Abstract = "Abstract 1", Contents = "Contents 1", CreatedDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(60)) },
            new Article { ArticleId = 222, Title = "Test Article 2", Abstract = "Abstract 2", Contents = "Contents 2", CreatedDate = DateTime.Now }
        };
    }
}
