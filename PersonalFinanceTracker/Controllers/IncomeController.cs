using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Controllers
{
    //[Authorize]
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public IncomeController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var incomes = await _dbContext.Incomes
                .Include(i => i.Category)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
            return View(incomes);
        }


        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Income income)
        {
            if(ModelState.IsValid)
            {
                LoadCategories();
                return View(income);
            }

            
            income.UserId = _userManager.GetUserId(User);
            _dbContext.Incomes.Add(income);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var income = await _dbContext.Incomes.FindAsync(id);
            if (income == null || income.UserId != _userManager.GetUserId(User))
                return NotFound();

            LoadCategories(); 
            return View(income);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Income income)
        {
            if (id != income.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return View(income);
            }

            var userId = _userManager.GetUserId(User);
            var existing = await _dbContext.Incomes.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (existing == null) return NotFound();

            income.UserId = userId;
            _dbContext.Update(income);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var income = await _dbContext.Incomes
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (income == null) return NotFound();

            return View(income);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var income = await _dbContext.Incomes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (income == null) return NotFound();

            _dbContext.Incomes.Remove(income);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Helper to load categories for dropdown
        private void LoadCategories()
        {
            ViewBag.Categories = new SelectList(_dbContext.Categories.OrderBy(c => c.Name), "Id", "Name");
        }
    }
}
