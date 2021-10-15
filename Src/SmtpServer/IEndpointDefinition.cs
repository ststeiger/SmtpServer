using System;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace SmtpServer
{

    /// <summary>
    /// Selects the server Transport Layer Security (TLS) certificate.
    /// </summary>
    /// <param name="sender">A System.Net.Security.SslStream object</param>
    /// <param name="hostName">The host name requested by the client. 
    /// If the client doesn't use the host_name TLS extension, the hostName is an empty string.
    /// </param>
    /// <returns>An System.Security.Cryptography.X509Certificates.X509Certificate used for establishing a TLS connection</returns>
    public delegate X509Certificate ServerCertificateSelectionCallback(object sender, string hostName);
    // System.Net.Security.ServerCertificateSelectionCallback


    public class sam
    {
        /// <summary>
        /// Selects the server Transport Layer Security (TLS) certificate.
        /// </summary>
        /// <param name="sender">A System.Net.Security.SslStream object</param>
        /// <param name="ip">IP-Address, if hostname is not set</param>
        /// <param name="serverName">The host name requested by the client. 
        /// If the client doesn't use the host_name TLS extension, the hostName is an empty string.
        /// </param>
        /// <returns>An System.Security.Cryptography.X509Certificates.X509Certificate used for establishing a TLS connection</returns>
        public X509Certificate SelectServerCertificate(object sender, string ip, string serverName)
        {
            System.Net.Security.SslStream stream = (System.Net.Security.SslStream)sender;

            return null;
        }
    }

    



    public interface IEndpointDefinition
    {
        /// <summary>
        /// The IP endpoint to listen on.
        /// </summary>
        IPEndPoint Endpoint { get; }

        /// <summary>
        /// Indicates whether the endpoint is secure by default.
        /// </summary>
        bool IsSecure { get; }

        /// <summary>
        /// Gets a value indicating whether the client must authenticate in order to proceed.
        /// </summary>
        bool AuthenticationRequired { get; }

        /// <summary>
        /// Gets a value indicating whether authentication should be allowed on an unsecure session.
        /// </summary>
        bool AllowUnsecureAuthentication { get; }

        /// <summary>
        /// The timeout on each individual buffer read.
        /// </summary>
        TimeSpan ReadTimeout { get; }

        /// <summary>
        /// Gets the Server Certificate to use when starting a TLS session.
        /// </summary>
        // X509Certificate ServerCertificate { get; }
        ServerCertificateSelectionCallback ServerCertificate { get; }

        /// <summary>
        /// The supported SSL protocols.
        /// </summary>
        SslProtocols SupportedSslProtocols { get; }
    }
}