
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AnySqlSmtpServer 
{


    public static class SysLoggerExtensions 
    {

        static public Microsoft.Extensions.Logging.ILoggingBuilder AddSysLogger(
            this Microsoft.Extensions.Logging.ILoggingBuilder builder,
            System.Action<Logging.SysLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new System.ArgumentNullException(nameof(configure));
            }

            builder.Services.TryAddEnumerable(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton<
                    Microsoft.Extensions.Logging.ILoggerProvider,
                    Logging.SyslogLoggerProvider
                >()
            );

            builder.Services.TryAddEnumerable(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton
                <IConfigureOptions<Logging.SysLoggerOptions>, Logging.SysLoggerOptionsSetup>());

            builder.Services.TryAddEnumerable(
                Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton
                <
                    IOptionsChangeTokenSource<Logging.SysLoggerOptions>,
                    LoggerProviderOptionsChangeTokenSource<Logging.SysLoggerOptions
                    , Logging.SyslogLoggerProvider>
                >());

            builder.Services.Configure(configure);

            return builder;
        }


    }
}
