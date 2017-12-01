using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlogPlayground.Data;
using BlogPlayground.Services;
using BlogPlayground.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogPlayground.Controllers
{
    [Produces("application/json")]
    [Route("api/articles")]
    public class ArticlesApiController : Controller
    {
        private readonly IArticlesRepository _articlesRepository;
        private readonly IRequestUserProvider _requestUserProvider;

        public ArticlesApiController(IArticlesRepository articlesRepository, IRequestUserProvider requestUserProvider)
        {
            _articlesRepository = articlesRepository;
            _requestUserProvider = requestUserProvider;
        }

        [HttpGet()]
        public async Task<IEnumerable<Article>> GetArticles()
        {
            return await _articlesRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetArticle(int id)
        {
            var article = await _articlesRepository.GetOne(id);
            if (article == null) return NotFound();

            return Ok(article);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> AddArticle([FromBody]Article article)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            article.AuthorId = _requestUserProvider.GetUserId();
            article.CreatedDate = DateTime.Now;
            _articlesRepository.Add(article);
            await _articlesRepository.SaveChanges();
            return Ok(article);
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteArticle(int id)
        {
            var article = await _articlesRepository.GetOne(id);
            if (article == null) return NotFound();

            _articlesRepository.Remove(article);
            await _articlesRepository.SaveChanges();

            return NoContent();
        }        
    }
}