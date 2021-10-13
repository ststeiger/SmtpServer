
namespace AnySqlSmtpServer 
{


    internal static class TaskExtensions
    {


        internal static void WaitWithoutException(this System.Threading.Tasks.Task task)
        {
            try
            {
                task.Wait();
            }
            catch (System.AggregateException e)
            {
                e.Handle(exception => exception is System.OperationCanceledException);
            }
        }


    }


}