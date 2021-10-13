
namespace SampleApp
{


    public class SampleMessageStore
        : SmtpServer.Storage.MessageStore
    {

        protected readonly System.IO.TextWriter m_writer;


        public SampleMessageStore(System.IO.TextWriter writer)
        {
            this.m_writer = writer;
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
            MimeKit.MimeMessage message = null;
            byte[] msg = null;


            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                System.SequencePosition position = buffer.GetPosition(0);
                while (buffer.TryGet(ref position, out System.ReadOnlyMemory<byte> memory))
                {
                    // stream.Write(memory.Span);
                    await stream.WriteAsync(memory, cancellationToken);
                }

                stream.Position = 0;


                msg = stream.ToArray();
                // string str = System.Text.Encoding.UTF8.GetString(msg);
                // System.Console.WriteLine(str);


                message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);
                string fileName = message.MessageId + ".eml";

                string messagePath = System.IO.Path.Combine(@"E:\", fileName);
                message.WriteTo(messagePath);
            }

            if (message != null)
            {
                this.m_writer.WriteLine("Subject={0}", message.Subject);
                this.m_writer.WriteLine("Body={0}", message.Body);
            } // End if (message != null) 

            return SmtpServer.Protocol.SmtpResponse.Ok;
        } // End Task SaveAsync 


    } // End Class 


} // End Namespace 
