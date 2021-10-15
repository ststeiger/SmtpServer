
using Dapper; // Execute
using Microsoft.Extensions.Logging; // LogInformation LogTrace LogError


namespace AnySqlSmtpServer
{


    public class DbMessageStore 
        : SmtpServer.Storage.MessageStore
    {
        protected readonly DB.SqlFactory m_factory;
        protected readonly Microsoft.Extensions.Logging.ILogger<DbMessageStore> m_logger;


        public DbMessageStore(Microsoft.Extensions.Logging.ILogger<DbMessageStore> logger, DB.SqlFactory factory)
        {
            this.m_logger = logger;
            this.m_factory = factory;
        }

        /// <summary>
        /// Save the given message to the underlying storage system.
        /// </summary>
        /// <param name="context">The session context.</param>
        /// <param name="transaction">The SMTP message transaction to store.</param>
        /// <param name="buffer">The buffer that contains the message content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A unique identifier that represents this message in the underlying message store.</returns>
        public override async System.Threading.Tasks.Task<SmtpServer.Protocol.SmtpResponse> SaveAsync(
            SmtpServer.ISessionContext context,
            SmtpServer.IMessageTransaction transaction,
            System.Buffers.ReadOnlySequence<byte> buffer,
            System.Threading.CancellationToken cancellationToken)
        {
            DB.InsertMessageParameters m = new DB.InsertMessageParameters()
            {
                __msg_uid = System.Guid.NewGuid(), 
                __msg_from_host = transaction.From.Host, 
                __msg_from_mailbox = transaction.From.User, 
                __msg_host = transaction.To[0].Host, 
                __msg_mailbox = transaction.To[0].User, 
            };
            

            if (context.Properties.ContainsKey(SmtpServer.Net.EndpointListener.RemoteEndPointKey))
            {
                System.Net.IPEndPoint endp = null;

                try
                {
                    endp = (System.Net.IPEndPoint)context.Properties[SmtpServer.Net.EndpointListener.RemoteEndPointKey];
                    m.__msg_port = endp.Port;
                    m.__msg_addressFamily = endp.AddressFamily.ToString();
                    m.__msg_unmapped = endp.Address.ToString();
                }
                catch (System.Exception exUnmapped)
                {
                    m.__msg_unmapped = exUnmapped.Message + "\r\n" + exUnmapped.StackTrace + "\r\n" + exUnmapped.Source;
                }

                try
                {
                    // if (endp.Address.IsIPv4MappedToIPv6)
                    m.__msg_ip_v4 = endp.Address.MapToIPv4().ToString();
                }
                catch (System.Exception exV4)
                {
                    m.__msg_ip_v4 = exV4.Message + "\r\n" + exV4.StackTrace + "\r\n" + exV4.Source;
                }

                try
                {
                    m.__msg_ip_v6 = endp.Address.MapToIPv6().ToString();
                }
                catch (System.Exception exV6)
                {
                    m.__msg_ip_v6 = exV6.Message + "\r\n" + exV6.StackTrace + "\r\n" + exV6.Source;
                }

            }

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {

                try
                {
                    System.SequencePosition position = buffer.GetPosition(0);
                    while (buffer.TryGet(ref position, out System.ReadOnlyMemory<byte> memory))
                    {
                        // stream.Write(memory.Span);
                        await stream.WriteAsync(memory, cancellationToken);
                    }

                    stream.Position = 0;

                    m.__msg_bytes = stream.ToArray();
                    // string str = System.Text.Encoding.UTF8.GetString(m.__msg_bytes);
                    // this.m_logger.LogInformation(str);

                    MimeKit.MimeMessage message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);
                    m.__msg_id = message.MessageId;
                    m.__msg_subject = message.Subject;
                    m.__msg_body = message.Body.ToString();
                }
                catch (System.Exception exMessage)
                {
                    m.__msg_id = "";
                    m.__msg_subject = exMessage.Message;
                    m.__msg_body = exMessage.Message + "\r\n" + exMessage.StackTrace + "\r\n" + exMessage.Source;

                    this.m_logger.LogError(exception: exMessage, message: "Error getting mail-content in Task SaveAsync");
                }

            }

            try
            {
                using (System.Data.Common.DbConnection cnn = this.m_factory.Connection)
                {
                    cnn.Execute(DB.Commands.Insert_Message, m);

                    if (cnn.State != System.Data.ConnectionState.Closed)
                        cnn.Close();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
                this.m_logger.LogError(exception: ex, message: "Error DB-access in Task SaveAsync");
            }

            return SmtpServer.Protocol.SmtpResponse.Ok;
        } // End Task SaveAsync 


    } // End Class DbMessageStore 


} // End Namespace 
