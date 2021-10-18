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
