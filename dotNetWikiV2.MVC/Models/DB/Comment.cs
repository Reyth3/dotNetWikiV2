using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetWikiV2.MVC.Models.DB
{
    public class Comment
    {
        public Comment() { }

        [Key]
        public Guid CommentId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Guid AuthorId { get; set; }
        public WikiUser Author { get; set; }

        public string PageId { get; set; }
        public Page Page { get; set; }
    }
}
