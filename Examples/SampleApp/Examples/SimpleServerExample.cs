
namespace SampleApp.Examples
{


    public static class SimpleServerExample
    {


        public static void Run()
        {
            System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

            SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")
                .Port(9025)
                .CommandWaitTimeout(System.TimeSpan.FromSeconds(100))
                .Build();

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, SmtpServer.ComponentModel.ServiceProvider.Default);
            server.SessionCreated += OnSessionCreated;

            System.Threading.Tasks.Task serverTask = server.StartAsync(cancellationTokenSource.Token);

            System.Console.WriteLine("Press any key to shutdown the server.");
            System.Console.ReadKey();

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


    }


}
