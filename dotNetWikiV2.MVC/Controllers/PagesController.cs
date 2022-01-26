using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dotNetWikiV2.MVC.Models.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace dotNetWikiV2.MVC.Controllers
{
    public class PagesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<WikiUser> _userManager;
        private readonly Random _random;

        public PagesController(AppDbContext context, UserManager<WikiUser> userManager, Random random)
        {
            _context = context;
            _userManager = userManager;
            _random = random;
        }

        // GET: Pages
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Pages.Include(p => p.Category);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Pages/Show/5
        public async Task<IActionResult> Show(string id)
        {
            if (id == null)
            {
                return View("MissingPage");
            }

            var page = await _context.Pages
                .Include(p => p.Category)
                .Include(p => p.Changes)
                .ThenInclude(p => (p as EditEntry).Author)
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                return View("MissingPage");
            }

            return View(page);
        }

        // GET: Pages/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            ViewData["Names"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Pages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PageId,Title,Content,CategoryId")] Page page)
        {
            if (ModelState.IsValid)
            {
                var usr = await _userManager.GetUserAsync(User);
                page.PageId = Page.SlugUtil.Slugify(page.Title);
                page.Changes.Add(new EditEntry()
                {
                    Author = usr,
                    Description = "Created the page.",
                    Timestamp = DateTimeOffset.UtcNow
                });

                _context.Add(page);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Show), "Pages", new { id = page.PageId });
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", page.CategoryId);
            return View(page);
        }

        // GET: Pages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", page.CategoryId);
            return View(page);
        }

        // POST: Pages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PageId,Title,Content,CategoryId")] Page page, string editReason)
        {
            if (id != page.PageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    page.Changes.Add(new EditEntry()
                    {
                        Description = editReason,
                        Timestamp = DateTimeOffset.UtcNow,
                        WikiUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    });
                    _context.Update(page);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(page.PageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", page.CategoryId);
            return View(page);
        }

        // GET: Pages/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Pages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var page = await _context.Pages
                .Include(o => o.Comments)
                .Include(o => o.Changes)
                .FirstOrDefaultAsync(o => o.PageId == id);
            foreach (var change in page.Changes)
                _context.EditEntries.Remove(change);
            foreach (var comment in page.Comments)
                _context.Comments.Remove(comment);
            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Pages/Random
        public async Task<IActionResult> Random()
        {
            var skip = (int)(_random.NextDouble() * _context.Pages.Count());
            var page = _context.Pages.OrderBy(o => o.PageId).Skip(skip).Take(1).FirstOrDefault();
            return RedirectToAction(nameof(Show), new { id = page.PageId ?? "homepage" });
        }

        private bool PageExists(string id)
        {
            return _context.Pages.Any(e => e.PageId == id);
        }
    }
}
