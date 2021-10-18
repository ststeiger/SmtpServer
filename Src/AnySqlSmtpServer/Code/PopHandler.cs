using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnySqlSmtpServer.Code
{


    public enum Pop3Command
    {
        STLS,
        USER,
        PASS,
        AUTH,
        STAT,
        LIST,
        UIDL,
        TOP,
        RETR,
        DELE,
        NOOP,
        RSET,
        CAPA,
        QUIT,

    }



    public delegate void foo_t(string args);

    public class POP3_ServerMessage
    { 

    }


    public class POP3_e_GetMessagesInfo : EventArgs
    {
        private List<POP3_ServerMessage> m_pMessages = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal POP3_e_GetMessagesInfo()
        {
            m_pMessages = new List<POP3_ServerMessage>();
        }


        #region Properties implementation

        /// <summary>
        /// Gets POP3 messages info collection.
        /// </summary>
        public List<POP3_ServerMessage> Messages
        {
            get { return m_pMessages; }
        }

        #endregion
    }


    class PopHandler
    {

        string[] commands;
        System.Collections.Generic.Dictionary<string, foo_t> commandHandlers;


        public void foo(string args)
        { }


        public PopHandler()
        {
            this.commands = new string[] {
                "STLS",
                "USER",
                "PASS",
                "AUTH",
                "STAT",
                "LIST",
                "UIDL",
                "TOP",
                "RETR",
                "DELE",
                "NOOP",
                "RSET",
                "CAPA",
                "QUIT"
            };

            commandHandlers = new Dictionary<string, foo_t>(System.StringComparer.InvariantCultureIgnoreCase);
            for (int i = 0; i < commands.Length; ++i)
            {
                commandHandlers.Add(commands[i], foo);
            }

        }




        public bool IsAuthenticated;


        private System.IO.StreamWriter TcpStream;

        private void WriteLine(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            // int countWritten = this.TcpStream.WriteLine(line);

            // Log.
            //if (this.Server.Logger != null)
            //{
            //    this.Server.Logger.AddWrite(this.ID, this.AuthenticatedUserIdentity, countWritten, line, this.LocalEndPoint, this.RemoteEndPoint);
            //}
        }

        private void PASS(string cmdText)
        {
            /* RFC 1939 7. PASS
			Arguments:
				a server/mailbox-specific password (required)
				
			Restrictions:
				may only be given in the AUTHORIZATION state immediately
				after a successful USER command
				
			NOTE:
				When the client issues the PASS command, the POP3 server
				uses the argument pair from the USER and PASS commands to
				determine if the client should be given access to the
				appropriate maildrop.
				
			Possible Responses:
				+OK maildrop locked and ready
				-ERR invalid password
				-ERR unable to lock maildrop
						
			*/

            bool m_SessionRejected = false;
            string m_UserName = null;

            if (m_SessionRejected)
            {
                WriteLine("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (this.IsAuthenticated)
            {
                this.TcpStream.WriteLine("-ERR Re-authentication error.");

                return;
            }
            if (m_UserName == null)
            {
                this.TcpStream.WriteLine("-ERR Specify user name first.");

                return;
            }
            if (string.IsNullOrEmpty(cmdText))
            {
                this.TcpStream.WriteLine("-ERR Error in arguments.");

                return;
            }

            /*
            POP3_e_Authenticate e = OnAuthenticate(m_UserName, cmdText);
            if (e.IsAuthenticated)
            {
                m_pUser = new GenericIdentity(m_UserName, "POP3-USER/PASS");

                // Get mailbox messages.
                POP3_e_GetMessagesInfo eMessages = OnGetMessagesInfo();
                int seqNo = 1;
                foreach (POP3_ServerMessage message in eMessages.Messages)
                {
                    message.SequenceNumber = seqNo++;
                    m_pMessages.Add(message.UID, message);
                }

                this.TcpStream.WriteLine("+OK Authenticated successfully.");
            }
            else
            {
                this.TcpStream.WriteLine("-ERR Authentication failed.");
            }
            */
            
        }


        private string m_GreetingText = "";

        protected void Start()
        {
            // base.Start();

            /* RFC 1939 4.
                Once the TCP connection has been opened by a POP3 client, the POP3
                server issues a one line greeting.  This can be any positive
                response.  An example might be:
                    S:  +OK POP3 server ready
            */

            try
            {
                string reply = null;
                if (string.IsNullOrEmpty(this.m_GreetingText))
                {
                    // reply = "+OK [" + Net_Utils.GetLocalHostName(this.LocalHostName) + "] POP3 Service Ready.";
                    string lhn = System.Net.Dns.GetHostName();
                    reply = "+OK [" + lhn + "] POP3 Service Ready.";
                }
                else
                {
                    reply = "+OK " + this.m_GreetingText;
                }

                /*
                POP3_e_Started e = OnStarted(reply);

                if (!string.IsNullOrEmpty(e.Response))
                {
                    WriteLine(reply.ToString());
                }
                */
                // Setup rejected flag, so we respond "-ERR Session rejected." any command except QUIT.
                //if (string.IsNullOrEmpty(e.Response) || e.Response.ToUpper().StartsWith("-ERR"))
                //{
                //    m_SessionRejected = true;
                //}

                //BeginReadCmd();
            }
            catch (Exception x)
            {
                System.Console.WriteLine(x.Message);
                System.Console.WriteLine(x.StackTrace);
                //OnError(x);
            }
        }
        private void BeginReadCmd()
        {
            /*
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                SmartStream.ReadLineAsyncOP readLineOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
                // This event is raised only when read next coomand completes asynchronously.
                readLineOP.CompletedAsync += new EventHandler<EventArgs<SmartStream.ReadLineAsyncOP>>(delegate (object sender, EventArgs<SmartStream.ReadLineAsyncOP> e)
                {
                    if (ProcessCmd(readLineOP))
                    {
                        BeginReadCmd();
                    }
                });
                // Process incoming commands while, command reading completes synchronously.
                while (this.TcpStream.ReadLine(readLineOP, true))
                {
                    if (!ProcessCmd(readLineOP))
                    {
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                OnError(x);
            }
            */
        }


        private void QUIT(string cmdText)
        {
            /* RFC 1939 6. QUIT
			   NOTE:
                When the client issues the QUIT command from the TRANSACTION state,
				the POP3 session enters the UPDATE state.  (Note that if the client
				issues the QUIT command from the AUTHORIZATION state, the POP3
				session terminates but does NOT enter the UPDATE state.)

				If a session terminates for some reason other than a client-issued
				QUIT command, the POP3 session does NOT enter the UPDATE state and
				MUST not remove any messages from the maildrop.
             
				The POP3 server removes all messages marked as deleted
				from the maildrop and replies as to the status of this
				operation.  If there is an error, such as a resource
				shortage, encountered while removing messages, the
				maildrop may result in having some or none of the messages
				marked as deleted be removed.  In no case may the server
				remove any messages not marked as deleted.

				Whether the removal was successful or not, the server
				then releases any exclusive-access lock on the maildrop
				and closes the TCP connection.
			*/

            try
            {
                //if (this.IsAuthenticated)
                //{
                //    // Delete messages marked for deletion.
                //    foreach (POP3_ServerMessage msg in m_pMessages)
                //    {
                //        if (msg.IsMarkedForDeletion)
                //        {
                //            OnDeleteMessage(msg);
                //        }
                //    }
                //}

                string localHostName = System.Net.Dns.GetHostName();

                // cached lh name
                // WriteLine("+OK <" + Net_Utils.GetLocalHostName(this.LocalHostName) + "> Service closing transmission channel.");
                WriteLine("+OK <" + localHostName + "> Service closing transmission channel.");
            }
            catch
            {
            }
            // Disconnect();
            // Dispose();
        }


        private bool ProcessCmd(SmartStream op)
        {
            bool readNextCommand = true;
            try
            {
                int m_BadCommands = 0;

                string[] cmd_args = Encoding.UTF8.GetString(op.Buffer, 0, op.LineBytesInBuffer).Split(new char[] { ' ' }, 2);
                string cmd = cmd_args[0].ToUpperInvariant();
                string args = cmd_args.Length == 2 ? cmd_args[1] : "";


                // Logging(); // Hide password from log.

                if (commandHandlers.ContainsKey(cmd))
                {
                    commandHandlers[cmd](args);
                }
                else
                {
                    m_BadCommands++;
                    // if(m_BadCommands = too_many)
                    // WriteLine("-ERR Too many bad commands, closing transmission channel."); Disconnect();
                    // else
                    // WriteLine("-ERR Error: command '" + cmd + "' not recognized.");
                }

            }
            catch (System.Exception ex)
            {

            }

            return readNextCommand;
        }


    }

    public class SmartStream
    {
        public byte[] Buffer;
        public int LineBytesInBuffer;
    }

}
