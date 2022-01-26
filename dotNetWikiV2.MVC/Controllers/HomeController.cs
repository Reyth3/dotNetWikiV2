using Bogus;
using dotNetWikiV2.MVC.Models;
using dotNetWikiV2.MVC.Models.DB;
using dotNetWikiV2.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace dotNetWikiV2.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<WikiUser> _userManager;
        private readonly SignInManager<WikiUser> _signInManager;
        private readonly MarkdownGeneration _markdownGeneration;

        public HomeController(ILogger<HomeController> logger,
            AppDbContext context,
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<WikiUser> userManager,
            SignInManager<WikiUser> signInManager,
            MarkdownGeneration markdownGeneration)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _markdownGeneration = markdownGeneration;
        }

        public IActionResult Index()
        {
            // Project hasn't been configured yet:
            var installation = _context.InstallationSettings.FirstOrDefault();
            if (installation == null)
                return RedirectToAction(nameof(Install));

            return RedirectToAction("Show", "Pages", new { id = "Homepage" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Install()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InstallConfirm([Bind("SiteName,SiteLogoUrl")] InstallationSettings install, string adminUser, string adminPass, string category, bool demoContent)
        {
            if (_context.InstallationSettings.FirstOrDefault() != null)
                return NotFound();

            Faker f = new Faker();
            var admin = new WikiUser(adminUser) { Email = adminUser };
            var u = await _userManager.CreateAsync(admin, adminPass);
            await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            await _userManager.AddToRoleAsync(admin, "Admin");
            await _signInManager.SignInAsync(admin, true);

            var defaultCategory = new Category(category)
            {
                Description = f.Lorem.Paragraphs(1, 3)
            };

            if (demoContent)
            {
                await GenerateDemoContent(admin);
                defaultCategory.Pages.Add(new Page("Homepage", await _markdownGeneration.GetMarkdownPage(), admin));
            }
            _context.Categories.Add(defaultCategory);

            _context.InstallationSettings.Add(install);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task GenerateDemoContent(WikiUser author)
        {
            var nature = new Category("Nature") { Description = "This category contains information about ***nature only***." };
            
            var plants = new Category("Plants") { Parent = nature };
            var vegetables = new Category("Vegetables") { Parent = plants };
            var fruits = new Category("Fruits") { Parent = plants };

            var animals = new Category("Animals") { Parent = nature };
            var reptiles = new Category("Reptiles") { Parent = animals };
            var birds = new Category("Birds") { Parent = animals };
            var amphibians = new Category("Amphibians") { Parent = animals };
            var fish = new Category("Fish") { Parent = animals };

            var science = new Category("Science");
            var mathematics = new Category("Mathematics") { Parent = science };
            var chemistry = new Category("Chemistry") { Parent = science };
            var physics = new Category("Physics") { Parent = science };

            var categories = new Category[] { nature, plants, vegetables, fruits, animals, amphibians, reptiles, fish, birds, science, chemistry, mathematics, physics };

            Faker f = new Faker();
            foreach (var c in categories)
            {
                c.Description = f.Lorem.Paragraphs(1, 4);
                var num = c.Name.Length * 2;

                for(int i = 0; i < num; i++)
                {
                    var page = new Page($"{f.Lorem.Word()} {Guid.NewGuid().ToString().Substring(0, 3)}", await _markdownGeneration.GetMarkdownPage(), author);
                    c.Pages.Add(page);
                }
            }

            _context.Categories.AddRange(categories);
        }
    }
}