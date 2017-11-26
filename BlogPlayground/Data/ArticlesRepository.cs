using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogPlayground.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPlayground.Data
{
    public class ArticlesRepository : IArticlesRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticlesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Article>> GetAll() =>
            _context.Article.Include(a => a.Author).ToListAsync();

        public Task<List<Article>> GetLatest(int num) =>
            _context.Article.Include(a => a.Author).OrderByDescending(a => a.CreatedDate).Take(num).ToListAsync();

        public Task<Article> GetOne(int id) =>
             _context.Article.Include(a => a.Author).SingleOrDefaultAsync(m => m.ArticleId == id);

        public void Add(Article article) =>
            _context.Article.Add(article);

        public void Remove(Article article) => 
            _context.Article.Remove(article);        

        public Task SaveChanges() => 
            _context.SaveChangesAsync();
    }
}
