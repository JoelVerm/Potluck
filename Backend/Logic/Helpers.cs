namespace Logic;

public static class Helpers
{
    public static B? Map<A, B>(this A? value, Func<A, B> mapper)
        where A : class
        where B : class
    {
        return value == null ? null : mapper(value);
    }
}