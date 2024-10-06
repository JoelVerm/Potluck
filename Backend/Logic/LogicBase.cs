using Backend_Example.Database;

namespace Backend_Example.Logic
{
    public abstract class LogicBase
    {
        protected Database.User _user = null!;
        protected PotluckDb _db = null!;

        public static T Create<T>(Database.User user, PotluckDb db)
            where T : LogicBase, new()
        {
            return new T { _user = user, _db = db };
        }
    }
}
