
namespace SampleApp.Examples
{


    public static class ServerCancellingExample
    {


        public static void Run()
        {
            System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

            SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")
                .Port(9025)
                .Build();

            SmtpServer.ComponentModel.ServiceProvider serviceProvider = new SmtpServer.ComponentModel.ServiceProvider();
            serviceProvider.Add(new SampleMailboxFilter(System.TimeSpan.FromSeconds(5)));

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, serviceProvider);
            server.SessionCreated += OnSessionCreated;
            server.SessionCompleted += OnSessionCompleted;
            server.SessionFaulted += OnSessionFaulted;
            server.SessionCancelled += OnSessionCancelled;

            System.Threading.Tasks.Task serverTask = server.StartAsync(cancellationTokenSource.Token);

            // ReSharper disable once MethodSupportsCancellation
            System.Threading.Tasks.Task.Run(() => SampleMailClient.Send());

            System.Console.WriteLine("Press any key to cancel the server.");
            System.Console.ReadKey();

            System.Console.WriteLine("Forcibily cancelling the server and any active sessions");

            cancellationTokenSource.Cancel();
            serverTask.WaitWithoutException();

            System.Console.WriteLine("The server has been cancelled.");
        }

        static void OnSessionCreated(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("Session Created.");
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