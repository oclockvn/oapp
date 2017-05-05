using oapp.Extensions;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace oapp.Helpers
{
    public class MailHelper
    {
        private static readonly NLog.ILogger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// send email using gmail. Make sure that you add config keys defined in this method
        /// </summary>
        /// <param name="from">add key in config: <add name="mail.from" value="" /></param>
        /// <param name="to">add key in config: <add name="mail.to" value="" /></param>
        /// <param name="subject">email subject</param>
        /// <param name="body">html body of email</param>
        /// <returns>true if send success, otherwise return false</returns>
        public static bool SendMail(string from, string to, string subject, string body)
        {
            Func<string, string> GetConfig = key => ConfigHelper.GetAppSetting(key);

            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(!string.IsNullOrEmpty(from) ? from : GetConfig(AppKey.MailFromKey));
                    if (!string.IsNullOrEmpty(to))
                    {
                        mail.To.Add(to);
                    }
                    mail.To.Add(GetConfig(AppKey.MailToKey));
                    mail.Bcc.Add(GetConfig(AppKey.MailBccKey));
                    mail.Subject = $"{AppKey.AppFlag} {subject}";
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    mail.BodyEncoding = Encoding.UTF8;

                    using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(GetConfig(AppKey.MailUserKey), GetConfig(AppKey.MailPasswordKey));
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }

                    return true;
                }
            }
            catch (Exception ex) // todo: log error
            {
                Debug.WriteLine(ex.ToErrorMessage());
                log.Error($"Send email from: {from} -> to: {to} error: " + ex.ToErrorMessage());
                return false;
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}