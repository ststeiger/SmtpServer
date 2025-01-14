﻿
using SyslogNet.Client.Serialization;


namespace SyslogNet.Client.Transport
{


    public class SyslogLocalSender
        : ISyslogMessageSender, System.IDisposable
    {


        // Will not work on Windows, as this relies on Unix system calls
        public SyslogLocalSender()
        {
            System.PlatformID platform = System.Environment.OSVersion.Platform;
            if (!(platform == System.PlatformID.MacOSX || platform == System.PlatformID.Unix))
            {
                throw new SyslogNetException("SyslogLocalSender is only available on Unix-like systems (e.g., Linux, BSD, OS X)");
            }
        }


        [System.Runtime.InteropServices.DllImport("libc", ExactSpelling = true)]
        // Because openlog() makes a copy of the char *ident that it gets passed, we have to
        // make sure it gets marshalled into unmanaged memory. Hence the IntPtr ident parameter.
        protected static extern void openlog(System.IntPtr ident, int option, int facility);

        [System.Runtime.InteropServices.DllImport("libc", CharSet = System.Runtime.InteropServices.CharSet.Ansi, ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        protected static extern void syslog(int priority, string fmt, byte[] msg);

        [System.Runtime.InteropServices.DllImport("libc", ExactSpelling = true)]
        protected static extern void closelog();

        public static ISyslogMessageSerializer defaultSerializer = new SyslogLocalMessageSerializer();


        public static int CalculatePriorityValue(Facility? facility, Severity? severity)
        {
            return ((int)facility << 3) | (int)severity;
        }


        public void Reconnect()
        {
            // Not needed; glibc syslog() is effectively "connectionless"
        }


        protected System.IntPtr MarshalIdent(string ident)
        {
            return (ident == null) ? System.IntPtr.Zero : System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(ident);
        }


        protected void DisposeOfIdent(System.IntPtr identPtr)
        {
            if (identPtr != System.IntPtr.Zero)
                System.Runtime.InteropServices.Marshal.FreeHGlobal(identPtr);
        }


        protected ISyslogMessageSerializer EnsureValidSerializer(ISyslogMessageSerializer serializer)
        {
            return serializer ?? defaultSerializer;
        }


        protected void SendToSyslog(SyslogMessage message, ISyslogMessageSerializer serializer)
        {
            int priority = CalculatePriorityValue(message.Facility, message.Severity);
            serializer = EnsureValidSerializer(serializer);
            byte[] data = serializer.Serialize(message);
            syslog(priority, "%s", data);
        }


        public void Send(SyslogMessage message, ISyslogMessageSerializer serializer)
        {
            System.IntPtr ident = System.IntPtr.Zero;
            try
            {
                ident = MarshalIdent(message.AppName);
                openlog(ident, (int)SyslogOptions.LogPid, CalculatePriorityValue(message.Facility, 0));
                SendToSyslog(message, serializer);
            }
            finally
            {
                closelog();
                DisposeOfIdent(ident);
            }
        }


        public void Send(System.Collections.Generic.IEnumerable<SyslogMessage> messages, ISyslogMessageSerializer serializer)
        {
            // Slightly tricky, since we need to get the appName out of the first message before
            // looping, so we can't just use foreach(). Using an explicit iterator works, though.
            System.IntPtr ident = System.IntPtr.Zero;
            using (System.Collections.Generic.IEnumerator<SyslogMessage> iterator = messages.GetEnumerator())
            {
                try
                {
                    if (iterator.MoveNext())
                    {
                        SyslogMessage message = iterator.Current;
                        ident = MarshalIdent(message.AppName);
                        openlog(ident, (int)SyslogOptions.LogPid, CalculatePriorityValue(message.Facility, 0));
                        SendToSyslog(message, serializer);
                    }
                    while (iterator.MoveNext())
                    {
                        SendToSyslog(iterator.Current, serializer);
                    }
                }
                finally
                {
                    closelog();
                    DisposeOfIdent(ident);
                }
            }
        }


        // Convenience overloads since there's only one possible serializer for local syslog messages
        public void Send(SyslogMessage message)
        {
            Send(message, defaultSerializer);
        }


        public void Send(System.Collections.Generic.IEnumerable<SyslogMessage> messages)
        {
            Send(messages, defaultSerializer);
        }


        public void Dispose()
        {
            // No action needed as we don't keep any state
        }


        [System.Flags]
        protected enum SyslogOptions : int
        {
            LogPid = 1,
            LogToConsoleIfErrorSendingToSyslog = 2,
            DelayOpenUntilFirstSyslogCall = 4,
            DontDelayOpen = 8,
            DEPRECATED_DontWaitForConsoleForks = 16,
            AlsoLogToStderr = 32
        }


    }


}
