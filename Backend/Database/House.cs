using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Backend_Example.Database
{
    [Index(nameof(Name), IsUnique = true)]
    public class House
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; } = [];
        public List<Transaction> Transactions { get; set; } = [];
    }
}
