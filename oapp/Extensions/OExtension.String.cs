namespace oapp.Extensions
{
    public static partial class OExtension
    {
        /// <summary>
        /// check input is null, empty or whitespace
        /// </summary>
        /// <param name="s">a string to check</param>
        /// <returns>true if string is one of null, empty or whitespace. Otherwise return false</returns>
        public static bool IsNullOrEmtyOrWhitespace(this string s)
        {
            return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// trim string by specify length
        /// </summary>
        /// <param name="s">a string to cut</param>
        /// <param name="length">number of return string length</param>
        /// <returns>truncated string</returns>
        public static string Truncate(this string s, int length = 50)
        {
            if (s.IsNullOrEmtyOrWhitespace())
                return string.Empty;

            if (s.Length <= length)
                return s;

            return s.Substring(0, length);
        }
    }
}