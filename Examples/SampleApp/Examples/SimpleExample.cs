
namespace SampleApp.Examples
{


    public static class SimpleExample 
    {


        public static void Run()
        {
            System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

            SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")
                .Port(25)
                .Build();

            SmtpServer.ComponentModel.ServiceProvider serviceProvider = new SmtpServer.ComponentModel.ServiceProvider();
            serviceProvider.Add(new SampleMessageStore(System.Console.Out));

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, serviceProvider);
            System.Threading.Tasks.Task serverTask = server.StartAsync(cancellationTokenSource.Token);

            SampleMailClient.Send(); 

            cancellationTokenSource.Cancel();
            serverTask.WaitWithoutException();
        }


    }


}