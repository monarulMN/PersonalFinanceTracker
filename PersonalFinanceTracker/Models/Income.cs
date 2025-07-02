using System.ComponentModel;

namespace PersonalFinanceTracker.Models
{
    public class Income
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }   
    }
}
