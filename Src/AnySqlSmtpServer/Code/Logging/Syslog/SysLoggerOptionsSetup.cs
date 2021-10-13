
namespace AnySqlSmtpServer.Logging
{


    public class SysLoggerOptionsSetup
    : Microsoft.Extensions.Options.ConfigureFromConfigurationOptions<Logging.SysLoggerOptions>
    {

        public SysLoggerOptionsSetup(
            Microsoft.Extensions.Logging.Configuration.ILoggerProviderConfiguration<Logging.SyslogLoggerProvider>
            providerConfiguration
        )
            : base(providerConfiguration.Configuration)
        { }

    }


}
