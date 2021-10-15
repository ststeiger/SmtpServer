using System;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace SmtpServer.IO
{
    internal sealed class SecurableDuplexPipe : ISecurableDuplexPipe
    {
        readonly Action _disposeAction;
        Stream _stream;
        bool _disposed;
        readonly System.Net.IPAddress _clientAddress;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream that the pipe is reading and writing to.</param>
        /// <param name="disposeAction">The action to execute when the stream has been disposed.</param>
        internal SecurableDuplexPipe(Stream stream, Action disposeAction, System.Net.IPAddress clientAddress)
        {
            _stream = stream;
            _disposeAction = disposeAction;
            _clientAddress = clientAddress;

            Input = PipeReader.Create(_stream);
            Output = PipeWriter.Create(_stream);
        }

        /// <summary>
        /// Upgrade to a secure pipeline.
        /// </summary>
        /// <param name="certificate">The X509Certificate used to authenticate the server.</param>
        /// <param name="protocols">The value that represents the protocol used for authentication.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that asynchronously performs the operation.</returns>
        public async Task UpgradeAsync(ServerCertificateSelectionCallback certificate, SslProtocols protocols, CancellationToken cancellationToken = default)
        {
            // var stream = new SslStream(_stream, true);


            StreamExtended.DefaultBufferPool bufferPool = new StreamExtended.DefaultBufferPool();

            StreamExtended.Network.CustomBufferedStream yourClientStream =
                new StreamExtended.Network.CustomBufferedStream(_stream, bufferPool, 4096);

            StreamExtended.ClientHelloInfo clientSslHelloInfo =
                await StreamExtended.SslTools.PeekClientHello(yourClientStream, bufferPool);


            string sniHostName = null;

            //will be null if no client hello was received (not a SSL connection)
            if (clientSslHelloInfo != null)
            {
                sniHostName = clientSslHelloInfo.Extensions?.FirstOrDefault(x => x.Key == "server_name").Value?.Data;
            }

            if (string.IsNullOrWhiteSpace(sniHostName))
                sniHostName = _clientAddress.MapToIPv4().ToString();

            System.Net.Security.SslStream stream = new System.Net.Security.SslStream(yourClientStream, true);
            X509Certificate cert = certificate(stream, sniHostName);
            await stream.AuthenticateAsServerAsync(cert, false, protocols, true).ConfigureAwait(false);
            _stream = stream;
            
            Input = PipeReader.Create(_stream);
            Output = PipeWriter.Create(_stream);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the stream and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    _disposeAction();
                    _stream = null;
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Gets the <see cref="T:System.IO.Pipelines.PipeReader" /> half of the duplex pipe.
        /// </summary>
        public PipeReader Input { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:System.IO.Pipelines.PipeWriter" /> half of the duplex pipe.
        /// </summary>
        public PipeWriter Output { get; private set; }

        /// <summary>
        /// Returns a value indicating whether or not the current pipeline is secure.
        /// </summary>
        public bool IsSecure => _stream is SslStream;
    }
}