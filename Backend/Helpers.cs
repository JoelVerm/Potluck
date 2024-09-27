namespace Backend_Example
{
    public static class Helpers
    {
        public static int ToCents(this decimal value) => (int)(value * 100);
        public static decimal ToMoney(this int value) => value / 100m;
    }
}
