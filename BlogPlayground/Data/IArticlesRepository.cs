using BlogPlayground.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlayground.Data
{
    public interface IArticlesRepository
    {
        Task<List<Article>> GetAll();
        Task<Article> GetOne(int id);
        void Add(Article article);
        void Remove(Article article);
        Task SaveChanges();
    }
}
