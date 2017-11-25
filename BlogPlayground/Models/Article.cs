using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlayground.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Abstract { get; set; }
        [Required]
        public string Contents { get; set; }
        public DateTime CreatedDate { get; set; }

        //EF relationships should pick this relationship between Article and ApplicationUser tables: http://ef.readthedocs.io/en/latest/modeling/relationships.html     
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}
