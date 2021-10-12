
namespace SampleApp
{


    public sealed class SampleUserAuthenticator 
        : SmtpServer.Authentication.UserAuthenticator
    {
        /// <summary>
        /// Authenticate a user account.
        /// </summary>
        /// <param name="context">The session context.</param>
        /// <param name="user">The user to authenticate.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>true if the user is authenticated, false if not.</returns>
        public override System.Threading.Tasks.Task<bool> AuthenticateAsync(
            SmtpServer.ISessionContext context, 
            string user, 
            string password,
            System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult(user == "user" && password == "password");
        }


    }


}