
namespace SampleApp
{


    public static class SampleMailClient
    {


        public static void Send(
            string from = null, 
            string to = null, 
            string subject = null,
            string user = null, 
            string password = null,
            MimeKit.MimeEntity body = null,
            int count = 1,
            bool useSsl = false,
            int port = 9025)
        {
            // MimeKit.MimeMessage message = MimeKit.MimeMessage.Load(@"C:\Dev\Cain\Temp\message.eml");
            MimeKit.MimeMessage message = new MimeKit.MimeMessage();

            message.From.Add(MimeKit.MailboxAddress.Parse(from ?? "from@sample.com"));
            message.To.Add(MimeKit.MailboxAddress.Parse(to ?? "to@sample.com"));
            message.Subject = subject ?? "Hello";
            message.Body = body ?? new MimeKit.TextPart("plain")
            {
                Text = "Hello World"
            };

            using MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();

            client.Connect("localhost", port, useSsl);

            if (user != null && password != null)
            {
                client.Authenticate(user, password);
            }

            while (count-- > 0)
            {
                client.Send(message);
            }

            client.Disconnect(true);
        }


    }


}
