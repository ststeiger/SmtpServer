
namespace AnySqlSmtpServer.DB 
{


    internal static class Commands 
    {


		public static readonly string Create_Message_Table = @"

USE server_mail
GO


ALTER DATABASE server_mail SET COMPATIBILITY_LEVEL = 130
GO 


IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'smtp' ) 
BEGIN 
    EXEC('CREATE SCHEMA smtp;'); 
END 


GO 


DROP VIEW IF EXISTS smtp.v_dev_messages;
GO

DROP TABLE IF EXISTS smtp.messages_map_content; 
GO 

DROP TABLE IF EXISTS smtp.messages; 
GO

CREATE TABLE smtp.messages
(
	 msg_uid uniqueidentifier NOT NULL CONSTRAINT pk_messages PRIMARY KEY -- uuid
	,msg_mailbox national character varying(1000) 
	,msg_host national character varying(1000) 
	,msg_utc_timestamp datetime -- timestamp without time zone 
	 
	,msg_from_mailbox national character varying(1000) 
	,msg_from_host national character varying(1000) 
	,msg_unmapped character varying(255) 
	,msg_ip_v4 character varying(255) 
	,msg_ip_v6 character varying(255) 
	,msg_addressFamily character varying(255) 
	,msg_port integer 
	 
	,msg_id national character varying(1000) 
	 -- RFC 2822 states that the maximum number of characters in a subject line is 998 characters.
	,msg_subject national character varying(1000) 
	,msg_bytes binary varying(MAX) -- bytea 
);


GO


CREATE TABLE smtp.messages_map_content
(
	 msg_uid uniqueidentifier NOT NULL CONSTRAINT pk_messages_map_content PRIMARY KEY(msg_uid) -- uuid  
	,msg_bytes binary varying(MAX) -- bytea
	,msg_body national character varying(MAX) 
	,CONSTRAINT fk_messages_map_content_messages FOREIGN KEY(msg_uid) REFERENCES smtp.messages (msg_uid) 
	 
);


/*

DROP VIEW IF EXISTS smtp.v_dev_messages; 
DROP TABLE IF EXISTS smtp.messages_map_content; 
DROP TABLE IF EXISTS smtp.messages; 


CREATE TABLE IF NOT EXISTS smtp.messages
(
     msg_uid uuid NOT NULL CONSTRAINT pk_messages PRIMARY KEY 
    ,msg_mailbox character varying(1000) 
    ,msg_host character varying(1000) 
    ,msg_utc_timestamp timestamp without time zone 
    ,msg_from_mailbox character varying(1000) 
    ,msg_from_host character varying(1000) 
    ,msg_unmapped character varying(255) 
    ,msg_ip_v4 character varying(255) 
    ,msg_ip_v6 character varying(255) 
    ,msg_addressfamily character varying(255) 
    ,msg_port integer 
    ,msg_id character varying(1000) 
	 -- RFC 2822 states that the maximum number of characters in a subject line is 998 characters.
	,msg_subject national character varying(1000) 
    ,msg_bytes bytea 
    -- ,CONSTRAINT pk_messages PRIMARY KEY (msg_uid) 
); 


CREATE TABLE IF NOT EXISTS smtp.messages_map_content
(
	 msg_uid uuid NOT NULL CONSTRAINT pk_messages_map_content PRIMARY KEY 
	,msg_bytes bytea 
	,msg_body national character varying 
	--,CONSTRAINT pk_messages_map_content PRIMARY KEY(msg_uid) 
	,CONSTRAINT fk_messages_map_content_messages FOREIGN KEY(msg_uid) REFERENCES smtp.messages (msg_uid) 
); 


*/

";




        public static readonly string Insert_Message = @"

/*
DECLARE @__msg_uid uniqueidentifier
DECLARE @__msg_mailbox nvarchar(1000)
DECLARE @__msg_host nvarchar(1000)
DECLARE @__msg_utc_timestamp datetime
DECLARE @__msg_from_mailbox nvarchar(1000)
DECLARE @__msg_from_host nvarchar(1000)
DECLARE @__msg_unmapped varchar(255)
DECLARE @__msg_ip_v4 varchar(255)
DECLARE @__msg_ip_v6 varchar(255)
DECLARE @__msg_addressFamily varchar(255)
DECLARE @__msg_port int
DECLARE @__msg_id nvarchar(1000)
DECLARE @__msg_subject national character varying(1000) 
DECLARE @__msg_body national character varying(MAX) 
DECLARE @__msg_bytes varbinary(max)


SET @__msg_uid = NEWID()
-- SET @__msg_mailbox -- nvarchar(1000)
-- SET @__msg_host -- nvarchar(1000)
-- SET @__msg_utc_timestamp -- datetime
-- SET @__msg_from_mailbox -- nvarchar(1000)
-- SET @__msg_from_host -- nvarchar(1000)
-- SET @__msg_unmapped -- varchar(255)
-- SET @__msg_ip_v4 -- varchar(255)
-- SET @__msg_ip_v6 -- varchar(255)
-- SET @__msg_addressFamily -- varchar(255)
-- SET @__msg_port -- int
-- SET @__msg_id -- nvarchar(1000)
-- SET @__msg_bytes -- varbinary(max)
-- SET @__msg_subject -- national character varying(1000) 
-- SET @__msg_body -- national character varying(MAX) 

-- SELECT * FROM smtp.messages_map_content; 
-- SELECT * FROM smtp.messages; 

IF 1=2 
BEGIN 
	TRUNCATE TABLE smtp.messages_map_content; 
	DELETE FROM smtp.messages; 
END 
*/


INSERT INTO smtp.messages 
( 
	 msg_uid 
	,msg_mailbox 
	,msg_host 
	,msg_utc_timestamp 
	,msg_from_mailbox 
	,msg_from_host 
	 
	,msg_unmapped 
	,msg_ip_v4 
	,msg_ip_v6 
	,msg_addressFamily 
	,msg_port 
	,msg_id 
	,msg_subject 
	,msg_bytes 
) 
VALUES 
( 
	 @__msg_uid -- uniqueidentifier
	,@__msg_mailbox -- nvarchar(1000)
	,@__msg_host -- nvarchar(1000)
	,@__msg_utc_timestamp -- datetime
	,@__msg_from_mailbox -- nvarchar(1000)
	,@__msg_from_host -- nvarchar(1000)
	,@__msg_unmapped -- varchar(255)
	,@__msg_ip_v4 -- varchar(255)
	,@__msg_ip_v6 -- varchar(255)
	,@__msg_addressFamily -- varchar(255)
	,@__msg_port -- int
	,@__msg_id -- nvarchar(1000)
	,@__msg_subject -- national character varying(1000) 
	,@__msg_bytes -- varbinary(max)
); 

INSERT INTO smtp.messages_map_content(msg_uid, msg_bytes, msg_body) VALUES ( @__msg_uid, @__msg_bytes,@__msg_body ); 

";



        public static readonly string Query_Messages = @"
IF 1=2 
BEGIN 
	TRUNCATE TABLE smtp.messages_map_content; 
	DELETE FROM smtp.messages; 
END 

DROP VIEW IF EXISTS smtp.v_dev_messages;
GO

CREATE VIEW smtp.v_dev_messages AS 
SELECT 
	 messages.msg_uid 
	,messages.msg_mailbox 
	,messages.msg_host 
	,messages.msg_utc_timestamp 
	,messages.msg_from_mailbox 
	,messages.msg_from_host 
	,messages.msg_unmapped 
	,messages.msg_ip_v4 
	,messages.msg_ip_v6 
	,messages.msg_addressFamily 
	,messages.msg_port 
	,messages.msg_id 
	,messages.msg_subject 
	-- ,messages.msg_bytes 
	,messages_map_content.msg_bytes 
	,messages_map_content.msg_body 
FROM smtp.messages 

LEFT JOIN smtp .messages_map_content 
	ON messages_map_content .msg_uid  = messages.msg_uid 
";
	} // End Class Commands 


} // End Namespace 
