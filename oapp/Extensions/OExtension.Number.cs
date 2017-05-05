namespace oapp.Extensions
{
    public static partial class OExtension
    {
        /// <summary>
        /// try parse integer number by given string
        /// </summary>
        /// <param name="value">string number to parse</param>
        /// <param name="defaultValue">default value if parse fail</param>
        /// <returns>parsed value</returns>
        public static int TryParseInt(this string value, int defaultValue = 0)
        {
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }

        /// <summary>
        /// try parse double number by given string
        /// </summary>
        /// <param name="value">string number to parse</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double TryParseDouble(this string value, double defaultValue = 0)
        {
            double result;
            return double.TryParse(value, out result) ? result : defaultValue;
        }
    }
}