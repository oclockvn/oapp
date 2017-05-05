using System.Configuration;

namespace oapp.Helpers
{
    public class ConfigHelper
    {
        public static string GetAppSetting(string key) => ConfigurationManager.AppSettings[key];
    }
}