
namespace SampleApp.Examples
{


    public static class SessionContextExample
    {

        static System.Threading.CancellationTokenSource _cancellationTokenSource;


        public static void Run()
        {
            _cancellationTokenSource = new System.Threading.CancellationTokenSource();

            SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")
                .Endpoint(builder =>
                    builder
                        .AllowUnsecureAuthentication()
                        .AuthenticationRequired()
                        .Port(9025))
                .Build();

            SmtpServer.ComponentModel.ServiceProvider serviceProvider = new SmtpServer.ComponentModel.ServiceProvider();
            serviceProvider.Add(new AuthenticationHandler());

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, serviceProvider);

            server.SessionCreated += OnSessionCreated;
            server.SessionCompleted += OnSessionCompleted;

            System.Threading.Tasks.Task serverTask = server.StartAsync(_cancellationTokenSource.Token);

            SampleMailClient.Send(user: "cain", password: "o'sullivan", count: 5);

            serverTask.WaitWithoutException();
        }


        static void OnSessionCreated(object sender, SmtpServer.SessionEventArgs e)
        {
            // the session context contains a Properties dictionary 
            // which can be used to custom session context

            e.Context.Properties["Start"] = System.DateTimeOffset.Now;
            e.Context.Properties["Commands"] = new System.Collections.Generic.List<SmtpServer.Protocol.SmtpCommand>();

            e.Context.CommandExecuting += OnCommandExecuting;
        }


        static void OnCommandExecuting(object sender, SmtpServer.SmtpCommandEventArgs e)
        {
            ((System.Collections.Generic.List<SmtpServer.Protocol.SmtpCommand>)e.Context.Properties["Commands"]).Add(e.Command);
        }

        static void OnSessionCompleted(object sender, SmtpServer.SessionEventArgs e)
        {
            e.Context.CommandExecuting -= OnCommandExecuting;

            System.Console.WriteLine("The session started at {0}.", e.Context.Properties["Start"]);
            System.Console.WriteLine();

            System.Console.WriteLine("The user that authenticated was {0}", e.Context.Properties["User"]);
            System.Console.WriteLine();

            System.Console.WriteLine("The following commands were executed during the session;");
            System.Console.WriteLine();

            SmtpServer.Tracing.TracingSmtpCommandVisitor writer = new SmtpServer.Tracing.TracingSmtpCommandVisitor(System.Console.Out);

            foreach (SmtpServer.Protocol.SmtpCommand command in (System.Collections.Generic.List<SmtpServer.Protocol.SmtpCommand>)e.Context.Properties["Commands"])
            {
                writer.Visit(command);
            }

            _cancellationTokenSource.Cancel();
        }


        public class AuthenticationHandler 
            : SmtpServer.Authentication.UserAuthenticator
        {
            public override System.Threading.Tasks.Task<bool> AuthenticateAsync(
                SmtpServer.ISessionContext context,
                string user,
                string password,
                System.Threading.CancellationToken cancellationToken)
            {
                context.Properties["User"] = user;

                return System.Threading.Tasks.Task.FromResult(true);
            }
        }


    }


}