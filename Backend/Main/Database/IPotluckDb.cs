namespace Backend_Example.Database
{
    public interface IPotluckDb
    {
        public int SaveChanges();

        public User? GetUser(string? name);
    }
}
