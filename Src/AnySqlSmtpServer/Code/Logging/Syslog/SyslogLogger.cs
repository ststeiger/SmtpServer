
namespace AnySqlSmtpServer.Logging
{


    public class SyslogLogger
            : Microsoft.Extensions.Logging.ILogger
    {
        protected const int SyslogFacility = 16;

        protected string m_categoryName;
        protected SysLoggerOptions m_settings;


        // protected readonly System.Func<string, Microsoft.Extensions.Logging.LogLevel, bool> m_filter;


        public SyslogLogger(
             string categoryName,
             SysLoggerOptions options)
        {
            this.m_categoryName = categoryName;
            this.m_settings = options;
        } // End Constructor 


        public System.IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        } // End Function BeginScope 


        public bool IsEnabled(
            Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return (this.m_settings.Filter == null 
                || this.m_settings.Filter(this.m_categoryName, logLevel)
            );
        }

        public void Log<TState>(
              Microsoft.Extensions.Logging.LogLevel logLevel
            , Microsoft.Extensions.Logging.EventId eventId
            , TState state
            , System.Exception exception
            , System.Func<TState, System.Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            } // End if (!IsEnabled(logLevel)) 

            if (formatter == null)
            {
                throw new System.ArgumentNullException(nameof(formatter));
            } // End if (formatter == null) 

            string message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            } // End if (string.IsNullOrEmpty(message)) 

            message = $"{ logLevel }: {message}";

            if (exception != null)
            {
                message += System.Environment.NewLine 
                    + System.Environment.NewLine 
                    + exception.ToString();
            } // End if (exception != null) 

            
            Send(logLevel, message);
        } // End Sub Log 


        internal void Send(Microsoft.Extensions.Logging.LogLevel mslogLevel, string message)
        {
            if (string.IsNullOrWhiteSpace(this.m_settings.SyslogServerHostname) || this.m_settings.SyslogServerPort <= 0)
            {
                return;
            } // End if (string.IsNullOrWhiteSpace(this.m_host) || this.m_port <= 0) 

            // https://stackoverflow.com/questions/1233217
            // System.Environment.MachineName and 
            // System.Windows.Forms.SystemInformation.ComputerName 
            // are identical and returns the computer's NetBIOS name. 
            // This name is restricted to 15 characters 
            // and only visible on the LAN.

            // System.Environment.GetEnvironmentVariable("COMPUTERNAME") 
            // returns the computer name set during installation.
            // NetBIOS and hostname are initially set to the same name.

            // System.Net.Dns.GetHostName() returns the computer's 
            // TCP/IP based hostname. By adding a domain suffix 
            // to the hostname you can resolve your computer's IP address 
            // across LANs / on the internet.

            // System.Console.WriteLine(options);
            SyslogNet.Client.Severity logLevel = MapToSyslogSeverity(mslogLevel);


            SyslogNet.Client.SyslogMessage msg = new SyslogNet.Client.SyslogMessage(
                 System.DateTimeOffset.Now
                , SyslogNet.Client.Facility.UserLevelMessages
                // ,SyslogNet.Client.Severity.Error
                , logLevel
                , this.m_settings.LocalHostName
                , this.m_settings.AppName
                , this.m_settings.ProcId
                , this.m_settings.MsgType
                , message ?? (this.m_settings.Message ??
                      "Test message at "
                    + System.DateTime.UtcNow.ToString("dddd, dd.MM.yyyy HH:mm:ss.fff"
                    , System.Globalization.CultureInfo.InvariantCulture)
                )
            );

            msg.Send(this.m_settings);


            /*
            SyslogLogLevel logLevel2 = MapToSyslogLevel(mslogLevel);
            string hostName = System.Net.Dns.GetHostName();
            int level = SyslogFacility * 8 + (int)logLevel2;
            
            string logMessage = string.Format("<{0}>{1} {2}"
                , level, hostName, message
            );

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(logMessage);

            using (System.Net.Sockets.UdpClient client = 
                new System.Net.Sockets.UdpClient())
            {
                client.Send(
                      bytes
                    , bytes.Length
                    , this.m_host
                    , this.m_port
                );
            } // End Using client 
            */
        } // End Sub Send 

        // SyslogNet.Client.Severity.Informational
        // 
        private SyslogNet.Client.Severity MapToSyslogSeverity(
            Microsoft.Extensions.Logging.LogLevel level)
        {
            if (level == Microsoft.Extensions.Logging.LogLevel.Critical)
                return SyslogNet.Client.Severity.Critical;

            if (level == Microsoft.Extensions.Logging.LogLevel.Debug)
                return SyslogNet.Client.Severity.Debug;

            if (level == Microsoft.Extensions.Logging.LogLevel.Error)
                return SyslogNet.Client.Severity.Error;

            if (level == Microsoft.Extensions.Logging.LogLevel.Information)
                return SyslogNet.Client.Severity.Informational;

            if (level == Microsoft.Extensions.Logging.LogLevel.None)
                return SyslogNet.Client.Severity.Informational;

            if (level == Microsoft.Extensions.Logging.LogLevel.Trace)
                return SyslogNet.Client.Severity.Informational;

            if (level == Microsoft.Extensions.Logging.LogLevel.Warning)
                return SyslogNet.Client.Severity.Warning;

            return SyslogNet.Client.Severity.Informational;
        } // End Function MapToSyslogLevel 


        private SyslogLogLevel MapToSyslogLevel(
            Microsoft.Extensions.Logging.LogLevel level)
        {
            if (level == Microsoft.Extensions.Logging.LogLevel.Critical)
                return SyslogLogLevel.Critical;

            if (level == Microsoft.Extensions.Logging.LogLevel.Debug)
                return SyslogLogLevel.Debug;

            if (level == Microsoft.Extensions.Logging.LogLevel.Error)
                return SyslogLogLevel.Error;

            if (level == Microsoft.Extensions.Logging.LogLevel.Information)
                return SyslogLogLevel.Info;

            if (level == Microsoft.Extensions.Logging.LogLevel.None)
                return SyslogLogLevel.Info;

            if (level == Microsoft.Extensions.Logging.LogLevel.Trace)
                return SyslogLogLevel.Info;

            if (level == Microsoft.Extensions.Logging.LogLevel.Warning)
                return SyslogLogLevel.Warn;

            return SyslogLogLevel.Info;
        } // End Function MapToSyslogLevel 


    } // End Class SyslogLogger 


} // End Namespace SSRS_Manager.Trash_ForReference 
