
namespace SampleApp
{


    internal class Program
    {


        internal static void Main(string[] args)
        {
            CertificateCallback.Initialize();

            Examples.CommonPortsExample.Run();
            

            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();

            // SampleApp.Examples.SimpleExample.Run();
            // Examples.SimpleServerExample.Run();
            // Examples.CustomEndpointListenerExample.Run();
            // Examples.ServerCancellingExample.Run();
            // Examples.SessionTracingExample.Run();
            // Examples.DependencyInjectionExample.Run();
            // Examples.SecureServerExample.Run();

            // SampleMailClient.Send(user: "user1", password: "password1", useSsl: false, port: 587);
            // SampleMailClient.Send(useSsl: false, port: 587);

            // http://www.cs.cmu.edu/~enron/enron_mail_20150507.tar.gz
            // string[] files = System.IO.Directory.GetFiles(@"C:\Temp\enron_mail_20150507.tar", "*.*", System.IO.SearchOption.AllDirectories);
            // // System.Console.WriteLine(files.OrderByDescending(file => new System.IO.FileInfo(file).Length).First());
            // System.Array.Sort(files, delegate (string a, string b) { return new System.IO.FileInfo(b).Length.CompareTo(new System.IO.FileInfo(a).Length); });
            // System.Array.Sort(files, (a, b) => new System.IO.FileInfo(b).Length.CompareTo(new System.IO.FileInfo(a).Length));
        }


    }


}
