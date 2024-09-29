namespace Backend_Example.Database
{
    public class TransactionUser
    {
        public int Id { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual User? User { get; set; }
        public int Count { get; set; }
    }
}
