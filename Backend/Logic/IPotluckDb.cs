namespace Logic
{
    public interface IPotluckDb
    {
        public int SaveChanges();

        public Models.User? GetUser(string? name);
    }
}
