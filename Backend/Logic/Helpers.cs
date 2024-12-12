namespace Logic;

public static class Helpers
{
    public static int ToCents(this decimal value)
    {
        return (int)(value * 100);
    }

    public static decimal ToMoney(this int value)
    {
        return value / 100m;
    }
}