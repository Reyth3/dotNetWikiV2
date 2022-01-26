using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetWikiV2.MVC.Models.DB
{
    public class Page
    {
        public Page() { Changes = new List<EditEntry>(); Comments = new List<Comment>(); }

        public Page(string title, string content, WikiUser author)
        {
            PageId = SlugUtil.Slugify(title);
            Title = title;
            Content = content;

            Changes = new List<EditEntry>();
            Changes.Add(new EditEntry()
            {
                Author = author,
                Description = "Created this page.",
                Timestamp = DateTimeOffset.Now
            });
        }

        public string PageId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<EditEntry> Changes { get; set; }
        public virtual List<Comment> Comments { get; set; }

        public static class SlugUtil
        {
            public static string Slugify(string input)
            {
                if (string.IsNullOrEmpty(input))
                {
                    throw new ArgumentNullException("input");
                }

                var stringBuilder = new StringBuilder();
                foreach (char c in input.ToLower().ToArray())
                {
                    if (Char.IsLetterOrDigit(c))
                    {
                        stringBuilder.Append(c);
                    }
                    else if (c == ' ')
                    {
                        stringBuilder.Append("-");
                    }
                }

                return stringBuilder.ToString().ToLower();
            }
        }
    }
}
