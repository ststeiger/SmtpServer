
namespace AnySqlSmtpServer
{


    public class PrototypeMailboxFilter 
        : SmtpServer.Storage.MailboxFilter
    {
        
        protected readonly Microsoft.Extensions.Logging.ILogger<PrototypeMailboxFilter> m_logger;

        protected readonly System.Collections.Generic.List<string> m_allowedDomains;


        public PrototypeMailboxFilter()
        {
            this.m_allowedDomains = new System.Collections.Generic.List<string>() {
                 "henri-bernhard.ch", "www.henri-bernhard.ch"
                ,"daniel-steiger.ch", "www.daniel-steiger.ch"
            };

        }


        public static void dkim()
        {
            // https://schwabencode.com/blog/2020/10/16/net-emails-mit-dkim-signieren-mimekit
            // http://www.mimekit.net/docs/html/T_MimeKit_Cryptography_DkimSigner.htm
            // https://www.codeproject.com/Questions/5306635/How-to-sign-emails-with-DKIM-and-mimekit-package
            // https://github.com/jstedfast/MimeKit/issues/251
            // https://stackoverflow.com/questions/2358095/how-to-domainkeys-dkim-email-signing-using-the-c-sharp-smtp-client
            // https://github.com/ONLYOFFICE/CommunityServer/tree/master/module/ASC.Mail/ASC.Mail/Core/Dao
        }


        public static async System.Threading.Tasks.Task spf(string domain, string senderAddress)
        {
            // domain = "example.com";
            // senderAddress = "sender@example.com";

            // https://postmarkapp.com/guides/spf
            // https://www.agari.com/email-security-blog/what-is-spf/
            // https://www.validity.com/blog/how-to-build-your-spf-record-in-5-simple-steps/
            // https://mxtoolbox.com/problem/spf/spf-record
            // https://stackoverflow.com/questions/64634743/set-return-path-using-mailkit
            // https://webmasters.stackexchange.com/questions/27910/txt-vs-spf-record-for-google-servers-spf-record-either-or-both
            // Also RFC 7208 obsoletes 4408 and states: SPF records MUST be published as a DNS TXT(type 16) Resource Record(RR) [RFC1035] only.
            // The use of alternative DNS RR types that was formerly supported during the experimental phase of SPF was discontinued in 2014.
            // SPF records must now only be published as a DNS TXT(type 16) Resource Record(RR) [RFC1035].
            // See RFC 7208 for further detail on this change.
            // https://www.libspf2.org/
            // https://dankaminsky.com/dns-txt-record-parsing-bug-in-libspf2/

            // https://stackoverflow.com/questions/11846705/get-spf-records-from-a-domain/11884089#11884089
            // https://docs.ar-soft.de/arsoft.tools.net/#Welcome.html
            // https://github.com/martindevans/Hellequin-p2p-dns/blob/master/p2p-DNS/ARSoft.Tools.Net/Spf/SpfCheckHostParameter.cs
            // https://github.com/martindevans/Hellequin-p2p-dns/blob/master/p2p-DNS/ARSoft.Tools.Net/Spf/SpfRecord.cs
            // https://randronov.blogspot.com/2013/04/lookup-spf-record-in-c.html

            ARSoft.Tools.Net.Spf.SpfValidator spfValidator = new ARSoft.Tools.Net.Spf.SpfValidator();
            System.Net.IPAddress mailIpAddress = System.Net.IPAddress.Parse("X.X.X.X"); // @nimi the X.X.X.X is a placeholder for the real IP you want to test. 
            // @nimi if you have multiple Mail Servers(meaning more IPs to test), simply iterate through the available IPs and run the above check for each of them.

            ARSoft.Tools.Net.Spf.SpfQualifier result = spfValidator.CheckHost(mailIpAddress, domain, senderAddress).Result;

            await System.Threading.Tasks.Task.CompletedTask;
        }


        public static void foo(string heloDomain, string localDomain)
        {
            // heloDomain = "example.com";
            // localDomain = "receivingmta.example.com";
            string localIp = "192.0.2.1";

            ARSoft.Tools.Net.Spf.SpfValidator validator = new ARSoft.Tools.Net.Spf.SpfValidator()
            {
                HeloDomain = ARSoft.Tools.Net.DomainName.Parse(heloDomain),
                LocalDomain = ARSoft.Tools.Net.DomainName.Parse(localDomain),
                LocalIP = System.Net.IPAddress.Parse(localIp)
            };

            ARSoft.Tools.Net.Spf.SpfQualifier result = validator.CheckHost(
                  System.Net.IPAddress.Parse("192.0.2.200")
                , ARSoft.Tools.Net.DomainName.Parse("example.com")
                , "sender@example.com"
            ).Result;
        }



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

            if (@from == SmtpServer.Mail.Mailbox.Empty || @from == null)
            {
                return await System.Threading.Tasks.Task.FromResult(SmtpServer.Storage.MailboxFilterResult.NoPermanently);
            }
            // get mx record of from domain 
            // check if smtp of from-domain supports spf


#if false
            System.Net.IPEndPoint endpoint = (System.Net.IPEndPoint)context.Properties[SmtpServer.Net.EndpointListener.RemoteEndPointKey];
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

            for (int i = 0; i < this.m_allowedDomains.Count; ++i)
            {
                if (this.m_allowedDomains[i].Equals(to.Host, System.StringComparison.InvariantCultureIgnoreCase))
                    return await System.Threading.Tasks.Task.FromResult(SmtpServer.Storage.MailboxFilterResult.Yes);
            }

            return await System.Threading.Tasks.Task.FromResult(SmtpServer.Storage.MailboxFilterResult.NoPermanently);
        }


    }


}
