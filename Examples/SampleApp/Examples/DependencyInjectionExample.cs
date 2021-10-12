
using Microsoft.Extensions.DependencyInjection;


namespace SampleApp.Examples
{


    public static class DependencyInjectionExample
    {


        public static void Run()
        {
            System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

            SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")
                .Port(9025)
                .Build();

            ServiceCollection services = new ServiceCollection();
            services.AddSingleton(options);
            services.AddTransient<SmtpServer.Protocol.ISmtpCommandFactory, CustomSmtpCommandFactory>();

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, services.BuildServiceProvider());

            System.Threading.Tasks.Task serverTask = server.StartAsync(cancellationTokenSource.Token);

            SampleMailClient.Send();

            cancellationTokenSource.Cancel();
            serverTask.WaitWithoutException();
        }


        public sealed class CustomSmtpCommandFactory 
            : SmtpServer.Protocol.SmtpCommandFactory
        {
            public override SmtpServer.Protocol.SmtpCommand CreateEhlo(string domainOrAddress)
            {
                return new CustomEhloCommand(domainOrAddress);
            }
        }


        public sealed class CustomEhloCommand 
            : SmtpServer.Protocol.EhloCommand
        {
            public CustomEhloCommand(string domainOrAddress) 
                : base(domainOrAddress) 
            { }

            protected override string GetGreeting(SmtpServer.ISessionContext context)
            {
                return "Good morning, Vietnam!";
            }
        }


    }


}