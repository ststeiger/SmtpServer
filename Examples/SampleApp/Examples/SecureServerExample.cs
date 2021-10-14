
namespace SampleApp.Examples
{


    public static class SecureServerExample
    {


        public static void Run()
        {
            // this is important when dealing with a certificate that isnt valid
            System.Net.ServicePointManager.ServerCertificateValidationCallback = IgnoreCertificateValidationFailureForTestingOnly;

            System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

            SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")
                .Endpoint(builder =>
                    builder
                        .Port(9025, true)
                        .AllowUnsecureAuthentication(false)
                        .Certificate(
                            delegate (object sender, string hostname)
                            {
                                return CreateCertificate();
                            }
                        ))
                .Build();

            SmtpServer.ComponentModel.ServiceProvider serviceProvider = new SmtpServer.ComponentModel.ServiceProvider();
            serviceProvider.Add(new SampleUserAuthenticator());

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, serviceProvider);
            server.SessionCreated += OnSessionCreated;

            System.Threading.Tasks.Task serverTask = server.StartAsync(cancellationTokenSource.Token);

            SampleMailClient.Send(user: "user", password: "password", useSsl: true);

            cancellationTokenSource.Cancel();
            serverTask.WaitWithoutException();
        }


        static void OnSessionCreated(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("Session Created.");

            e.Context.CommandExecuting += OnCommandExecuting;
        }


        static void OnCommandExecuting(object sender, SmtpServer.SmtpCommandEventArgs e)
        {
            System.Console.WriteLine("Command Executing.");

            new SmtpServer.Tracing.TracingSmtpCommandVisitor(System.Console.Out).Visit(e.Command);
        }


        static bool IgnoreCertificateValidationFailureForTestingOnly(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain, 
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


        static System.Security.Cryptography.X509Certificates.X509Certificate2 CreateCertificate()
        {
            // to create an X509Certificate for testing you need to run MAKECERT.EXE and then PVK2PFX.EXE
            // http://www.digitallycreated.net/Blog/38/using-makecert-to-create-certificates-for-development

            byte[] certificate = System.IO.File.ReadAllBytes(@"C:\Users\cain\Dropbox\Documents\Cain\Programming\SmtpServer\SmtpServer.pfx");
            string password = System.IO.File.ReadAllText(@"C:\Users\cain\Dropbox\Documents\Cain\Programming\SmtpServer\SmtpServerPassword.txt");

            return new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate, password);
        }


    }


}