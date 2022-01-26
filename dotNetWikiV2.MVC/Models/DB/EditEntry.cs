using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetWikiV2.MVC.Models.DB
{
    public class EditEntry
    {
        [Key]
        public int EditEntryId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        [Required, MaxLength(150)]
        public string Description { get; set; }

        public string PageId { get; set; }
        public Page Page { get; set; }

        public Guid WikiUserId { get; set; }
        public WikiUser Author { get; set; }
    }
}
