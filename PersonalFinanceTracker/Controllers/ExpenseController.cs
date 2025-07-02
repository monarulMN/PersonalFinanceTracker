using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExpenseController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var expenses = await _dbContext.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            return View(expenses);
        }


        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if(ModelState.IsValid)
            {
                LoadCategories();
                return View(expense);
            }

            expense.UserId = _userManager.GetUserId(User);
            _dbContext.Expenses.Add(expense);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _dbContext.Expenses.FindAsync(id);

            if (expense == null || expense.UserId != _userManager.GetUserId(User))
                return NotFound();

            LoadCategories();
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id) return NotFound();

            if (ModelState.IsValid)
            {
                LoadCategories();
                return View(expense);
            }

            var userId = _userManager.GetUserId(User);
            var existing = await _dbContext.Expenses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            
            if(existing == null) return NotFound();

            expense.UserId = userId;
            _dbContext.Update(expense);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            var expense = await _dbContext.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (expense == null) return NotFound();

            return View(expense);
        }

        // POST: /Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var expense = await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (expense == null) return NotFound();

            _dbContext.Expenses.Remove(expense);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private void LoadCategories()
        {
            ViewBag.Categories = new SelectList(_dbContext.Categories.OrderBy(c => c.Name), "Id", "Name");
        }
    }
}
