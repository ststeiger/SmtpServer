
namespace SampleApp
{


    public class SqlFactory
    {
        private static string s_ConnectionString;
        private static System.Data.Common.DbProviderFactory s_Factory;


        static SqlFactory()
        {
            s_ConnectionString = GetConnectionString();
            s_Factory = DbProviderFactories.GetFactory(
                typeof(System.Data.SqlClient.SqlClientFactory)
            );
        }


        public static System.Data.Common.DbConnection GetConnection()
        {
            System.Data.Common.DbConnection con = s_Factory.CreateConnection();
            con.ConnectionString = s_ConnectionString;

            if (con.State != System.Data.ConnectionState.Open)
                con.Open();

            return con;
        }


        public static string Connection_String
        {
            get { return GetConnectionString(); }
        }


        public static string GetConnectionString()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();

            csb.DataSource = System.Environment.MachineName; // SecretManager.GetSecret<string>("DataSource");
            csb.InitialCatalog = "server_mail";

            csb.IntegratedSecurity = true;
            if (!csb.IntegratedSecurity)
            { 
                csb.UserID = SecretManager.GetSecret<string>("DefaultDbUser");
                csb.Password = SecretManager.GetSecret<string>("DefaultDbPassword");
            }

            csb.PacketSize = 4096;
            csb.PersistSecurityInfo = false;
            csb.ApplicationName = "SMTP SampleServer";
            csb.ConnectTimeout = 15;
            csb.Pooling = true;
            csb.MinPoolSize = 1;
            csb.MaxPoolSize = 100;
            csb.MultipleActiveResultSets = false;
            csb.WorkstationID = System.Environment.MachineName;

            return csb.ConnectionString;
        }


        public string ConnectionString
        {
            get
            {
                return s_ConnectionString;
            }
        }


        public System.Data.Common.DbProviderFactory Factory
        {
            get
            {
                return s_Factory;
            }
        }


        public System.Data.Common.DbConnection Connection
        {
            get
            {
                System.Data.Common.DbConnection con = s_Factory.CreateConnection();
                con.ConnectionString = s_ConnectionString;

                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();

                return con;
            }
        }


        public SqlFactory()
        { }




    }


}
