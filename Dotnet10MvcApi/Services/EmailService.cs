using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dotnet10MvcApi.Services
{
    public static class EmailService
    {
        private const string ACCOUNT_EMAIL = "yuberto.gabon@gmail.com";
        private const string ACCOUNT_PASSWORD = "Mypwd123";
        private const string SMTP_HOST = "smtp.gmail.com";
        private const int SMTP_PORT = 587;
        private const bool REQUIRE_SSL = true;    
        private const bool IS_HTML = true;
        private static string SUBJECT_LABEL = "[MyAppName]";
        public const char MULTI_MAILTO_SEPARATOR = ',';

        private const string GMAIL_CLIENT_ID = "YOUR_CLIENT_ID";
        private const string GMAIL_CLIENT_SECRET = "YOUR_CLIENT_SECRET";
        private const string GMAIL_REFRESH_TOKEN = "YOUR_REFRESH_TOKEN";

        private static readonly HttpClient _httpClient = new HttpClient();

        public static bool SendEmail(string mailTo, string subject, string body, EmailAttachment? attachment = null, string attachmentFile = "", string mailCc = "", string mailBc = "", string mailReplyTo = "")
        {
            if (string.IsNullOrEmpty(mailTo)) return false;

            using (var smtp = new SmtpClient()
            {
                Host = SMTP_HOST,
                Port = SMTP_PORT,
                EnableSsl = REQUIRE_SSL,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(ACCOUNT_EMAIL, ACCOUNT_PASSWORD)
            })
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(ACCOUNT_EMAIL);
                
                string[] mailTos = mailTo.Split(MULTI_MAILTO_SEPARATOR).ToArray();
                foreach (var mailto in mailTos)
                {
                    mail.To.Add(mailto);
                }
                
                if (!string.IsNullOrEmpty(mailCc)) mail.CC.Add(mailCc);
                if (!string.IsNullOrEmpty(mailBc)) mail.Bcc.Add(mailBc);
                if (!string.IsNullOrEmpty(mailReplyTo)) mail.ReplyToList.Add(mailReplyTo);
                
                mail.Subject = SUBJECT_LABEL + " " + subject;
                mail.IsBodyHtml = IS_HTML;
                mail.Body = body;

                MemoryStream? attachmentStream = null;
                try
                {
                    if (attachment != null && string.IsNullOrEmpty(attachmentFile))
                    {
                        attachmentStream = new MemoryStream(attachment.Data);
                        mail.Attachments.Add(new Attachment(attachmentStream, attachment.FileName, attachment.ContentType));
                    }
                    else if (!string.IsNullOrEmpty(attachmentFile) && attachment == null)
                    {
                        mail.Attachments.Add(new Attachment(attachmentFile));
                    }

                    smtp.Send(mail);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    attachmentStream?.Dispose();
                }
            }
        }

        public static async Task<bool> SendWithGmailAsync(string mailTo, string subject, string body, EmailAttachment? attachment = null, string attachmentFile = "", string mailCc = "", string mailBc = "", string mailReplyTo = "")
        {
            if (string.IsNullOrEmpty(mailTo)) return false;

            string? accessToken = await GetGmailAccessTokenAsync(GMAIL_CLIENT_ID, GMAIL_CLIENT_SECRET, GMAIL_REFRESH_TOKEN);
            if (string.IsNullOrEmpty(accessToken)) return false;

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(ACCOUNT_EMAIL);
                
                string[] mailTos = mailTo.Split(MULTI_MAILTO_SEPARATOR).ToArray();
                foreach (var mailto in mailTos)
                {
                    mail.To.Add(mailto);
                }
                
                if (!string.IsNullOrEmpty(mailCc)) mail.CC.Add(mailCc);
                if (!string.IsNullOrEmpty(mailBc)) mail.Bcc.Add(mailBc);
                if (!string.IsNullOrEmpty(mailReplyTo)) mail.ReplyToList.Add(mailReplyTo);
                
                mail.Subject = SUBJECT_LABEL + " " + subject;
                mail.IsBodyHtml = IS_HTML;
                mail.Body = body;

                MemoryStream? attachmentStream = null;
                try
                {
                    if (attachment != null && string.IsNullOrEmpty(attachmentFile))
                    {
                        attachmentStream = new MemoryStream(attachment.Data);
                        mail.Attachments.Add(new Attachment(attachmentStream, attachment.FileName, attachment.ContentType));
                    }
                    else if (!string.IsNullOrEmpty(attachmentFile) && attachment == null)
                    {
                        mail.Attachments.Add(new Attachment(attachmentFile));
                    }

                    byte[] mimeBytes = GetMailMessageMimeBytes(mail);
                    string base64UrlMime = Convert.ToBase64String(mimeBytes)
                        .Replace('+', '-')
                        .Replace('/', '_')
                        .TrimEnd('=');

                    return await SendMimeToGmailApiAsync(base64UrlMime, accessToken);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    attachmentStream?.Dispose();
                }
            }
        }

        private static async Task<string?> GetGmailAccessTokenAsync(string clientId, string clientSecret, string refreshToken)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("grant_type", "refresh_token")
                });

                var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    int tokenIndex = jsonResponse.IndexOf("\"access_token\":");
                    if (tokenIndex > -1)
                    {
                        int start = jsonResponse.IndexOf("\"", tokenIndex + 15) + 1;
                        int end = jsonResponse.IndexOf("\"", start);
                        return jsonResponse.Substring(start, end - start);
                    }
                }
            }
            catch
            {
                // Fail silently
            }
            return null;
        }

        private static async Task<bool> SendMimeToGmailApiAsync(string base64UrlMime, string accessToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://gmail.googleapis.com/gmail/v1/users/me/messages/send");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                
                string jsonBody = $"{{\"raw\":\"{base64UrlMime}\"}}";
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] GetMailMessageMimeBytes(MailMessage message)
        {
            var assembly = typeof(SmtpClient).Assembly;
            var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");
            if (mailWriterType == null) throw new InvalidOperationException("MailWriter type not found.");
            
            using (var memoryStream = new MemoryStream())
            {
                var mailWriterConstructor = mailWriterType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(Stream) },
                    null);

                if (mailWriterConstructor == null) throw new InvalidOperationException("MailWriter constructor not found.");

                var mailWriter = mailWriterConstructor.Invoke(new object[] { memoryStream });

                var sendMethod = typeof(MailMessage).GetMethod(
                    "Send",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (sendMethod == null) throw new InvalidOperationException("Send method not found.");

                sendMethod.Invoke(message, new[] { mailWriter, true });
                return memoryStream.ToArray();
            }
        }

        public static EmailAttachment? FileToAttachment(IFormFile? file)
        {
            if (file != null)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    return new EmailAttachment
                    {
                        Data = ms.ToArray(),
                        FileName = file.FileName,
                        ContentType = file.ContentType
                    };
                }
            }
            return null;
        }

        public class EmailAttachment
        {
            public byte[] Data { get; set; } = Array.Empty<byte>();
            public string FileName { get; set; } = string.Empty;
            public string ContentType { get; set; } = string.Empty;
        }
    }
}
