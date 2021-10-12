
namespace SampleApp
{


    public class SampleMailboxFilter 
        : SmtpServer.Storage.MailboxFilter
    {
        readonly System.TimeSpan _delay;


        public SampleMailboxFilter(System.TimeSpan delay)
        {
            _delay = delay;
        }


        public SampleMailboxFilter()
            : this(System.TimeSpan.Zero) 
        { }


        /// <summary>
        /// Returns a value indicating whether the given mailbox can be accepted as a sender.
        /// </summary>
        /// <param name="context">The session context.</param>
        /// <param name="from">The mailbox to test.</param>
        /// <param name="size">The estimated message size to accept.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The acceptance state of the mailbox.</returns>
        public override async System.Threading.Tasks.Task<SmtpServer.Storage.MailboxFilterResult> 
            CanAcceptFromAsync(
            SmtpServer.ISessionContext context,
            SmtpServer.Mail.IMailbox @from, 
            int size,
            System.Threading.CancellationToken cancellationToken)
        {
            // await System.Threading.Tasks.Task.Delay(_delay, cancellationToken);

            if (@from == SmtpServer.Mail.Mailbox.Empty)
            {
                return await System.Threading.Tasks.Task.FromResult(SmtpServer.Storage.MailboxFilterResult.NoPermanently);
            }

            System.Net.IPEndPoint endpoint = (System.Net.IPEndPoint)context.Properties[SmtpServer.Net.EndpointListener.RemoteEndPointKey];

#if false
            if (!endpoint.Address.Equals(System.Net.IPAddress.Parse("127.0.0.1")))
                return await System.Threading.Tasks.Task.FromResult(SmtpServer.Storage.MailboxFilterResult.NoPermanently);
#endif

            return await System.Threading.Tasks.Task.FromResult(SmtpServer.Storage.MailboxFilterResult.Yes);
        }

        /// <summary>
        /// Returns a value indicating whether the given mailbox can be accepted as a recipient to the given sender.
        /// </summary>
        /// <param name="context">The session context.</param>
        /// <param name="to">The mailbox to test.</param>
        /// <param name="from">The sender's mailbox.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The acceptance state of the mailbox.</returns>
        public override async System.Threading.Tasks.Task<SmtpServer.Storage.MailboxFilterResult> 
            CanDeliverToAsync(
            SmtpServer.ISessionContext context,
            SmtpServer.Mail.IMailbox to,
            SmtpServer.Mail.IMailbox @from,
            System.Threading.CancellationToken cancellationToken)
        {
            // await System.Threading.Tasks.Task.Delay(_delay, cancellationToken);

            // return SmtpServer.Storage.MailboxFilterResult.Yes;
            return await System.Threading.Tasks.Task.FromResult(SmtpServer.Storage.MailboxFilterResult.Yes);
        }


    }


}
