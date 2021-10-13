
namespace AnySqlSmtpServer.Logging
{

    public class NoopDisposable
        : System.IDisposable
    {

        // public static NoopDisposable Instance;

        public static NoopDisposable Instance
        {
            get
            {
                return new NoopDisposable();
            }
        }


        static NoopDisposable()
        {
            // Instance = new NoopDisposable();
        }

        public NoopDisposable()
        { }


        public void Dispose()
        { }

    }


}
