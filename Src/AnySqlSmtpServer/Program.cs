
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;


namespace AnySqlSmtpServer
{


    public class Program
    {


        private static Logging.TrivialLogger s_logger;

        private static string s_ProgramDirectory;
        private static string s_CurrentDirectory;
        private static string s_BaseDirectory;
        private static string s_ExecutablePath;
        private static string s_ExecutableDirectory;
        private static string s_Executable;
        private static string s_ContentRootDirectory;


        private static void DisplayError(System.Exception ex)
        {
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(System.Environment.NewLine);

            System.Exception thisError = ex;
            while (thisError != null)
            {
                System.Console.WriteLine(thisError.Message);
                System.Console.WriteLine(thisError.StackTrace);

                if (thisError.InnerException != null)
                {
                    System.Console.WriteLine(System.Environment.NewLine);
                    System.Console.WriteLine("Inner Exception:");
                } // End if (thisError.InnerException != null) 

                thisError = thisError.InnerException;
            } // Whend 

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(System.Environment.NewLine);
        } // End Sub DisplayError 

        static Program()
        {
            try
            {
                s_ProgramDirectory = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                s_CurrentDirectory = System.IO.Directory.GetCurrentDirectory();
                s_BaseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                s_ExecutablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                s_ExecutableDirectory = System.IO.Path.GetDirectoryName(s_ExecutablePath);
                s_Executable = System.IO.Path.GetFileNameWithoutExtension(s_ExecutablePath);

                string logFilePath = null;
                string fileName = @"ServiceStartupLog.htm";

                if ("dotnet".Equals(s_Executable, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    s_ContentRootDirectory = s_ProgramDirectory;
                    logFilePath = System.IO.Path.Combine(s_ProgramDirectory, fileName);
                }
                else
                {
                    s_ContentRootDirectory = s_ExecutableDirectory;
                    logFilePath = System.IO.Path.Combine(s_ExecutableDirectory, fileName);
                }

                if (System.IO.File.Exists(logFilePath))
                    System.IO.File.Delete(logFilePath);

                s_logger = new Logging.HtmlLogger(logFilePath);

                s_logger.Log(Logging.LogLevel_t.Information, "Program Directory: {0}", s_ProgramDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Current Directory: {0}", s_CurrentDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Base Directory: {0}", s_BaseDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Logfile Directory: {0}", s_ContentRootDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Executable Path: {0}", s_ExecutablePath);
                s_logger.Log(Logging.LogLevel_t.Information, "Executable Directory: {0}", s_ExecutableDirectory);
                s_logger.Log(Logging.LogLevel_t.Information, "Executable: {0}", s_Executable);

                CertificateCallback.Initialize();
            } // End Try 
            catch (System.Exception ex)
            {
                DisplayError(ex);
                System.Environment.Exit(ex.HResult);
            } // End Catch 

        } // End Static Constructor 

        public static void Main(string[] args)
        {
            // CreateDefaultHostBuilder(args).Build().Run();
            CreateCustomHostBuilder(args).Build().Run();
        } // End Sub Main 


        public static IHostBuilder CreateCustomHostBuilder(string[] args)
        {
            IHostBuilder builder = new HostBuilder();
            try
            {
                builder.UseContentRoot(s_ContentRootDirectory);

                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-3.1&tabs=visual-studio#app-configuration
                    // Requires Microsoft.Extensions.Hosting.WindowsServices
                    builder.UseWindowsService();
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                {
                    // https://devblogs.microsoft.com/dotnet/net-core-and-systemd/
                    // Requires Microsoft.Extensions.Hosting.WindowsServices
                    builder.UseSystemd(); // Add: Microsoft.Extensions.Hosting.Systemd
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                {
                    throw new System.NotImplementedException("Service for OSX Platform is NOT implemented.");
                }
                else
                {
                    throw new System.NotSupportedException("This Platform is NOT supported.");
                }


                builder.ConfigureHostConfiguration(
                    delegate (IConfigurationBuilder config)
                    {
                    // Has no effect ... 
                    // s_logger.Log(Logging.LogLevel_t.Information, "SetBasePath: {0}", s_ExecutableDirectory);
                    // config.SetBasePath(s_ExecutableDirectory);

                    config.AddEnvironmentVariables(prefix: "DOTNET_");
                        if (args != null)
                        {
                            config.AddCommandLine(args);
                        } // End if (args != null) 

                    } // End Delegate 
                );

                builder.ConfigureAppConfiguration(
                        delegate (HostBuilderContext hostingContext, IConfigurationBuilder config)
                        {
                            IHostEnvironment env = hostingContext.HostingEnvironment;
                        // Completely wrong ... 
                        // s_logger.Log(Logging.LogLevel_t.Information, "ContentRootPath: {0}", env.ContentRootPath);

                        config.SetBasePath(s_ExecutableDirectory);

                            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                            if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                            {
                                System.Reflection.Assembly appAssembly =
                                    System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(env.ApplicationName));

                                if (appAssembly != null)
                                {
                                    config.AddUserSecrets(appAssembly, optional: true);
                                } // End if (appAssembly != null) 

                            } // End if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))

                            config.AddEnvironmentVariables();

                            if (args != null)
                            {
                                config.AddCommandLine(args);
                            } // End if (args != null) 

                        } // End Delegate 
                    )
                    .ConfigureLogging(
                        delegate (HostBuilderContext hostingContext, ILoggingBuilder logging)
                        {
                            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                                    System.Runtime.InteropServices.OSPlatform.Windows
                            );

                            bool isLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                                    System.Runtime.InteropServices.OSPlatform.Linux
                            );

                            // IMPORTANT: This needs to be added *before* configuration is loaded, this lets
                            // the defaults be overridden by the configuration.
                            if (isWindows)
                            {
                                // Default the EventLogLoggerProvider to warning or above
                                logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                            } // End if (isWindows) 

                            // logging.ClearProviders();
                            // logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                            
                            //string logDir = System.IO.Path.Combine(s_ContentRootDirectory, "Log");

                            //if (!System.IO.Directory.Exists(logDir))
                            //    System.IO.Directory.CreateDirectory(logDir);


                            logging.SetMinimumLevel(LogLevel.Information);
                            // logging.AddFilter(x => x >= LogLevel.Trace);
                            logging.AddFilter(
                                delegate (string categoryName, LogLevel b) 
                                {
                                    if (categoryName.StartsWith("Microsoft.")) return false;
                                    return true;
                                }
                            );


                            logging.AddSysLogger(
                                delegate (Logging.SysLoggerOptions options)
                                {
                                    // options.SyslogVersion = SyslogNet.Client.SyslogVersions.Rfc3164;
                                    // options.SyslogVersion = SyslogNet.Client.SyslogVersions.Rfc5424;
                                    // options.AppName = System.AppDomain.CurrentDomain.FriendlyName;
                                    // options.ProcId = System.Diagnostics.Process.GetCurrentProcess().Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
                                    // options.LocalHostName = System.Environment.MachineName;
                                    // options.MsgType = "msg-id";
                                    options.Message = "Default";

                                    options.SyslogServerHostname = "127.0.0.1";
                                    options.NetworkProtocol = SyslogNet.Client.NetworkProtocols.UPD;
                                    options.InferDefaultPort();
                                    // options.SyslogServerPort = 514; // UPD: 514 , TCP: 601, TLS: 6514
                                    // options.InferNetworkProtocol();
                                    
                                    options.Filter = delegate(string categoryName, LogLevel b) {
                                        if (categoryName.StartsWith("Microsoft."))
                                            return false;

                                        return true; 
                                    };
                                }
                            );


                            logging.AddConsole();
                            logging.AddDebug();
                            logging.AddEventSourceLogger(); // is for Event Tracing.
                                                            // this uses Event Tracing for Windows (ETW)
                                                            // https://docs.microsoft.com/en-us/windows/win32/etw/event-tracing-portal

                            if(isLinux)
                                logging.AddSystemdConsole();

                            if (isWindows)
                                logging.AddEventLog(); // Add the EventLogLoggerProvider for Windows Event Log on windows machines
                            
                        } // End Delegate 
                    )
                    .ConfigureServices(
                        delegate (HostBuilderContext hostingContext, IServiceCollection services)
                        {
                            // AWSSDK.Extensions.NETCore.Setup
                            // AWSSDK.SQS
                            // Microsoft.VisualStudio.Azure.Containers.Tools.Targets

                            // AWS Configuration
                            // AWSOptions options = hostingContext.Configuration.GetAWSOptions();
                            // services.AddDefaultAWSOptions(options);
                            // services.AddAWSService<IAmazonSQS>();

                            // System.IServiceProvider isp = services.BuildServiceProvider();
                            // ILogger<ConfigurationHelpers> logger = isp.GetRequiredService<ILogger<ConfigurationHelpers>>();
                            // ConfigurationHelpers.Logger = logger;


                            // string iniFileName = typeof(Program).Assembly.GetName().Name + ".ini";
                            // Configuration.Settings config = ConfigurationHelpers.GetConfig(s_ContentRootDirectory, iniFileName);
                            // services.AddSingleton(config);


                            services.AddSingleton<DB.SqlFactory>();
                            services.AddSingleton<SmtpServer.Storage.IMessageStore, DbMessageStore>();
                            services.AddSingleton<SmtpServer.Authentication.IUserAuthenticator, DummyUserAuthenticator>();
                            services.AddSingleton<SmtpServer.Storage.IMailboxFilter, PrototypeMailboxFilter>();

                            services.AddSingleton(
                                delegate (System.IServiceProvider provider)
                                {
                                    SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()

                                        .ServerName("AnySql SMTP Server")
                                        .Endpoint(builder => builder.Port(25).IsSecure(false))

#if false 
                                    // For a brief period in time this was a recognized port whereby
                                    // TLS was enabled by default on the connection. When connecting to
                                    // port 465 the client will upgrade its connection to SSL before
                                    // doing anything else. Port 465 is obsolete in favor of using
                                    // port 587 but it is still available by some mail servers.
                                    .Endpoint(builder => 
                                        builder
                                            .Port(465)
                                            .IsSecure(true) // indicates that the client will need to upgrade to SSL upon connection
                                            .Certificate(delegate (object sender, string hostname)
                                            {
                                                return new System.Security.Cryptography.X509Certificates.X509Certificate2();
                                            }).SupportedSslProtocols(System.Security.Authentication.SslProtocols.Tls12)) // requires a valid certificate to be configured

                                    // Port 587 is the default port that should be used by modern mail
                                    // clients. When a certificate is provided, the server will advertise
                                    // that is supports the STARTTLS command which allows the client
                                    // to determine when they want to upgrade the connection to SSL. 
                                    .Endpoint(builder => 
                                        builder
                                            .Port(587)
                                            // Can also be used without ssl ? 
                                            // .IsSecure(true) // indicates that the client will need to upgrade to SSL upon connection
                                            .AllowUnsecureAuthentication(false) // using 'false' here means that the user cant authenticate unless the connection is secure
                                            .Certificate(delegate (object sender, string hostname)
                                            {
                                                return new System.Security.Cryptography.X509Certificates.X509Certificate2();
                                            }).SupportedSslProtocols(System.Security.Authentication.SslProtocols.Tls12)) // requires a valid certificate to be configured
#endif

                                        .Build();

                                    return new SmtpServer.SmtpServer(options, provider.GetRequiredService<System.IServiceProvider>());
                                }
                            );



                            services.AddHostedService<Worker>();
                        } // End Delegate 
                    )
                    .UseDefaultServiceProvider(
                        delegate (HostBuilderContext hostingContext, ServiceProviderOptions options)
                        {
                            bool isDevelopment = hostingContext.HostingEnvironment.IsDevelopment();
                            options.ValidateScopes = isDevelopment;
                            options.ValidateOnBuild = isDevelopment;
                        } // End Delegate 
                    );
            }
            catch (System.Exception ex)
            {
                s_logger.Log(Logging.LogLevel_t.Configuration, "EXXX: {0}", ex);

                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
                System.Console.WriteLine();
                System.Console.WriteLine(System.Environment.NewLine);
                System.Environment.Exit(1);
            }
            return builder;
        }


        public static IHostBuilder CreateDefaultHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        services.AddSingleton<DB.SqlFactory>();
                        services.AddSingleton<SmtpServer.Storage.IMessageStore, DbMessageStore>();
                        services.AddSingleton<SmtpServer.Authentication.IUserAuthenticator, DummyUserAuthenticator>();
                        services.AddSingleton<SmtpServer.Storage.IMailboxFilter, PrototypeMailboxFilter>();

                        services.AddSingleton(
                            delegate (System.IServiceProvider provider)
                            {
                                SmtpServer.ISmtpServerOptions options = new SmtpServer.SmtpServerOptionsBuilder()

                                    .ServerName("AnySql SMTP Server")
                                    .Endpoint(builder => builder.Port(25).IsSecure(false))

#if false 
                                    // For a brief period in time this was a recognized port whereby
                                    // TLS was enabled by default on the connection. When connecting to
                                    // port 465 the client will upgrade its connection to SSL before
                                    // doing anything else. Port 465 is obsolete in favor of using
                                    // port 587 but it is still available by some mail servers.
                                    .Endpoint(builder => 
                                        builder
                                            .Port(465)
                                            .IsSecure(true) // indicates that the client will need to upgrade to SSL upon connection
                                            .Certificate(new System.Security.Cryptography.X509Certificates.X509Certificate2())) // requires a valid certificate to be configured

                                    // Port 587 is the default port that should be used by modern mail
                                    // clients. When a certificate is provided, the server will advertise
                                    // that is supports the STARTTLS command which allows the client
                                    // to determine when they want to upgrade the connection to SSL. 
                                    .Endpoint(builder => 
                                        builder
                                            .Port(587)
                                            .AllowUnsecureAuthentication(false) // using 'false' here means that the user cant authenticate unless the connection is secure
                                            .Certificate(new System.Security.Cryptography.X509Certificates.X509Certificate2())) // requires a valid certificate to be configured
#endif

                                    .Build();

                                return new SmtpServer.SmtpServer(options, provider.GetRequiredService<System.IServiceProvider>());
                            }
                        );

                        services.AddHostedService<Worker>();
                    });
        } // End Function CreateHostBuilder 



    } // End Class Program 


} // End Namespace 
