using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;

namespace Backend_Example.Database
{
    [Index(nameof(Name), IsUnique = true)]
    public class House
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<User> Users { get; set; } = [];
        public virtual List<Transaction> Transactions { get; set; } = [];
        public string? CookingUserId { get; set; }
        public virtual User? CookingUser { get; set; }
        public int CookingPrice { get; set; } = 0;
        public string CookingDescription { get; set; } = "";
        public string ShoppingList { get; set; } = "";
    }
}
