using BlogPlayground.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlayground.ViewComponents
{
    public class LatestArticlesViewComponent: ViewComponent
    {
        private readonly IArticlesRepository _repository;

        public LatestArticlesViewComponent(IArticlesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int howMany = 2)
        {
            return View(await _repository.GetLatest(howMany));
        }
    }
}
