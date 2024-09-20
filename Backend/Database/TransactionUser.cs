namespace Backend_Example.Database
{
    public class TransactionUser
    {
        public int Id { get; set; }
        public Transaction Transaction { get; set; }
        public User? User { get; set; }
    }
}
