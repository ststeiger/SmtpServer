
-- SELECT ',messages.' ||column_name FROM information_schema.columns WHERE table_name = 'messages'


SELECT 
	 messages.msg_uid
	-- ,messages_map_content.msg_bytes 
	-- ,messages_map_content.msg_body 
	 
	-- ,messages.msg_uid 
	,messages.msg_mailbox 
	,messages.msg_host 
	,messages.msg_utc_timestamp 
	,messages.msg_from_mailbox 
	,messages.msg_from_host 
	-- ,messages.msg_unmapped 
	-- ,messages.msg_ip_v4 
	-- ,messages.msg_ip_v6 
	-- ,messages.msg_addressfamily 
	-- ,messages.msg_port 
	-- ,messages.msg_id 
	,messages.msg_subject 
	-- ,messages.msg_bytes 
FROM messages 

LEFT JOIN messages_map_content ON messages_map_content.msg_uid = messages.msg_uid 

ORDER BY msg_utc_timestamp DESC ; 
