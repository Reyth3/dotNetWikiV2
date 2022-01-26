using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetWikiV2.MVC.Models.DB
{
    public class WikiUser : IdentityUser<Guid>
    {
        public WikiUser() { }

        public WikiUser(string username) :base(username)
        {
            Changes = new List<EditEntry>();
            Comments = new List<Comment>();
        }

        public virtual List<EditEntry> Changes { get; set; }
        public virtual List<Comment> Comments { get; set; }


        [NotMapped]
        public string AvatarUrl { get { return $"https://avatars.dicebear.com/api/identicon/wikiuser-{Id}.svg"; } }
    }
}
