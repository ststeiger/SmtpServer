﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SmtpServer.IO;

namespace SmtpServer.Net
{
    public sealed class EndpointListener : IEndpointListener
    {
        public const string LocalEndPointKey = "EndpointListener:LocalEndPoint";
        public const string RemoteEndPointKey = "EndpointListener:RemoteEndPoint";

        readonly IEndpointDefinition _endpointDefinition;
        readonly TcpListener _tcpListener;
        readonly Action _disposeAction;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpointDefinition">The endpoint definition to create the listener for.</param>
        /// <param name="tcpListener">The TCP listener for the endpoint.</param>
        /// <param name="disposeAction">The action to execute when the listener has been disposed.</param>
        internal EndpointListener(IEndpointDefinition endpointDefinition, TcpListener tcpListener, Action disposeAction)
        {
            _endpointDefinition = endpointDefinition;
            _tcpListener = tcpListener;
            _disposeAction = disposeAction;
        }

        /// <summary>
        /// Returns a securable pipe to the endpoint.
        /// </summary>
        /// <param name="context">The session context that the pipe is being created for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The securable pipe from the endpoint.</returns>
        public async Task<ISecurableDuplexPipe> GetPipeAsync(ISessionContext context, CancellationToken cancellationToken)
        {
            TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync().WithCancellation(cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            context.Properties.Add(LocalEndPointKey, _tcpListener.LocalEndpoint);
            context.Properties.Add(RemoteEndPointKey, tcpClient.Client.RemoteEndPoint);

            NetworkStream stream = tcpClient.GetStream();
            stream.ReadTimeout = (int)_endpointDefinition.ReadTimeout.TotalMilliseconds;

            return new SecurableDuplexPipe(stream, 
                delegate() 
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
                }
                , ((System.Net.IPEndPoint)tcpClient.Client.RemoteEndPoint).Address
            );
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _tcpListener.Stop();
            _disposeAction();
        }
    }
}