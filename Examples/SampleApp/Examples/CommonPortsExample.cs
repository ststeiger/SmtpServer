
namespace SampleApp.Examples
{


    public static class CommonPortsExample 
    {


        public static void Run()
        {
            SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")

                // Port 25 is primarily used for SMTP relaying where emails are 
                // sent from one mail server to another. Mail clients generally wont
                // use this port and most ISP will likely block it anyway.
                .Endpoint(builder => builder.Port(25).IsSecure(false))
                
                // For a brief period in time this was a recognized port whereby
                // TLS was enabled by default on the connection. When connecting to
                // port 465 the client will upgrade its connection to SSL before
                // doing anything else. Port 465 is obsolete in favor of using
                // port 587 but it is still available by some mail servers.
                .Endpoint(builder => 
                    builder
                        .Port(465)
                        .IsSecure(true) // indicates that the client will need to upgrade to SSL upon connection
                        .Certificate(new System.Security.Cryptography.X509Certificates.X509Certificate2())) // requires a valid certificate to be configured

                // Port 587 is the default port that should be used by modern mail
                // clients. When a certificate is provided, the server will advertise
                // that is supports the STARTTLS command which allows the client
                // to determine when they want to upgrade the connection to SSL. 
                .Endpoint(builder => 
                    builder
                        .Port(587)
                        .AllowUnsecureAuthentication(false) // using 'false' here means that the user cant authenticate unless the connection is secure
                        .Certificate(new System.Security.Cryptography.X509Certificates.X509Certificate2())) // requires a valid certificate to be configured
                .Build();


            SmtpServer.ComponentModel.ServiceProvider serviceProvider = new SmtpServer.ComponentModel.ServiceProvider();
            serviceProvider.Add(new SampleUserAuthenticator());
            serviceProvider.Add(new SampleMailboxFilter(System.TimeSpan.FromSeconds(5)));
            serviceProvider.Add(new SampleMessageStore(System.Console.Out));

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, serviceProvider);
            
            server.SessionCreated += OnSessionCreated;
            server.SessionCompleted += OnSessionCompleted;
            server.SessionFaulted += OnSessionFaulted;
            server.SessionCancelled += OnSessionCancelled;


            System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();
            System.Threading.Tasks.Task serverTask = server.StartAsync(cancellationTokenSource.Token);

            // SampleMailClient.Send(user: "user", password: "password", useSsl: true);

            // cancellationTokenSource.Cancel();
            // serverTask.WaitWithoutException();


            System.Console.WriteLine("Press any key to shudown the server.");
            System.Console.ReadKey();

            System.Console.WriteLine("Gracefully shutting down the server.");
            server.Shutdown();

            server.ShutdownTask.WaitWithoutException();
            System.Console.WriteLine("The server is no longer accepting new connections.");

            System.Console.WriteLine("Waiting for active sessions to complete.");
            serverTask.WaitWithoutException();
        }


        static void OnCommandExecuting(object sender, SmtpServer.SmtpCommandEventArgs e)
        {
            System.Console.WriteLine("Command Executing.");

            new SmtpServer.Tracing.TracingSmtpCommandVisitor(System.Console.Out).Visit(e.Command);
        }


        static void OnSessionCreated(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("Session Created.");

            e.Context.CommandExecuting += OnCommandExecuting;
        }


        static void OnSessionCompleted(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("Session Completed");
        }


        static void OnSessionFaulted(object sender, SmtpServer.SessionFaultedEventArgs e)
        {
            System.Console.WriteLine("Session Faulted: {0}", e.Exception);
        }


        static void OnSessionCancelled(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("Session Cancelled");
        }


    }


}