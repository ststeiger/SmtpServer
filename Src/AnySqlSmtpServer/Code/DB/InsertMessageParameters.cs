
namespace AnySqlSmtpServer.DB 
{


    public class InsertMessageParameters
    {
        public System.Guid __msg_uid { get; set; }
        public string __msg_mailbox { get; set; }
        public string __msg_host { get; set; }
        public System.DateTimeOffset __msg_utc_timestamp { get; set; } = System.DateTimeOffset.UtcNow;
        public string __msg_from_mailbox { get; set; }
        public string __msg_from_host { get; set; }
        public string __msg_unmapped { get; set; }
        public string __msg_ip_v4 { get; set; }
        public string __msg_ip_v6 { get; set; }
        public string __msg_addressFamily { get; set; }
        public int? __msg_port { get; set; }
        public string __msg_id { get; set; }
        public string __msg_subject { get; set; }
        public string __msg_body { get; set; }
        public byte[] __msg_bytes { get; set; }
    }


}
