// C# SMTP Configuration for Outlook.Com SMTP Host
// https://www.codeproject.com/Articles/700211/Csharp-SMTP-Configuration-for-Outlook-Com-SMTP-Hos
// Allowing less secure apps to access your account in Gmail
// https://support.google.com/accounts/answer/6010255

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;

public static class EmailService
{
    private const string ACCOUNT_EMAIL = "yuberto.gabon@gmail.com";
    private const string ACCOUNT_PASSWORD = "Mypwd123";
    private const string SMTP_HOST = "smtp.gmail.com"; // smtp-mail.outlook.com, smtp.mail.yahoo.com
    private const int SMTP_PORT = 587;
    private const bool REQUIRE_SSL = true;    
    private const bool IS_HTML = true;
    private static string SUBJECT_LABEL = "[MyAppName]";
    public const char MULTI_MAILTO_SEPARATOR = ','; // semi-colon or comma

    // Google OAuth2 Gmail API credentials
    private const string GMAIL_CLIENT_ID = "YOUR_CLIENT_ID";
    private const string GMAIL_CLIENT_SECRET = "YOUR_CLIENT_SECRET";
    private const string GMAIL_REFRESH_TOKEN = "YOUR_REFRESH_TOKEN";

    #region EmailServiceMethods
    public static bool SendEmail(string mailTo, string subject, string body, EmailAttachment attachment = null, string attachmentFile = "", string mailCc = "", string mailBc = "", string mailReplyTo = "")
    {
        if (string.IsNullOrEmpty(mailTo)) return false;

        using (var smtp = new SmtpClient()
        {
            Host = SMTP_HOST,
            Port = SMTP_PORT,
            EnableSsl = REQUIRE_SSL,
            UseDefaultCredentials = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new System.Net.NetworkCredential(ACCOUNT_EMAIL, ACCOUNT_PASSWORD)
        })
        using (var mail = new MailMessage())
        {
            mail.From = new MailAddress(ACCOUNT_EMAIL);
            
            string[] mailTos = mailTo.Split(MULTI_MAILTO_SEPARATOR).ToArray();
            foreach (var mailto in mailTos)
            {
                mail.To.Add(mailto);
            }
            
            if (!String.IsNullOrEmpty(mailCc)) mail.CC.Add(mailCc);
            if (!String.IsNullOrEmpty(mailBc)) mail.Bcc.Add(mailBc);
            if (!String.IsNullOrEmpty(mailReplyTo)) mail.ReplyToList.Add(mailReplyTo);
            
            mail.Subject = SUBJECT_LABEL + " " + subject;
            mail.IsBodyHtml = IS_HTML;
            mail.Body = body;

            MemoryStream attachmentStream = null;
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
                if (attachmentStream != null)
                {
                    attachmentStream.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// Sends an email using the Google Gmail REST API with OAuth2 credentials. [BernardGabon.com]
    /// </summary>
    public static bool SendWithGmail(string mailTo, string subject, string body, EmailAttachment attachment = null, string attachmentFile = "", string mailCc = "", string mailBc = "", string mailReplyTo = "")
    {
        if (string.IsNullOrEmpty(mailTo)) return false;

        string accessToken = GetGmailAccessToken(GMAIL_CLIENT_ID, GMAIL_CLIENT_SECRET, GMAIL_REFRESH_TOKEN);
        if (string.IsNullOrEmpty(accessToken)) return false;

        using (var mail = new MailMessage())
        {
            mail.From = new MailAddress(ACCOUNT_EMAIL);
            
            string[] mailTos = mailTo.Split(MULTI_MAILTO_SEPARATOR).ToArray();
            foreach (var mailto in mailTos)
            {
                mail.To.Add(mailto);
            }
            
            if (!String.IsNullOrEmpty(mailCc)) mail.CC.Add(mailCc);
            if (!String.IsNullOrEmpty(mailBc)) mail.Bcc.Add(mailBc);
            if (!String.IsNullOrEmpty(mailReplyTo)) mail.ReplyToList.Add(mailReplyTo);
            
            mail.Subject = SUBJECT_LABEL + " " + subject;
            mail.IsBodyHtml = IS_HTML;
            mail.Body = body;

            MemoryStream attachmentStream = null;
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

                return SendMimeToGmailApi(base64UrlMime, accessToken);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (attachmentStream != null)
                {
                    attachmentStream.Dispose();
                }
            }
        }
    }

    private static string GetGmailAccessToken(string clientId, string clientSecret, string refreshToken)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://oauth2.googleapis.com/token");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            string postData = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token",
                Uri.EscapeDataString(clientId),
                Uri.EscapeDataString(clientSecret),
                Uri.EscapeDataString(refreshToken));

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                string jsonResponse = streamReader.ReadToEnd();
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

    private static bool SendMimeToGmailApi(string base64UrlMime, string accessToken)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("https://gmail.googleapis.com/gmail/v1/users/me/messages/send");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            string jsonBody = string.Format("{{\"raw\":\"{0}\"}}", base64UrlMime);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
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
        
        using (var memoryStream = new MemoryStream())
        {
            var mailWriterConstructor = mailWriterType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(Stream) },
                null);

            var mailWriter = mailWriterConstructor.Invoke(new object[] { memoryStream });

            var sendMethod = typeof(MailMessage).GetMethod(
                "Send",
                BindingFlags.Instance | BindingFlags.NonPublic);

            sendMethod.Invoke(message, new[] { mailWriter, true });
            return memoryStream.ToArray();
        }
    }

    public static EmailAttachment FileToAttachment(HttpPostedFileBase File)
    {
        if (File != null)
        {
            var attachment = new EmailAttachment
            {
                Data = new byte[File.ContentLength],
                FileName = File.FileName,
                ContentType = File.ContentType
            };
            File.InputStream.Read(attachment.Data, 0, File.ContentLength);

            return attachment;
        }

        return null;
    }

    public static EmailAttachment FileToAttachment(HttpPostedFile file)
    {
        return file != null ? FileToAttachment(new HttpPostedFileWrapper(file)) : null;
    }

    public class EmailAttachment
    {
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }

    #endregion
}
