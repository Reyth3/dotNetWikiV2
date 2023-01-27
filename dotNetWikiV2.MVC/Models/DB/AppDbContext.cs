using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetWikiV2.MVC.Models.DB
{
    public class AppDbContext : IdentityDbContext<WikiUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions options) : base(options) {}

        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EditEntry> EditEntries { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<InstallationSettings> InstallationSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var page = modelBuilder.Entity<Page>();
            page.HasIndex(o => o.Title);
            var edit = modelBuilder.Entity<EditEntry>();
            edit.HasIndex(o => o.Timestamp);
            var comment = modelBuilder.Entity<Comment>();
            comment.HasIndex(o => o.Timestamp);
        }
    }
}
