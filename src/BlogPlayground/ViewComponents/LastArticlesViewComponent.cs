using BlogPlayground.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlayground.ViewComponents
{
    public class LastArticlesViewComponent: ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public LastArticlesViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int howMany = 2)
        {
            var lastArticles = await _context.Article
                                            .OrderByDescending(a => a.CreatedDate)
                                            .Take(howMany)
                                            .ToListAsync();
            return View(lastArticles);
        }
    }
}
