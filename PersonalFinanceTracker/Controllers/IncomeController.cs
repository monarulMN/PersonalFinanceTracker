using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Controllers
{
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
                .ToListAsync();
            return View(incomes);
        }


        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_dbContext.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Income income)
        {
            if(ModelState.IsValid)
            {
                income.UserId = _userManager.GetUserId(User);
                _dbContext.Add(income);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewData["CategoryId"] = new SelectList(_dbContext.Categories, "Id", "Name", income.CategoryId);
            return View(income);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var income = await _dbContext.Incomes.FindAsync(id);
            if (income == null || income.UserId != _userManager.GetUserId(User))
                return NotFound();

            ViewData["CategoryId"] = new SelectList(_dbContext.Categories, "Id", "Name", income.CategoryId);
            return View(income);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Income income)
        {
            if (id != income.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    income.UserId = _userManager.GetUserId(User);
                    _dbContext.Update(income);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncomeExists(income.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_dbContext.Categories, "Id", "Name", income.CategoryId);
            return View(income);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var income = await _dbContext.Incomes
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == _userManager.GetUserId(User));

            if (income == null) return NotFound();

            return View(income);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var income = await _dbContext.Incomes.FindAsync(id);
            if (income != null && income.UserId == _userManager.GetUserId(User))
            {
                _dbContext.Incomes.Remove(income);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool IncomeExists(int id) =>
            _dbContext.Incomes.Any(e => e.Id == id);

    }
}
