using BlogPlayground.Data;
using BlogPlayground.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlayground.IntegrationTest.Data
{
    public class DatabaseSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DatabaseSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Seed()
        {
            // Add all the predefined profiles using the predefined password
            foreach (var profile in PredefinedData.Profiles)
            {
                await _userManager.CreateAsync(profile, PredefinedData.Password);
                // Set the AuthorId navigation property
                if (profile.Email == "author@test.com")
                {
                    PredefinedData.Articles.ToList().ForEach(a => a.AuthorId = profile.Id);
                }           
            }
            
            // Add all the predefined articles
            _context.Article.AddRange(PredefinedData.Articles);
            _context.SaveChanges();
        }
    }
}
