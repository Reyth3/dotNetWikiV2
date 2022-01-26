using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetWikiV2.MVC.Models.DB
{
    public class InstallationSettings
    {
        [Key]
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteLogoUrl { get; set; }

    }
}
