
namespace SampleApp
{


    public class SampleMessageStore 
        : SmtpServer.Storage.MessageStore
    {
        readonly System.IO.TextWriter _writer;

        public SampleMessageStore(System.IO.TextWriter writer)
        {
            _writer = writer;
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
            await using System.IO.MemoryStream stream = new System.IO.MemoryStream();

            System.SequencePosition position = buffer.GetPosition(0);
            while (buffer.TryGet(ref position, out System.ReadOnlyMemory<byte> memory))
            {
                stream.Write(memory.Span);
            }

            stream.Position = 0;

            MimeKit.MimeMessage message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);

            _writer.WriteLine("Subject={0}", message.Subject);
            _writer.WriteLine("Body={0}", message.Body);

            return SmtpServer.Protocol.SmtpResponse.Ok;
        }


    }


}