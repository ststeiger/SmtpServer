
namespace AnySqlSmtpServer.Logging 
{


    public static class SyslogLoggerExtensions
    {

        public static Microsoft.Extensions.Logging.ILoggerFactory AddSyslog(
              this Microsoft.Extensions.Logging.ILoggerFactory factory
            , string host
            , int port
            , System.Func<string, Microsoft.Extensions.Logging.LogLevel, bool> filter 
            = null)
        {
            SysLoggerOptions options = new SysLoggerOptions() { SyslogServerHostname = host, SyslogServerPort = port, Filter = filter };
            factory.AddProvider(new SyslogLoggerProvider(options));
            return factory;
        }

    }


}
