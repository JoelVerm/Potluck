namespace Logic
{
    public abstract class LogicBase
    {
        protected Models.User _user = null!;
        protected IPotluckDb _db = null!;

        public static T Create<T>(Models.User user, IPotluckDb db)
            where T : LogicBase, new()
        {
            return new T { _user = user, _db = db };
        }
    }
}
