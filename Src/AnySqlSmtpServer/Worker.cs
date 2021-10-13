
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace AnySqlSmtpServer
{


    public class Worker 
        : BackgroundService
    {

        protected readonly ILogger<Worker> m_logger;
        protected SmtpServer.SmtpServer m_smtpServer;
        protected System.Threading.Tasks.Task m_serverTask;


        public Worker(ILogger<Worker> logger, SmtpServer.SmtpServer smtpServer)
        {
            this.m_logger = logger;
            this.m_smtpServer = smtpServer;

            this.m_smtpServer.SessionCreated += OnSessionCreated;
            this.m_smtpServer.SessionCompleted += OnSessionCompleted;
            this.m_smtpServer.SessionFaulted += OnSessionFaulted;
            this.m_smtpServer.SessionCancelled += OnSessionCancelled;
        } // End Constructor 


        protected override async System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        { 
            //    //while (!stoppingToken.IsCancellationRequested)
            //    //{
            //    //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    //    await Task.Delay(1000, stoppingToken);
            //    //}
            //    await _smtpServer.StartAsync(stoppingToken);
            //    System.Console.WriteLine("foo");

            this.m_logger.LogInformation("Worker ExecuteAsync starting at: {time}", System.DateTimeOffset.Now);
            await System.Threading.Tasks.Task.CompletedTask;
            this.m_logger.LogInformation("Worker ExecuteAsync ending at: {time}", System.DateTimeOffset.Now);
        } // End Task ExecuteAsync 


        public override async System.Threading.Tasks.Task StartAsync(System.Threading.CancellationToken cancellationToken)
        {
            this.m_logger.LogInformation("{time}: Starting server.", System.DateTimeOffset.Now);
            this.m_serverTask = this.m_smtpServer.StartAsync(cancellationToken);
            await System.Threading.Tasks.Task.CompletedTask;

            this.m_logger.LogInformation("{time}: Server started.", System.DateTimeOffset.Now);
            this.m_logger.LogInformation("Application started. Press Ctrl+C to shut down.");
        } // End Task StartAsync 


        public override async System.Threading.Tasks.Task StopAsync(System.Threading.CancellationToken cancellationToken)
        {
            this.m_logger.LogInformation("{time}: Gracefully shutting down the server.", System.DateTimeOffset.Now);
            this.m_smtpServer.Shutdown();

            this.m_smtpServer.ShutdownTask.WaitWithoutException();
            this.m_logger.LogInformation("{time}: The server is no longer accepting new connections.", System.DateTimeOffset.Now);


            this.m_logger.LogInformation("{time}: Waiting for active sessions to complete.", System.DateTimeOffset.Now);
            this.m_serverTask.WaitWithoutException();
            await System.Threading.Tasks.Task.CompletedTask;
            this.m_logger.LogInformation("{time}: Server stopped.", System.DateTimeOffset.Now);
        } // End Task StopAsync 


        void OnCommandExecuting(object sender, SmtpServer.SmtpCommandEventArgs e)
        {
            System.Console.WriteLine("Command Executing.");

            new SmtpServer.Tracing.TracingSmtpCommandVisitor(System.Console.Out).Visit(e.Command);
        } // End Sub OnCommandExecuting 


        void OnCommandExecuted(object sender, SmtpServer.SmtpCommandEventArgs e)
        {
            System.Console.WriteLine("Command Executed");
            new SmtpServer.Tracing.TracingSmtpCommandVisitor(System.Console.Out).Visit(e.Command);
        } // End Sub OnCommandExecuted 


        void OnSessionCreated(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("Session Created.");

            e.Context.CommandExecuting += OnCommandExecuting;
            e.Context.CommandExecuted += OnCommandExecuted;
        } // End Sub OnSessionCreated 


        void OnSessionCompleted(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("Session Completed");

            e.Context.CommandExecuting -= OnCommandExecuting;
            e.Context.CommandExecuted -= OnCommandExecuted;
        } // End Sub OnSessionCompleted 


        void OnSessionFaulted(object sender, SmtpServer.SessionFaultedEventArgs e)
        {
            System.Console.WriteLine("Session Faulted: {0}", e.Exception);
        } // End Sub OnSessionFaulted 


        void OnSessionCancelled(object sender, SmtpServer.SessionEventArgs e)
        {
            System.Console.WriteLine("Session Cancelled");
        } // End Sub OnSessionCancelled 


    } // End Class Worker 


} // End Namespace AnySqlSmtpServer 
