
namespace AnySqlSmtpServer.Logging
{


    public class SyslogLoggerProvider 
        : Microsoft.Extensions.Logging.ILoggerProvider
    {

        protected SysLoggerOptions m_settings;


        public SyslogLoggerProvider(SysLoggerOptions settings)
        {
            this.m_settings = settings;
        } // End Constructor 


        public SyslogLoggerProvider(Microsoft.Extensions.Options.IOptionsMonitor<SysLoggerOptions> settings)
            : this(settings.CurrentValue)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/change-tokens

            // this.SettingsChangeToken = Settings.OnChange((settings, b) => { this.Settings = settings; });
        } // End Constructor 


        public Microsoft.Extensions.Logging.ILogger CreateLogger(
            string categoryName
        )
        {
            return new SyslogLogger(categoryName, this.m_settings);
        } // End Function CreateLogger 


        public void Dispose()
        { }


    } // End Class SyslogLoggerProvider 


} // End Namespace SSRS_Manager.Trash_ForReference 
