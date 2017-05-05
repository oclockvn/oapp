using System;

namespace oapp.Extensions
{
    public static partial class OExtension
    {
        public static string ToErrorMessage(this Exception ex)
        {
            if (ex == null)
                return string.Empty;

            return $"Error => {ex.Message} | Inner exception => {ex.InnerException?.Message} | Deep exception => {ex.InnerException?.InnerException?.Message}";
        }
    }
}