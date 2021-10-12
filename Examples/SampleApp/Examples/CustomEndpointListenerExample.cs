
namespace SampleApp.Examples
{


    public static class CustomEndpointListenerExample
    {


        public static void Run()
        {
            System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

            SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()
                .ServerName("SmtpServer SampleApp")
                .Endpoint(builder =>
                    builder
                        .Port(9025, true)
                        .AllowUnsecureAuthentication(false)
                        .Certificate(CreateCertificate()))
                .Build();

            SmtpServer.ComponentModel.ServiceProvider serviceProvider = new SmtpServer.ComponentModel.ServiceProvider();
            serviceProvider.Add(new CustomEndpointListenerFactory());

            SmtpServer.SmtpServer server = new SmtpServer.SmtpServer(options, serviceProvider);

            System.Threading.Tasks.Task serverTask = server.StartAsync(cancellationTokenSource.Token);

            SampleMailClient.Send(useSsl: true);

            cancellationTokenSource.Cancel();
            serverTask.WaitWithoutException();
        }


        public sealed class CustomEndpointListenerFactory 
            : SmtpServer.Net.EndpointListenerFactory
        {
            public override SmtpServer.Net.IEndpointListener CreateListener(SmtpServer.IEndpointDefinition endpointDefinition)
            {
                return new CustomEndpointListener(base.CreateListener(endpointDefinition));
            }
        }


        public sealed class CustomEndpointListener 
            : SmtpServer.Net.IEndpointListener
        {
            readonly SmtpServer.Net.IEndpointListener _endpointListener;

            public CustomEndpointListener(SmtpServer.Net.IEndpointListener endpointListener)
            {
                _endpointListener = endpointListener;
            }

            public void Dispose()
            {
                _endpointListener.Dispose();
            }

            public async System.Threading.Tasks.Task<SmtpServer.IO.ISecurableDuplexPipe> GetPipeAsync(
                SmtpServer.ISessionContext context,
                System.Threading.CancellationToken cancellationToken)
            {
                SmtpServer.IO.ISecurableDuplexPipe pipe = await _endpointListener.GetPipeAsync(context, cancellationToken);

                return new CustomSecurableDuplexPipe(pipe);
            }
        }


        public sealed class CustomSecurableDuplexPipe 
            : SmtpServer.IO.ISecurableDuplexPipe
        {
            readonly SmtpServer.IO.ISecurableDuplexPipe _securableDuplexPipe;

            public CustomSecurableDuplexPipe(SmtpServer.IO.ISecurableDuplexPipe securableDuplexPipe)
            {
                _securableDuplexPipe = securableDuplexPipe;
            }

            public System.Threading.Tasks.Task UpgradeAsync(
                System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                System.Security.Authentication.SslProtocols protocols,
                System.Threading.CancellationToken cancellationToken = default)
            {
                return _securableDuplexPipe.UpgradeAsync(certificate, protocols, cancellationToken);
            }

            public void Dispose()
            {
                _securableDuplexPipe.Dispose();
            }

            public System.IO.Pipelines.PipeReader Input => new LoggingPipeReader(_securableDuplexPipe.Input);

            public System.IO.Pipelines.PipeWriter Output => _securableDuplexPipe.Output;

            public bool IsSecure => _securableDuplexPipe.IsSecure;
        }


        public sealed class LoggingPipeReader 
            : System.IO.Pipelines.PipeReader
        {
            readonly System.IO.Pipelines.PipeReader _delegate;

            public LoggingPipeReader(System.IO.Pipelines.PipeReader @delegate)
            {
                _delegate = @delegate;
            }

            public override void AdvanceTo(System.SequencePosition consumed)
            {
                _delegate.AdvanceTo(consumed);
            }

            public override void AdvanceTo(System.SequencePosition consumed, System.SequencePosition examined)
            {
                _delegate.AdvanceTo(consumed, examined);
            }

            public override void CancelPendingRead()
            {
                _delegate.CancelPendingRead();
            }

            public override void Complete(System.Exception exception = null)
            {
                _delegate.Complete(exception);
            }

            public override async System.Threading.Tasks.ValueTask<System.IO.Pipelines.ReadResult> 
                ReadAsync(
                System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
            {
                System.IO.Pipelines.ReadResult readResult = await _delegate.ReadAsync(cancellationToken);

                System.Console.WriteLine(">>> {0}", SmtpServer.Text.StringUtil.Create(readResult.Buffer));

                return readResult;
            }

            public override bool TryRead(out System.IO.Pipelines.ReadResult result)
            {
                return _delegate.TryRead(out result);
            }
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
