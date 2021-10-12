
namespace SampleApp.Examples
{


    public static class SessionTracingExample
    {
        static System.Threading.CancellationTokenSource _cancellationTokenSource;

        public static void Run()
        {
            _cancellationTokenSource = new System.Threading.CancellationTokenSource();

            SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")
                .Port(9025)
                .Build();

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, SmtpServer.ComponentModel.ServiceProvider.Default);
            
            server.SessionCreated += OnSessionCreated;
            server.SessionCompleted += OnSessionCompleted;
            server.SessionFaulted += OnSessionFaulted;
            server.SessionCancelled += OnSessionCancelled;

            System.Threading.Tasks.Task serverTask = server.StartAsync(_cancellationTokenSource.Token);

            SampleMailClient.Send();

            serverTask.WaitWithoutException();
        }

        static void OnSessionFaulted(object sender, SmtpServer.SessionFaultedEventArgs e)
        {
            System.Console.WriteLine("SessionFaulted: {0}", e.Exception);
        }

        static void OnSessionCancelled(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("SessionCancelled");
        }

        static void OnSessionCreated(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("SessionCreated: {0}", e.Context.Properties[SmtpServer.Net.EndpointListener.RemoteEndPointKey]);

            e.Context.CommandExecuting += OnCommandExecuting;
            e.Context.CommandExecuted += OnCommandExecuted;
        }

        static void OnCommandExecuting(object sender, SmtpServer.SmtpCommandEventArgs e)
        {
            System.Console.WriteLine("Command Executing");
            new SmtpServer.Tracing.TracingSmtpCommandVisitor(System.Console.Out).Visit(e.Command);
        }

        static void OnCommandExecuted(object sender, SmtpServer.SmtpCommandEventArgs e)
        {
            System.Console.WriteLine("Command Executed");
            new SmtpServer.Tracing.TracingSmtpCommandVisitor(System.Console.Out).Visit(e.Command);
        }

        static void OnSessionCompleted(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("SessionCompleted: {0}", e.Context.Properties[SmtpServer.Net.EndpointListener.RemoteEndPointKey]);

            e.Context.CommandExecuting -= OnCommandExecuting;
            e.Context.CommandExecuted -= OnCommandExecuted;

            _cancellationTokenSource.Cancel();
        }


    }


}