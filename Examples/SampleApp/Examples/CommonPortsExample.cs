using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using SmtpServer;
using SmtpServer.ComponentModel;

namespace SampleApp.Examples
{
    public static class SimpleExample
    {
        public static void Run()
        {
            ISmtpServerOptions options = new SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")

                // Port 25 is primarily used for SMTP relaying where emails are 
                // sent from one mail server to another. Mail clients generally wont
                // use this port and most ISP will likely block it anyway.
                .Endpoint(builder => builder.Port(25).IsSecure(false))
                /*
                // For a brief period in time this was a recognized port whereby
                // TLS was enabled by default on the connection. When connecting to
                // port 465 the client will upgrade its connection to SSL before
                // doing anything else. Port 465 is obsolete in favor of using
                // port 587 but it is still available by some mail servers.
                .Endpoint(builder => 
                    builder
                        .Port(465)
                        .IsSecure(true) // indicates that the client will need to upgrade to SSL upon connection
                        .Certificate(new X509Certificate2())) // requires a valid certificate to be configured

                // Port 587 is the default port that should be used by modern mail
                // clients. When a certificate is provided, the server will advertise
                // that is supports the STARTTLS command which allows the client
                // to determine when they want to upgrade the connection to SSL. 
                .Endpoint(builder => 
                    builder
                        .Port(587)
                        .AllowUnsecureAuthentication(false) // using 'false' here means that the user cant authenticate unless the connection is secure
                        .Certificate(new X509Certificate2())) // requires a valid certificate to be configured
                */
                .Build();


            ServiceProvider serviceProvider = new ServiceProvider();
            serviceProvider.Add(new SampleUserAuthenticator());
            serviceProvider.Add(new SampleMailboxFilter(TimeSpan.FromSeconds(5)));
            serviceProvider.Add(new SampleMessageStore(Console.Out));

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, serviceProvider);
            server.SessionCreated += OnSessionCreated;

            //server.SessionCreated += OnSessionCreated;
            //server.SessionCompleted += OnSessionCompleted;
            //server.SessionFaulted += OnSessionFaulted;
            //server.SessionCancelled += OnSessionCancelled;


            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            System.Threading.Tasks.Task serverTask = server.StartAsync(cancellationTokenSource.Token);

            // SampleMailClient.Send(user: "user", password: "password", useSsl: true);

            // cancellationTokenSource.Cancel();
            // serverTask.WaitWithoutException();


            Console.WriteLine("Press any key to shudown the server.");
            Console.ReadKey();

            Console.WriteLine("Gracefully shutting down the server.");
            server.Shutdown();

            server.ShutdownTask.WaitWithoutException();
            Console.WriteLine("The server is no longer accepting new connections.");

            Console.WriteLine("Waiting for active sessions to complete.");
            serverTask.WaitWithoutException();
        }


        static void OnSessionCreated(object sender, SessionEventArgs e)
        {
            Console.WriteLine("Session Created.");

            e.Context.CommandExecuting += OnCommandExecuting;
        }

        static void OnCommandExecuting(object sender, SmtpCommandEventArgs e)
        {
            Console.WriteLine("Command Executing.");

            new SmtpServer.Tracing.TracingSmtpCommandVisitor(Console.Out).Visit(e.Command);
        }



    }
}