namespace Logic
{
    public static class Helpers
    {
        public static int ToCents(this decimal value) => (int)(value * 100);

        public static decimal ToMoney(this int value) => value / 100m;

        public static string[] Split(this string s, Func<char, bool> predicate)
        {
            var result = new List<string>();
            foreach (char c in s)
            {
                if (predicate(c))
                    result.Add(c.ToString());
                else if (result.Count == 0)
                    result.Add(c.ToString());
                else
                    result[^1] += c;
            }
            return result.ToArray();
        }
    }
}
