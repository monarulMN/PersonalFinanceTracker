using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;


namespace PersonalFinanceTracker.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var categories = await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
            return View(categories);
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            if (!ModelState.IsValid) return View(category);

            _dbContext.Update(category);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
