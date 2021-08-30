namespace Muse
{
    public static class Extensions
    {
        public static string TrimAfterLastChar(this string s, char c, bool includeChar = false) =>
            s.Substring(0, s.LastIndexOf(c) + (includeChar ? 0 : 1));

        public static string TrimBeforeLastChar(this string s, char c, bool includeChar = false) =>
            s.Substring(s.LastIndexOf(c) + (includeChar ? 1 : 0));

        public static string Flatten(this string[] array, string separator) =>
            string.Join(separator, array);
    }
}