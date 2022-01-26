using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetWikiV2.MVC.Models.DB
{
    public class Category
    {
        public Category() { Pages = new List<Page>(); Subcategories = new List<Category>(); }
        public Category(string name)
        {
            Name = name;
            Pages = new List<Page>();
            Subcategories = new List<Category>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Page> Pages { get; set; }
        public int? ParentId { get; set; }
        public Category Parent { get; set; }
        public List<Category> Subcategories { get; set; }
    }
}
