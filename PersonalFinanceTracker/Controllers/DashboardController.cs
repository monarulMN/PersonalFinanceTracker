using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var totalIncome = await _dbContext.Incomes
                .Where(i => i.UserId == userId)
                .SumAsync(i => (decimal?)i.Amount) ?? 0;


            var totalExpense = await _dbContext.Expenses
                .Where(i => i.UserId == userId)
                .SumAsync(i => (decimal?)i.Amount) ?? 0;

            var balance = totalIncome - totalExpense;

            //Income by Category
            var incomeByCategory = await _dbContext.Incomes
                .Where(i => i.UserId == userId)
                .GroupBy(i => i.Category.Name)
                .Select(g => new { Category = g.Key, Total = g.Sum(i => i.Amount) })
                .ToListAsync();

            //Expense by Category
            var expenseByCategory = await _dbContext.Expenses
                .Where(i => i.UserId == userId)
                .GroupBy(i => i.Category.Name)
                .Select(g => new {Category = g.Key, Total = g.Sum(e => e.Amount) })
                .ToListAsync();

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.Balance = balance;
            ViewBag.IncomeByCategory = incomeByCategory;
            ViewBag.ExpenseByCategory = expenseByCategory;

            return View();
        }
    }
}
