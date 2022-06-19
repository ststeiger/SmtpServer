-- Table: public.messages

-- DROP TABLE IF EXISTS public.messages;

CREATE TABLE IF NOT EXISTS public.messages
(
     msg_uid uuid NOT NULL 
    ,msg_mailbox character varying(1000) COLLATE pg_catalog."default" 
    ,msg_host character varying(1000) COLLATE pg_catalog."default" 
    ,msg_utc_timestamp timestamp without time zone 
    ,msg_from_mailbox character varying(1000) COLLATE pg_catalog."default" 
    ,msg_from_host character varying(1000) COLLATE pg_catalog."default" 
    ,msg_unmapped character varying(255) COLLATE pg_catalog."default" 
    ,msg_ip_v4 character varying(255) COLLATE pg_catalog."default" 
    ,msg_ip_v6 character varying(255) COLLATE pg_catalog."default" 
    ,msg_addressfamily character varying(255) COLLATE pg_catalog."default" 
    ,msg_port integer 
    ,msg_id character varying(1000) COLLATE pg_catalog."default" 
    ,msg_subject character varying(1000) COLLATE pg_catalog."default" 
    ,msg_bytes bytea 
    ,CONSTRAINT pk_messages PRIMARY KEY (msg_uid) 
) TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.messages OWNER to smtp_verver_web_services;
