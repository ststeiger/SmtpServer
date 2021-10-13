
namespace AnySqlSmtpServer.Logging
{


    public class SysLoggerOptions
        : SyslogNet.Client.SyslogOptions
    {


        public System.Func<string, Microsoft.Extensions.Logging.LogLevel, bool> Filter;

        public SysLoggerOptions()
        {  }

    }


}
